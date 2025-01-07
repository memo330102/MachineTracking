using MachineTracking.Application.Configurations;
using MachineTracking.Application.Helpers;
using MachineTracking.Application.MiddleWare;
using MachineTracking.Application.Services;
using MachineTracking.Domain.DTOs.MQTT;
using MachineTracking.Domain.Interfaces.Application;
using MachineTracking.Domain.Interfaces.Infrastructure;
using MachineTracking.Infrastructure.Connections.Dapper;
using MachineTracking.Infrastructure.Helpers;
using MachineTracking.Infrastructure.Repositories;
using MachineTracking.MessageBroker.Hubs;
using MachineTracking.MessageBroker.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
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
builder.Services.AddTransient<IMachineHistoryHelper, MachineHistoryHelper>();

builder.Services.AddSingleton<DbConnections>();
builder.Services.AddSingleton<IMqttClient>(new MqttFactory().CreateMqttClient());
builder.Services.AddHostedService<MqttBackendService>();

builder.Services.Configure<MqttSettings>(builder.Configuration.GetSection("MQTTSettings"));

builder.Services.AddSignalR();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) 
    .CreateLogger();

builder.Host.UseSerilog(); 

builder.Services.AddSingleton(Log.Logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
#region Services.Swagger.ApiVersioning
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
});
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();
#endregion

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseCors("AllowSpecificOrigin");
app.MapHub<MachineDataHub>(builder.Configuration["Hubs:MachineHub"]);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", "Version: " + description.GroupName.ToUpperInvariant());
        }
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
