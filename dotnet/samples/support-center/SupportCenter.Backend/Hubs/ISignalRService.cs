// Copyright (c) Microsoft Corporation. All rights reserved.
// ISignalRService.cs

namespace SupportCenter.Backend.Hubs;

public interface ISignalRService
{
    Task SendMessageToSpecificClient(string userId, string message, AgentTypes agentType);
}
