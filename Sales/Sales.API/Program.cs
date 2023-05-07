using Microsoft.EntityFrameworkCore;
using Sales.API.Data;
using Sales.API.Intefaces;
using Sales.API.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var devCorsPolicy = "devCorsPolicy";

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<DataContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("DOCKER_SALES")); });
builder.Services.AddTransient<SeedDb>();
builder.Services.AddHttpClient("CoutriesAPI", httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.Configuration.GetSection("CoutriesAPI").GetValue<string>("urlBase")!);
    httpClient.DefaultRequestHeaders.Add(builder.Configuration.GetSection("CoutriesAPI").GetValue<string>("tokenName")!,
        builder.Configuration.GetSection("CoutriesAPI").GetValue<string>("tokenValue"));
});
builder.Services.AddScoped<IApiService, ApiService>();

builder.Services.AddControllers();
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
SeedData(app);

void SeedData(WebApplication app)
{
    IServiceScopeFactory? scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (IServiceScope? scope = scopedFactory!.CreateScope())
    {
        SeedDb? service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }

}

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
