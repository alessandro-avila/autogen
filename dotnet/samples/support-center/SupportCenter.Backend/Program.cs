// Copyright (c) Microsoft Corporation. All rights reserved.
// Program.cs

using Microsoft.AutoGen.Core;
using SupportCenter.Backend.Agents;
using SupportCenter.Backend.Hubs;
using SupportCenter.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.ConfigureSemanticKernel();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<SignalRAgent>("signalr-hub");
builder.Services.AddSingleton<AgentWorker>();
builder.Services.AddSingleton<ISignalRService, SignalRService>();

// Allow any CORS origin if in DEV
const string AllowDebugOriginPolicy = "AllowDebugOrigin";
const string AllowOriginPolicy = "AllowOrigin";
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AllowDebugOriginPolicy, builder =>
        {
            builder
            .WithOrigins("http://localhost:3000", "http://localhost:3001") // client urls
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });
}
else
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy(AllowOriginPolicy, builder =>
        {
            builder
            .SetIsOriginAllowedToAllowWildcardSubdomains()
            .WithOrigins("https://*.azurecontainerapps.io") // client url
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
    });
}

builder.Services.ExtendOptions();
builder.Services.ExtendServices();
builder.Services.RegisterSemanticKernelNativeFunctions();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseRouting();
app.UseCors(AllowDebugOriginPolicy);
app.MapControllers();

/*app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
});*/

app.MapHub<SupportCenterHub>("/supportcenterhub");
app.Run();
