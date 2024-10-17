namespace SupportCenter.Backend.Hubs;

public interface ISupportCenterHub
{
    public Task ConnectToAgent(string userId);

    public Task ChatMessage(FrontEndMessage frontEndMessage);

    public Task SendMessageToSpecificClient(string userId, string message);
}
