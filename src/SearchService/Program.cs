using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var mongoClientSettings = MongoClientSettings.FromConnectionString(builder.Configuration.GetConnectionString("MongoDbConnection"));
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoClientSettings));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try
{
    await DB.InitAsync("SearchDb", mongoClientSettings);

    await DB.Index<Item>()
        .Key(x => x.Make, KeyType.Text)
        .Key(x => x.Model, KeyType.Text)
        .Key(x => x.Color, KeyType.Text)
        .CreateAsync();
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();
