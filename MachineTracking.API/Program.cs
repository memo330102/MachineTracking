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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://localhost:7045") // Frontend URL
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();  // Allow credentials (cookies, authentication)
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowSpecificOrigin");
app.MapHub<MachineDataHub>("/machinedatahub");

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
