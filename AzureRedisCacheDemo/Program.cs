using AzureRedisCacheDemo.Data;
using AzureRedisCacheDemo.Repositories;
using AzureRedisCacheDemo.Repositories.AzureRedisCache;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddDbContext<DataContext>
(o => o.UseInMemoryDatabase("RedisCacheDemo"));
builder.Services.AddScoped<IRedisCache, RedisCache>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();

    if (context.Products.Any())
    {
        return;
    }
    context.Products.AddRange(
        new Product
        {
            Id = 1,
            Name = "IPhone",
            Price = 120000,
            Stock = 100
        },
        new Product
        {
            Id = 2,
            Name = "Samsung TV",
            Price = 400000,
            Stock = 120
        });
    _ = context.SaveChanges();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.UseSwagger();
    _ = app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
