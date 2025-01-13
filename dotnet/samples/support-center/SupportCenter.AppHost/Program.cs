// Copyright (c) Microsoft Corporation. All rights reserved.
// Program.cs

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

//TODO: Add Persistent storage for the agents
var orleans = builder.AddOrleans("orleans")
    .WithDevelopmentClustering();

var agentHost = builder.AddProject<Projects.SupportCenter_AgentHost>("agentHost")
    .WithReference(orleans);
var agentHostHttps = agentHost.GetEndpoint("https");

var backend = builder.AddProject<Projects.SupportCenter_Backend>("supportcenter-backend")
    .WithEnvironment("AGENT_HOST", $"{agentHostHttps.Property(EndpointProperty.Url)}")
    .WithEnvironment("OpenAI__Key", builder.Configuration["OpenAI:Key"])
    .WithEnvironment("OpenAI__Endpoint", builder.Configuration["OpenAI:Endpoint"]);

builder.AddProject<Projects.SupportCenter_Agents>("supportcenter-agents")
    .WithEnvironment("AGENT_HOST", $"{agentHostHttps.Property(EndpointProperty.Url)}")
    .WithEnvironment("OpenAI__Key", builder.Configuration["OpenAI:Key"])
    .WithEnvironment("OpenAI__Endpoint", builder.Configuration["OpenAI:Endpoint"]);

builder.AddNpmApp("frontend", "../SupportCenter.Frontend", "dev")
    .WithReference(backend)
    .WithEnvironment("VITE_OAGENT_BASE_URL", backend.GetEndpoint("http"))
    .WithEnvironment("VITE_IS_MOCK_ENABLED", "true")
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
