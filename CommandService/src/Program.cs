using CommandService.Data;
using CommandService.DataServices.Async;
using CommandService.DataServices.Sync.Grpc;
using CommandService.EventProcessing;
using CommandService.Settings;
using CommandService.Settings.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var dbVariables = builder.Configuration.GetSection("Database").Get<DatabaseVariables>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEnvironmentVariables, EnvironmentVariables>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();
builder.Services.AddScoped<ICommandRepo, CommandRepo>();

if(bool.Parse(dbVariables.InMemoryDb))
{
    Log.Information("Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMen")
    );
}
else
{
    Log.Information($"Using MSSQL DB: {dbVariables.ConnectionString}");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(dbVariables.ConnectionString)
    );
}

var app = builder.Build();

PrepDb.PrepPopulation(app);

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
