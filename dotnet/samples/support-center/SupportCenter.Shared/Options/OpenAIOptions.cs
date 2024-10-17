using System.ComponentModel.DataAnnotations;

namespace SupportCenter.Shared.Options;

public class OpenAIOptions
{
    // Embeddings
    [Required]
    public required string EmbeddingsEndpoint { get; set; }
    [Required]
    public required string EmbeddingsApiKey { get; set; }
    [Required]
    public required string EmbeddingsDeploymentOrModelId { get; set; }

    // Chat
    [Required]
    public required string ChatEndpoint { get; set; }
    [Required]
    public required string ChatApiKey { get; set; }
    [Required]
    public required string ChatDeploymentOrModelId { get; set; }
}
