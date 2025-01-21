// Copyright (c) Microsoft Corporation. All rights reserved.
// SignalRAgentConfiguration.cs

using Microsoft.SemanticKernel;
using SupportCenter.Shared.Options;

namespace SupportCenter.Shared.AgentsConfigurationFactory;

internal class SignalRAgentConfiguration : IAgentConfiguration
{
    public void ConfigureOpenAI(OpenAiOptions options)
    {
    }

    public void ConfigureKernel(Kernel kernel, IServiceProvider serviceProvider)
    {
    }
}
