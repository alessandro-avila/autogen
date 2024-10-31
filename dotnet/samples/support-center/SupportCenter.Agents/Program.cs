using SupportCenter.Shared;
using Microsoft.AutoGen.Agents;
using SupportCenter.Agents.QnA;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureSemanticKernel();

builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<QnA>("qna");

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
