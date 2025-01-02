using MachineTracking.Application.MiddleWare;
using MachineTracking.Application.Services;
using MachineTracking.Domain.Interfaces.Application;
using MachineTracking.Domain.Interfaces.Infrastructure;
using MachineTracking.Infrastructure.Connections.Dapper;
using MachineTracking.Infrastructure.Helpers;
using MachineTracking.Infrastructure.Repositories;
using MachineTracking.MessageBroker.Hubs;
using MachineTracking.MessageBroker.Services;
using MQTTnet;
using MQTTnet.Client;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    var allowedOrigin = builder.Configuration["AllowedOrigins:FrontendURL"];
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins(allowedOrigin)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});
builder.Services.AddTransient<ISqlQuery, SqlQuery>();

builder.Services.AddTransient<IMqttDataService, MqttDataService>();
builder.Services.AddTransient<IMachineHistoryService, MachineHistoryService>();

builder.Services.AddTransient<IMachineHistoryRepository, MachineHistoryRepository>();

builder.Services.AddSingleton<DbConnections>();
builder.Services.AddSingleton<IMqttClient>(new MqttFactory().CreateMqttClient());
builder.Services.AddHostedService<MqttBackendService>();

builder.Services.AddSignalR();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .CreateLogger();

builder.Host.UseSerilog(); 

builder.Services.AddSingleton(Log.Logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowSpecificOrigin");
app.MapHub<MachineDataHub>(builder.Configuration["Hubs:MachineHub"]);

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
