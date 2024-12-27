// Copyright (c) Microsoft Corporation. All rights reserved.
// AgentExtensions.cs

using SupportCenter.Backend.Hubs;
using SupportCenter.Shared;

namespace SupportCenter.Agents.Extensions;

public static class AgentExtensions
{
    public static (string id, string userId, string userMessage) GetAgentData(this CustomerInfoRequested item)
    {
        var userId = item.UserId;
        var userMessage = item.Message;

        var conversationId = SignalRConnectionsDB.GetConversationId(userId) ?? item.ConversationId;
        var id = $"{userId}/{conversationId}";

        return (id, userId, userMessage);
    }
}
