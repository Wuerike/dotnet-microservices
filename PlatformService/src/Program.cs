using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Settings;
using PlatformService.DataServices.Sync.Http;
using PlatformService.DataServices.Async;
using PlatformService.DataServices.Async.Grpc;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

var environment = new EnvironmentVariables();
var inMemoryDb = bool.Parse(environment.ApplicationVariables()["InMemoryDb"]);
var connectionString = environment.DatabaseVariables()["ConnectionString"];

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEnvironmentVariables, EnvironmentVariables>();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

if(inMemoryDb)
{
    Log.Information("Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMen")
    );
}
else
{
    Log.Information($"Using MSSQL DB: {connectionString}");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(connectionString) 
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
