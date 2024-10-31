// Copyright (c) Microsoft. All rights reserved.

using global::SupportCenter.Shared;
using Microsoft.AutoGen.Abstractions;
using Microsoft.AutoGen.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Planning;
using SupportCenter.Agents.Extensions;

namespace SupportCenter.Agents.CustomerInfo;
[TopicSubscription("default")]
public class CustomerInfo(IAgentContext context, Kernel kernel, ISemanticTextMemory memory, [FromKeyedServices("EventTypes")] EventTypes typeRegistry, ILogger<CustomerInfo> logger)
    : SKAiAgent<CustomerInfoState>(context, memory, kernel, typeRegistry),
    IHandle<CustomerInfoRequested>,
    IHandle<UserNewConversation>
{
    public async Task Handle(CustomerInfoRequested item)
    {
        var scs = item.GetAgentData();
        var userId = scs.UserId;
        var message = scs.UserMessage;
        var id = scs.Id;

        logger.LogInformation("[{Agent}]:[{EventType}]:[{EventData}]", nameof(CustomerInfo), typeof(CustomerInfoRequested), item);

        var notif = new CustomerInfoNotification
        {
            UserId = userId,
            Message = "I'm working on the user's request..."
        };
        await PublishEvent(notif.ToCloudEvent(AgentId.ToString())).ConfigureAwait(false);

        // Get the customer info via the planners.
        var prompt = CustomerInfoPrompts.GetCustomerInfo
            .Replace("{{$userId}}", userId)
            .Replace("{{$userMessage}}", message)
            .Replace("{{$history}}", AppendChatHistory(message));

#pragma warning disable SKEXP0060 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        // FunctionCallingStepwisePlanner
        var planner = new FunctionCallingStepwisePlanner(new FunctionCallingStepwisePlannerOptions()
        {
            MaxIterations = 10,
        });
        var result = await planner.ExecuteAsync(_kernel, prompt).ConfigureAwait(false);
        logger.LogInformation("[{Agent}]:[{EventType}]:[{EventData}]", nameof(CustomerInfo), typeof(CustomerInfoRequested), result.FinalAnswer);

        var response = new CustomerInfoResponse
        {
            UserId = userId,
            Message = result.FinalAnswer
        };
        await PublishEvent(response.ToCloudEvent(AgentId.ToString())).ConfigureAwait(false);
    }

    public async Task Handle(UserNewConversation item)
    {
        // The user started a new conversation.
        _state.History.Clear();
    }
}
