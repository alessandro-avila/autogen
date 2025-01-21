// Copyright (c) Microsoft Corporation. All rights reserved.
// SemanticKernelHostingExtensions.cs

#pragma warning disable SKEXP0050
using System.Text.Json;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AzureAISearch;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using Microsoft.SemanticKernel.Memory;
using SupportCenter.Shared.AgentsConfigurationFactory;
using SupportCenter.Shared.Options;

namespace SupportCenter.Shared.Extensions;
public static class SemanticKernelHostingExtensions
{
    public static IHostApplicationBuilder ConfigureSemanticKernel(this IHostApplicationBuilder builder)
    {
        // TODO: fix
        //builder.Services.AddTransient(CreateKernel);
        //builder.Services.AddTransient(CreateMemory);

        builder.Services.Configure<OpenAiOptions>(o =>
        {
            o.EmbeddingsEndpoint = o.ChatEndpoint = builder.Configuration["OpenAI:Endpoint"] ?? throw new InvalidOperationException("Ensure that OpenAI:Endpoint is set in configuration");
            o.EmbeddingsApiKey = o.ChatApiKey = builder.Configuration["OpenAI:Key"]!;
            o.EmbeddingsDeploymentOrModelId = "text-embedding-ada-002";
            o.ChatDeploymentOrModelId = "gpt-4o";
        });

        //builder.Services.Configure<OpenAIClientOptions>(o =>
        //{
        //    o.RetryPolicy = new Azure.Core.Pipeline.RetryPolicy(maxRetries: 5, DelayStrategy.CreateExponentialDelayStrategy());
        //});

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });
        return builder;
    }

    public static ISemanticTextMemory CreateMemory(IServiceProvider provider, string agent)
    {
        OpenAiOptions openAiConfig = provider.GetService<IOptions<OpenAiOptions>>()?.Value ?? new OpenAiOptions();
        openAiConfig.ValidateRequiredProperties();

        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .SetMinimumLevel(LogLevel.Debug)
                .AddConsole()
            .AddDebug();
        });
        if (agent == "Invoice")
        {
            var aiSearchConfig = provider.GetService<IOptions<AiSearchOptions>>()?.Value ?? new AiSearchOptions();
            aiSearchConfig.ValidateRequiredProperties();

            var memoryBuilder = new MemoryBuilder();
            return memoryBuilder.WithLoggerFactory(loggerFactory)
                            .WithMemoryStore(new AzureAISearchMemoryStore(aiSearchConfig.SearchEndpoint!, aiSearchConfig.SearchKey!))
                            // IMPROVE: maybe with a dependency injection container:
                            // https://learn.microsoft.com/en-us/semantic-kernel/concepts/vector-store-connectors/embedding-generation?pivots=programming-language-csharp#constructing-an-embedding-generator
                            .WithTextEmbeddingGeneration(new AzureOpenAITextEmbeddingGenerationService(openAiConfig.EmbeddingsDeploymentOrModelId, openAiConfig.EmbeddingsEndpoint, openAiConfig.EmbeddingsApiKey))
                            .Build();
        }
        else
        {
            var qdrantConfig = provider.GetService<IOptions<QdrantOptions>>()?.Value ?? new QdrantOptions();
            qdrantConfig.ValidateRequiredProperties();

            return new MemoryBuilder().WithLoggerFactory(loggerFactory)
                         .WithQdrantMemoryStore(qdrantConfig.Endpoint, qdrantConfig.VectorSize)
                         .WithTextEmbeddingGeneration(new AzureOpenAITextEmbeddingGenerationService(openAiConfig.EmbeddingsDeploymentOrModelId, openAiConfig.EmbeddingsEndpoint, openAiConfig.EmbeddingsApiKey))
                         .Build();
        }
    }

    public static Kernel CreateKernel(IServiceProvider provider, string agent)
    {
        var openAiConfig = provider.GetService<IOptions<OpenAiOptions>>()?.Value ?? new OpenAiOptions();

        var agentConfiguration = AgentConfiguration.GetAgentConfiguration(agent);
        agentConfiguration.ConfigureOpenAI(openAiConfig);

        var clientOptions = new AzureOpenAIClientOptions()
        {

        };
        var builder = Kernel.CreateBuilder();
        builder.Services.AddLogging(c => c.AddConsole().AddDebug().SetMinimumLevel(LogLevel.Debug));

        // Chat
        var azureOpenAIClientForChat = new AzureOpenAIClient(new Uri(openAiConfig.ChatEndpoint), new AzureKeyCredential(openAiConfig.ChatApiKey), clientOptions);
        builder.Services.AddAzureOpenAIChatCompletion(openAiConfig.ChatDeploymentOrModelId, azureOpenAIClientForChat);

        // Embeddings
        var azureOpenAIClientForEmbedding = new AzureOpenAIClient(new Uri(openAiConfig.EmbeddingsEndpoint), new AzureKeyCredential(openAiConfig.EmbeddingsApiKey), clientOptions);

        builder.Services.AddAzureOpenAITextEmbeddingGeneration(openAiConfig.EmbeddingsDeploymentOrModelId, azureOpenAIClientForEmbedding);

        builder.Services.ConfigureHttpClientDefaults(c =>
        {
            c.AddStandardResilienceHandler().Configure(o =>
            {
                o.Retry.MaxRetryAttempts = 5;
                o.Retry.BackoffType = Polly.DelayBackoffType.Exponential;
            });
        });

        var kernel = builder.Build();
        agentConfiguration.ConfigureKernel(kernel, provider);

        return kernel;
    }
}
#pragma warning restore SKEXP0050
