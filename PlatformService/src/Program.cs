using Microsoft.EntityFrameworkCore;
using Serilog;
using PlatformService.Data;
using PlatformService.Settings;
using PlatformService.Settings.Models;
using PlatformService.DataServices.Sync.Http;
using PlatformService.DataServices.Async;
using PlatformService.DataServices.Async.Grpc;

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
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

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
app.MapGrpcService<GrpcPlatformService>();
app.MapGet("/protos/platforms.proto", async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("/Protos/platforms.proto"));
    }
);

app.Run();
