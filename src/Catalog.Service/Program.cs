using Catalog.Service.Entities;
using Common.MassTransit;
using Common.MongoDb;
using Common.Settings;

ServiceSettings serviceSettings;
const string AllowedOriginSetting = "AllowedOrigin";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo().AddMongRepository<Item>("items")
.AddMasTransitWithRabbitMq();

// builder.Services.AddMassTransitHostedService();

builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// builder.Services.AddCors(options =>
//             {
//                 options.AddPolicy("MyAllowSpecificOrigins",
//                 builder =>
//                 {
//                     builder.WithOrigins("http://localhost:3000").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
//                 });
//             });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseCors(builders =>
    {
        builders.WithOrigins(builder.Configuration[AllowedOriginSetting])
        .AllowAnyHeader().AllowAnyMethod();
    });
}
// app.UseCors("MyAllowSpecificOrigins");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
