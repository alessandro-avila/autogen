using SupportCenter.Shared;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel;
using Microsoft.AutoGen.Abstractions;
using Microsoft.AutoGen.Agents;
using SupportCenter.Backend.Hubs;
using Google.Protobuf.WellKnownTypes;

namespace SupportCenter.Backend.Agents;

[TopicSubscription("default")]
public class SignalRAgent(IAgentContext context, Kernel kernel, ISemanticTextMemory memory, [FromKeyedServices("EventTypes")] EventTypes typeRegistry, ISignalRService signalRClient)
    : SKAiAgent<Empty>(context, memory, kernel, typeRegistry),
    IHandle<QnAResponse>
{
    public async Task Handle(QnAResponse item)
    {
        string? userId = item.UserId;
        string? message = item.Message;

        await signalRClient.SendMessageToSpecificClient(userId, message, Hubs.AgentTypes.QnA);
    }
}
