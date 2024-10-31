// Copyright (c) Microsoft. All rights reserved.

using SupportCenter.Backend.Hubs;
using SupportCenter.Shared;

namespace SupportCenter.Agents.Extensions;

public static class AgentExtensions
{
    public static SupportCenterData GetAgentData(this CustomerInfoRequested item)
    {
        var userId = item.UserId;
        var userMessage = item.Message;

        var conversationId = SignalRConnectionsDB.GetConversationId(userId) ?? item.ConversationId;
        var id = $"{userId}/{conversationId}";

        return new SupportCenterData(id, userId, userMessage);
    }

    public record SupportCenterData(string Id, string UserId, string UserMessage);
}
