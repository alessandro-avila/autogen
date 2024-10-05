---
myst:
  html_meta:
    "description lang=en": |
      User Guide for AgentChat, a high-level api for AutoGen
---

# AgentChat

AgentChat is a high-level package for building multi-agent applications built on top of the `autogen-core` package. For beginner users, AgentChat is the recommended starting point. For advanced users, `autogen-core` provides more flexibility and control over the underlying components.

AgentChat aims to provide intuitive defaults, such as **Agents** with preset behaviors and **Teams** with predefined communication protocols, to simplify building multi-agent applications.

## Agents

Agents provide presets for how an agent might respond to received messages. The following Agents are currently supported:

- `CodingAssistantAgent` - Generates responses using an LLM on receipt of a message
- `CodeExecutionAgent` - Extracts and executes code snippets found in received messages and returns the output
- `ToolUseAssistantAgent` - Responds with tool call messages based on received messages and a list of tool schemas provided at initialization

## Teams

Teams define how groups of agents communicate to address tasks. The following Teams are currently supported:

- `RoundRobinGroupChat` - Agents take turns sending messages (in a round robin fashion) until a termination condition is met

```{toctree}
:caption: Getting Started
:maxdepth: 2
:hidden:

quickstart
guides/tool_use

```

```{toctree}
:caption: Examples
:maxdepth: 3
:hidden:

examples/index
```