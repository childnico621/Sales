using Microsoft.EntityFrameworkCore;
using Sales.API.Data;

var builder = WebApplication.CreateBuilder(args);
var devCorsPolicy = "devCorsPolicy";
// Add services to the container.

builder.Services.AddDbContext<DataContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("sales")); });


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddCors(options =>
{
    options.AddPolicy(devCorsPolicy, bldr =>
    {
        bldr.WithOrigins("https://localhost:44360").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});


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

app.UseCors(devCorsPolicy);
app.Run();
