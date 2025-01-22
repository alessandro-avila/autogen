// Copyright (c) Microsoft Corporation. All rights reserved.
// Program.cs

using Microsoft.AutoGen.Core;
using SupportCenter.Agents.CustomerInfo;
using SupportCenter.Agents.Discount;
using SupportCenter.Agents.Dispatcher;
using SupportCenter.Agents.Invoice;
using SupportCenter.Agents.QnA;
using SupportCenter.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddAgentWorker(builder.Configuration["AGENT_HOST"]!)
    .AddAgent<Dispatcher>("dispatcher")
    .AddAgent<CustomerInfo>("customerInfo")
    .AddAgent<Discount>("discount")
    .AddAgent<Invoice>("invoice")
    .AddAgent<QnA>("qna");

var app = builder.Build();

Extensions.MapDefaultEndpoints(app);

app.Run();
