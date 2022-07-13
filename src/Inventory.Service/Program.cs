using Common.MassTransit;
using Common.MongoDb;
using Inventory.Service.Clients;
using Inventory.Service.Entities;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMongo()
                .AddMongRepository<InventoryItem>("inventoryitems")
                .AddMongRepository<CatalogItem>("catalogitems")
                .AddMasTransitWithRabbitMq();
builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

builder.Services.AddCors(options =>
            {
                options.AddPolicy("MyAllowSpecificOrigins",
                builder =>
                {
                    builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

AddCatalogClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
void AddCatalogClient()
{
    builder.Services.AddHttpClient<CatalogClient>(client => { client.BaseAddress = new Uri("https://localhost:5001"); })
        .AddTransientHttpErrorPolicy(builders => builders.Or<TimeoutRejectedException>().WaitAndRetryAsync(
            5, retryAttmenpt => TimeSpan.FromSeconds(Math.Pow(2, retryAttmenpt)),
            onRetry: (outcome, timespan, rettryAttempt) =>
            {
                var seviceProvider = builder.Services.BuildServiceProvider();

                seviceProvider.GetService<ILogger<CatalogClient>>()
                    ?.LogWarning($"Delay for {timespan.TotalSeconds} seconds, then making retry {rettryAttempt}");
            }
        ))
        .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
}

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyAllowSpecificOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
