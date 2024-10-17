// Copyright (c) Microsoft. All rights reserved.

namespace SupportCenter.Backend.Hubs;

public interface ISignalRService
{
    Task SendMessageToSpecificClient(string userId, string message, AgentTypes agentType);
}
