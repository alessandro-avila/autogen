// Copyright (c) Microsoft Corporation. All rights reserved.
// Program.cs

using Microsoft.AutoGen.Core;
using SupportCenter.Agents.QnA;
using SupportCenter.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureSemanticKernel();

builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<QnA>("qna");

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();
