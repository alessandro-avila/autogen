// Copyright (c) Microsoft. All rights reserved.

using global::SupportCenter.Shared;
using Microsoft.AutoGen.Abstractions;
using Microsoft.AutoGen.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace SupportCenter.Agents.Invoice;
[TopicSubscription("default")]
public class Invoice(IAgentContext context, Kernel kernel, ISemanticTextMemory memory, [FromKeyedServices("EventTypes")] EventTypes typeRegistry, ILogger<Invoice> logger)
    : SKAiAgent<InvoiceState>(context, memory, kernel, typeRegistry),
    IHandle<InvoiceRequested>
{
    public async Task Handle(InvoiceRequested item)
    {
        var userId = item.UserId;
        var message = item.Message;

        logger.LogInformation("[{Agent}]:[{EventType}]:[{EventData}]", nameof(Invoice), typeof(InvoiceRequested), item);

        var notif = new InvoiceNotification
        {
            UserId = userId,
            Message = "Please wait while I look up the details for invoice..."
        };
        await PublishEvent(notif.ToCloudEvent(AgentId.ToString())).ConfigureAwait(false);

        var querycontext = new KernelArguments { ["input"] = AppendChatHistory(message) };
        var instruction = "Consider the following knowledge:!invoices!";
        var enhancedContext = await AddKnowledge(instruction, "invoices", querycontext).ConfigureAwait(false);
        var answer = await CallFunction(InvoicePrompts.InvoiceRequest, enhancedContext).ConfigureAwait(false);

        var response = new InvoiceResponse
        {
            UserId = userId,
            Message = answer
        };
        await PublishEvent(response.ToCloudEvent(AgentId.ToString())).ConfigureAwait(false);
    }
}
