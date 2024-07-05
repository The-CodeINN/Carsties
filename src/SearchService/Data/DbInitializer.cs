using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;
using System.Text.Json;

namespace SearchService.Data
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            var mongoClientSettings = MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection"));

            await DB.InitAsync("SearchDb", mongoClientSettings);

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();

            // Getting data from json file

            //if (count == 0) 
            //{
            //    Console.WriteLine("No items found in the database. Seeding data...");
            //    var itemData = await File.ReadAllTextAsync("Data/auction.json");

            //    var options = new JsonSerializerOptions
            //    {
            //        PropertyNameCaseInsensitive = true,
            //    };

            //    var items = JsonSerializer.Deserialize<List<Item>>(itemData, options);

            //    await DB.SaveAsync(items);
            //}

            // Getting data through async communication
            using var scope = app.Services.CreateScope();

            var auctionServiceHttpClient = scope.ServiceProvider.GetRequiredService<AuctionServiceHttpClient>();

            var items = await auctionServiceHttpClient.GetItemsForSearchDb();

            Console.WriteLine(items.Count + " returned from the aution service");

            if(items.Count > 0)
            {
                await DB.SaveAsync(items);
            } 
        }
    }
}
