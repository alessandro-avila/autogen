using SupportCenter.Agents;
using SupportCenter.Shared;
using Microsoft.AutoGen.Agents;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureSemanticKernel();

builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<QnA>("qna");

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
