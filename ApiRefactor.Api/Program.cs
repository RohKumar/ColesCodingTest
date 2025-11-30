using ApiRefactor.Api.Middlewares;

using ApiRefactor.Infrastructure;
using ApiRefactor.Application;

var builder = WebApplication.CreateBuilder(args);


// Connection string points to App_Data in the API project output directory
var connectionString = "Data Source=App_Data/waves.db";

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI registrations
builder.Services.AddSingleton<ISqlConnectionFactory>(_ => new SqliteConnectionFactory(connectionString));
builder.Services.AddScoped<IWaveRepository, SqliteWaveRepository>();
builder.Services.AddScoped<IWaveService, WaveService>();

var app = builder.Build();

// Global exception handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();


app.Run();
