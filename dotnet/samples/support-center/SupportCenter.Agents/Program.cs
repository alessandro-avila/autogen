// Copyright (c) Microsoft Corporation. All rights reserved.
// Program.cs

using Microsoft.AutoGen.Core;
using SupportCenter.Agents.QnA;
using SupportCenter.ServiceDefaults;
using SupportCenter.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.ConfigureSemanticKernel();

builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<QnA>("qna");

var app = builder.Build();

SupportCenter.ServiceDefaults.Extensions.MapDefaultEndpoints(app);

app.Run();
