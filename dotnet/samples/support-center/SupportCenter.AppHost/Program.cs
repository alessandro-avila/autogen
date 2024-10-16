var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

//TODO: Add Persistent storage for the agents
var orleans = builder.AddOrleans("orleans")
    .WithDevelopmentClustering();

builder.AddProject<Projects.SupportCenter_Backend>("supportcenter-backend");
builder.AddProject<Projects.SupportCenter_Agents>("supportcenter-agents");
builder.AddProject<Projects.SupportCenter_AgentHost>("agentHost")
    .WithReference(orleans);

builder.Build().Run();
