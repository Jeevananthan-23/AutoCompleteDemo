using AutoCompleteDemo.Repository;
using Microsoft.Extensions.FileProviders;
using Redis.OM;

var builder = WebApplication.CreateBuilder(args);

//redis connect
builder.Services.AddSingleton(new RedisConnectionProvider(builder.Configuration.GetValue<string>("ConnectionString:Redis")));
builder.Services.AddControllers();
builder.Services.AddSpaStaticFiles();
builder.Services.AddSingleton<AirportRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    DataSeeder.Seed(serviceScope).Wait();
}

app.Map(new PathString(""), client =>
{
    var clientPath = Path.Combine(Directory.GetCurrentDirectory(), "./ClientApp/static");
    StaticFileOptions clientAppDist = new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(clientPath)
    };
    client.UseSpaStaticFiles(clientAppDist);
    client.UseSpa(spa => { spa.Options.DefaultPageStaticFileOptions = clientAppDist; });

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();

    });
});

app.Run();
