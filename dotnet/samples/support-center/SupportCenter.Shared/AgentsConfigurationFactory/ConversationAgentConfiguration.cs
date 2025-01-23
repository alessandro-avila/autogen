// Copyright (c) Microsoft Corporation. All rights reserved.
// ConversationAgentConfiguration.cs

using Microsoft.SemanticKernel;
using SupportCenter.Shared.Options;

namespace SupportCenter.Shared.AgentsConfigurationFactory;

public class ConversationAgentConfiguration : IAgentConfiguration
{
    public void ConfigureOpenAI(OpenAIOptions options)
    {
        options.ChatDeploymentOrModelId = options.ConversationDeploymentOrModelId ?? options.ChatDeploymentOrModelId;
    }

    public void ConfigureKernel(Kernel kernel, IServiceProvider serviceProvider)
    {
    }
}