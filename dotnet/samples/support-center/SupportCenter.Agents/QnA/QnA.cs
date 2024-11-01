using SupportCenter.Shared;
using Microsoft.AutoGen.Abstractions;
using Microsoft.AutoGen.Agents;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace SupportCenter.Agents.QnA;

[TopicSubscription("default")]
public class QnA(IAgentContext context, Kernel kernel, ISemanticTextMemory memory, [FromKeyedServices("EventTypes")] EventTypes typeRegistry, ILogger<QnA> logger)
    : SKAiAgent<QnAState>(context, memory, kernel, typeRegistry),
    IHandle<QnARequested>
{
    public async Task Handle(QnARequested item)
    {
        logger.LogInformation($"[{nameof(QnA)}] Event {nameof(QnARequested)}. Text: {{Text}}", item.Message);

        var context = new KernelArguments { ["input"] = AppendChatHistory(item.Message) };
        var answer = await CallFunction(QnAPrompts.QnAGenericPrompt, context);
        if (answer.Contains("NOTFORME", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        await SendQnAResponse(answer, item.UserId);
    }

    private async Task SendQnAResponse(string message, string userId)
    {
        var qnaresponse = new QnAResponse
        {
            Message = message,
            UserId = userId
        }.ToCloudEvent(AgentId.Key);

        await PublishEvent(qnaresponse);
    }
}
