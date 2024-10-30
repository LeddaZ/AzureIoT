using Microsoft.Azure.Cosmos;
using WebAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var cosmosDbConfig = builder.Configuration.GetSection("CosmosDb");
string endpoint = cosmosDbConfig["Endpoint"] ?? "";
string primaryKey = cosmosDbConfig["PrimaryKey"] ?? "";
string databaseId = cosmosDbConfig["DatabaseId"] ?? "";
string containerId = cosmosDbConfig["ContainerId"] ?? "";

builder.Services.AddSingleton<CosmosClient>(new CosmosClient(endpoint, primaryKey));
builder.Services.AddSingleton<ICosmosDbService>(provider => new CosmosDbService(
    provider.GetRequiredService<CosmosClient>(),
    databaseId,
    containerId
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
