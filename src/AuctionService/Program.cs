using AuctionService.Data;
using dotenv.net;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var root = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\.."));
var dotenv = Path.Combine(root, ".env");
DotEnv.Load(options: new DotEnvOptions(envFilePaths: [dotenv]));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<AuctionDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        .Replace("[PostgresPassword]", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"));
    opt.UseNpgsql(connectionString);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try
{
    DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();