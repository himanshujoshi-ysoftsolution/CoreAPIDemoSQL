using CoreAPIDemo.Infrastructure;
using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
SqlConnection.ClearAllPools();

// Register CosmosClient and Container in the dependency injection container
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var cosmosEndpoint = configuration["CosmosDb:COSMOS_ENDPOINT"];
    var cosmosKey = configuration["CosmosDb:COSMOS_KEY"];
    return new CosmosClient(cosmosEndpoint, cosmosKey);
});

builder.Services.AddSingleton<Container>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var cosmosDatabaseId = configuration["CosmosDb:COSMOS_DATABASE_ID"];
    var cosmosContainerId = configuration["CosmosDb:COSMOS_CONTAINER_ID"];
    var cosmosClient = sp.GetRequiredService<CosmosClient>();
    var cosmosDatabase = cosmosClient.GetDatabase(cosmosDatabaseId);
    return cosmosDatabase.GetContainer(cosmosContainerId);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
