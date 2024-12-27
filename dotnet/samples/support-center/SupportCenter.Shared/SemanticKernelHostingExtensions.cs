// Copyright (c) Microsoft Corporation. All rights reserved.
// SemanticKernelHostingExtensions.cs

#pragma warning disable SKEXP0050
using System.Text.Json;
using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using OpenAI;
using SupportCenter.Options;
using SupportCenter.Shared.Options;

namespace SupportCenter.Shared;
public static class SemanticKernelHostingExtensions
{
    public static IHostApplicationBuilder ConfigureSemanticKernel(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient(CreateKernel);
        builder.Services.AddTransient(CreateMemory);

        builder.Services.Configure<OpenAIOptions>(o =>
        {
            o.EmbeddingsEndpoint = o.ChatEndpoint = builder.Configuration["OpenAI:Endpoint"] ?? throw new InvalidOperationException("Ensure that OpenAI:Endpoint is set in configuration");
            o.EmbeddingsApiKey = o.ChatApiKey = builder.Configuration["OpenAI:Key"]!;
            o.EmbeddingsDeploymentOrModelId = "text-embedding-ada-002";
            o.ChatDeploymentOrModelId = "gpt-4o";
        });              

        /*builder.Services.Configure<OpenAIClientOptions>(o =>
        {
            o.Retry.NetworkTimeout = TimeSpan.FromMinutes(5);
        });*/

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        return builder;
    }

    public static ISemanticTextMemory CreateMemory(IServiceProvider provider)
    {
        AiSearchOptions aiSearchConfig = provider.GetRequiredService<IOptions<AiSearchOptions>>().Value;
        if (string.IsNullOrEmpty(aiSearchConfig.SearchEndpoint) || string.IsNullOrEmpty(aiSearchConfig.SearchKey))
        {
            throw new InvalidOperationException("Ensure that AiSearch:SearchEndpoint and AiSearch:SearchKey are set in configuration");
        }
        var openAiConfig = provider.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
        var memoryBuilder = new MemoryBuilder();
        return memoryBuilder.WithLoggerFactory(loggerFactory)
                     .WithMemoryStore(new AzureAISearchMemoryStore(aiSearchConfig.SearchEndpoint, aiSearchConfig.SearchKey))
                     .WithOpenAITextEmbeddingGeneration(openAiConfig.EmbeddingsDeploymentOrModelId, openAiConfig.EmbeddingsEndpoint, openAiConfig.EmbeddingsApiKey)
                     .Build();
    }

    public static Kernel CreateKernel(IServiceProvider provider)
    {
        OpenAIOptions openAiConfig = provider.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        var builder = Kernel.CreateBuilder();
        // Chat
        if (openAiConfig.ChatEndpoint.Contains(".azure", StringComparison.OrdinalIgnoreCase))
        {
            var openAIClient = new AzureOpenAIClient(new Uri(openAiConfig.ChatEndpoint), new System.ClientModel.ApiKeyCredential(openAiConfig.ChatApiKey));
            builder.Services.AddAzureOpenAIChatCompletion(openAiConfig.ChatDeploymentOrModelId, openAIClient);
        }
        else
        {
            var openAIClient = new OpenAIClient(openAiConfig.ChatApiKey);
            builder.Services.AddOpenAIChatCompletion(openAiConfig.ChatDeploymentOrModelId, openAIClient);
        }
        /*

        // Embeddings
        if (openAiConfig.EmbeddingsEndpoint.Contains(".azure", StringComparison.OrdinalIgnoreCase))
        {
            var openAIClient = new OpenAIClient(new Uri(openAiConfig.EmbeddingsEndpoint), new System.ClientModel.ApiKeyCredential(openAiConfig.EmbeddingsApiKey));
            builder.Services.AddAzureOpenAITextEmbeddingGeneration(openAiConfig.EmbeddingsDeploymentOrModelId, openAIClient);
        }
        else
        {
            var openAIClient = new OpenAIClient(openAiConfig.EmbeddingsApiKey);
            builder.Services.AddOpenAITextEmbeddingGeneration(openAiConfig.EmbeddingsDeploymentOrModelId, openAIClient);
        }
        */

        return builder.Build();
    }
}
#pragma warning restore SKEXP0050
