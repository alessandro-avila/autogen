// Copyright (c) Microsoft. All rights reserved.

using global::SupportCenter.Shared;
using Microsoft.AutoGen.Abstractions;
using Microsoft.AutoGen.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace SupportCenter.Agents.Discount;
[TopicSubscription("default")]
public class Discount(IAgentContext context, Kernel kernel, ISemanticTextMemory memory, [FromKeyedServices("EventTypes")] EventTypes typeRegistry, ILogger<Discount> logger)
    : SKAiAgent<DiscountState>(context, memory, kernel, typeRegistry),
    IHandle<UserNewConversation>
{
    public async Task Handle(UserNewConversation item)
    {
        logger.LogInformation($"[{nameof(Discount)}] Event {nameof(UserNewConversation)}");
        // The user started a new conversation.
        _state.History.Clear();
    }
}
