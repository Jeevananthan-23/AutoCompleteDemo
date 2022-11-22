using AutoCompleteDemo.Models;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;
using System.Text.Json;

namespace AutoCompleteDemo.Repository
{
    public static class DataSeeder
    {
        private static IRedisConnection? _connection;
        private static IRedisCollection<Airport>? _airportCollection;
        private static readonly string SUG_KEY = "sugg:airport:name";
        public static async Task Seed(IServiceScope serviceScope)
        {
            var tasks = new List<Task<string>>();
            var args = new List<string>();
            var connectionProvider = serviceScope.ServiceProvider.GetService<RedisConnectionProvider>();
            _connection = connectionProvider.Connection;
            _airportCollection = connectionProvider.RedisCollection<Airport>();
            List<Airport>? airports = File.ReadAllLines("./Data/airport_codes.csv")
                                .Select(x => x.Split(","))
                                .Select(ar =>new Airport(){
                                    Name = ar[0],
                                    Code = ar[1],
                                    State = ar[2]
                                })
                                .ToList();
          var index =  _connection.CreateIndex(typeof(Airport));
            if (index is true)
            {
               
                foreach (var airport in airports)
                {
                    tasks.Add(_connection.SetAsync(airport));
                }
                await Task.WhenAll(tasks);
                var listOfAirports = await _airportCollection.ToListAsync();
                foreach (var airport in listOfAirports)
                {

                    args.Add(SUG_KEY);
                    args.Add(airport.Name);
                    args.Add("1.0");
                    args.Add("PAYLOAD");
                    args.Add(JsonSerializer.Serialize(airport));
                    await _connection.ExecuteAsync("FT.SUGADD", args.ToArray());
                    args.Clear();
                }
            }
        }
    }
}
