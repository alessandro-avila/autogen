// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.SignalR;

namespace SupportCenter.Backend.Hubs;

public class SignalRService(IHubContext<SupportCenterHub> hubContext) : ISignalRService
{
    public async Task SendMessageToSpecificClient(string userId, string message, AgentTypes agentType)
    {
        var connectionId = SignalRConnectionsDB.GetConversationId(userId) ?? throw new Exception("ConnectionId not found");
        var frontEndMessage = new FrontEndMessage()
        {
            Id = Guid.NewGuid().ToString(),
            ConversationId = connectionId,
            UserId = userId,
            Message = message,
            Sender = agentType.ToString()
        };
        await hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", frontEndMessage).ConfigureAwait(false);
    }
}
