using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.Settings;
using PlatformService.DataServices.Sync.Http;

var envVars = new EnvironmentVariables();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEnvironmentVariables, EnvironmentVariables>();
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

if(envVars.IsInMemoryDb())
{
    Console.WriteLine("--> Using InMem DB");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseInMemoryDatabase("InMen")
    );
}
else
{
    Console.WriteLine($"--> Using MSSQL DB: {envVars.GetConnectionString()}");
    builder.Services.AddDbContext<AppDbContext>(opt => 
        opt.UseSqlServer(envVars.GetConnectionString()) 
    );
}

var app = builder.Build();

PrepDb.PrepPopulation(app, envVars.IsInMemoryDb());

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
