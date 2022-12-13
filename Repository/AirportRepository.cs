using AutoCompleteDemo.Models;
using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace AutoCompleteDemo.Repository
{
    public class AirportRepository
    {
        private readonly IRedisConnection? _connection;
        private const string SUG_KEY = "sugg:airport:name";

        public AirportRepository(RedisConnectionProvider redisConnectionProvider)
        {
            _connection = redisConnectionProvider.Connection;
        }
        
        public async Task<List<Suggestion>> AutoCompleteName(string perfix, bool WITHPAYLOAD = false)
        {
            var args = new List<string>();
            var ret = new List<Suggestion>();
            args.Add(SUG_KEY);
            args.Add(perfix);
            if(WITHPAYLOAD)
            {
                args.Add("WITHPAYLOADS");
            }
            RedisReply[] res = await _connection.ExecuteAsync("FT.SUGGET", args.ToArray());
            for (var i = 0; i < res.Length; i += 2)
            {
                ret.Add(new Suggestion { Name = res[i], Payload = res[i+1]});
            }

            return ret;
        }       
    }
}
