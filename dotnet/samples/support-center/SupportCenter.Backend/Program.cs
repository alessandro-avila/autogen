// Copyright (c) Microsoft Corporation. All rights reserved.
// Program.cs

using Microsoft.AutoGen.Core;
using SupportCenter.Agents.CustomerInfo;
using SupportCenter.Agents.Discount;
using SupportCenter.Agents.Dispatcher;
using SupportCenter.Agents.Invoice;
using SupportCenter.Agents.QnA;
using SupportCenter.Agents.Services;
using SupportCenter.Agents.SignalR;
using SupportCenter.Shared.Extensions;
using SupportCenter.Shared.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
                //.AddNamedAzureSignalR("signalr"); ;

builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<Dispatcher>("dispatcher")
    .AddAgent<CustomerInfo>("customerInfo")
    .AddAgent<Discount>("discount")
    .AddAgent<Invoice>("invoice")
    .AddAgent<QnA>("qna")
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

app.MapHub<SupportCenterHub>("/supportcenterhub");
app.Run();
