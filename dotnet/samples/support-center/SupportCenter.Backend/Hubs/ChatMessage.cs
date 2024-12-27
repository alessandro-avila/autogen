// Copyright (c) Microsoft Corporation. All rights reserved.
// ChatMessage.cs

namespace SupportCenter.Backend.Hubs;

public class ChatMessage
{
    public string? Id { get; set; }
    public string? ConversationId { get; set; }
    public string? UserId { get; set; }
    public string? Text { get; set; }
    public string? Sender { get; set; }
}
