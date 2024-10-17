// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.SignalR;

namespace SupportCenter.Backend.Hubs;

public class SignalRService(IHubContext<SupportCenterHub> hubContext) : ISignalRService
{
    public async Task SendMessageToSpecificClient(string userId, string message, AgentTypes agentType)
    {
        var connectionId = SignalRConnectionsDB.ConnectionIdByUser[userId];
        var frontEndMessage = new FrontEndMessage()
        {
            UserId = userId,
            Message = message,
            Agent = agentType.ToString()
        };
        await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", frontEndMessage);
    }
}
