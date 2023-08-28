# OpenAI Integration in NeuralJourney

## General Overview
### Fine-tuning and Its Relevance
Fine-tuning is the process of adapting a pre-trained machine learning model to a specific task. In NeuralJourney, OpenAI's GPT models are fine-tuned to perform two main tasks: command classification and parameter extraction, which are further used for generating player responses. Fine-tuning allows the general-purpose GPT models to understand the specific language and context of the game, making them more effective in interpreting player inputs.

### Two-Step Process
The OpenAI integration in NeuralJourney is implemented as a two-step process: 
1. Command Classification: Understands the type of command a player has entered.
2. Text Generation: Extracts any additional parameters and generates player responses.

### Prerequisites
A basic understanding of machine learning, OpenAI's GPT models, and the concept of fine-tuning is required to fully grasp the intricacies of this integration, in addition to the following:
- OpenAI API Key
- .NET SDK
- Familiarity with RESTful APIs

## Implementation
### Command Classification
Command classification is the first step in understanding the player's intent. The OpenAI model, fine-tuned for this specific task, takes the player's input and classifies it into a specific command like 'move', 'attack', or 'defend'.

#### Training Data and Its Structure
The training data for this task is structured in JSON format, with each object containing a `prompt` and a `completion`. For example:

```json
{"prompt":"take a sip of the elixir\\n\\n###\\n\\n","completion":" drink"}
```

The `prompt` is the player's command, followed by an end sequence `\\n\\n###\\n\\n` that serves the purpose of separating the prompt from the completion. Since this model is used for classification the completion only contains a single-token command identifier, which in this case is `drink`.

#### Code Implementation and `max_tokens`
The `OpenAIService.cs` file contains the method `GetCommandClassificationAsync`, which uses the OpenAI SDK to classify the command:

```csharp
public async Task<CommandIdentifierEnum> GetCommandClassificationAsync(string inputText)
{
    var request = new CompletionRequest(prompt: $"{inputText}\\n\\n###\\n\\n", max_tokens: 1, model: _options.CommandClassificationModel);
    var completionResult = await _openApi.Completions.CreateCompletionAsync(request);
    // ... (rest of the code)
}
```

The `max_tokens: 1` parameter is crucial here. It tells the model to return just one token, to make sure that only the classified command identifier is returned.

### Text Generation
After classifying the command, the next step is text generation, which involves parameter extraction and generating player responses.

#### Training Data and Its Structure
The training data for this step is also in JSON format. For example:

```json
{"prompt":"move|flee west\\n\\n###\\n\\n","completion":" move(west)|you flee to the west##END##"}
```

The `prompt` contains the command and possible parameters, followed by an end sequence `###END###`. The `completion` contains the extracted parameters and the text that the player will see.

The end sequence `###END###` serves as a delimiter that helps the model understand where the input ends. This is crucial for both training the model and for runtime operations, as it provides a structured format for the input, except in certain cases like the command classification model that just cuts of the response at 1 token no matter what.

#### Code Implementation
The `SetCommandCompletionTextAsync` method in `OpenAIService.cs` handles this:

```csharp
public async Task<bool> SetCommandCompletionTextAsync(CommandContext context)
{
    // ... (rest of the code)
    var promptText = $"{context.CommandKey.Identifier}|{previousInput}{context.InputText}\n\n###\n\n";
    var completionResult = await _openApi.Completions.CreateCompletionAsync(promptText);
    context.CompletionText = completionResult?.Completions.FirstOrDefault()?.Text.TrimStart();
    // ... (rest of the code)
}
```

## Conclusion
The OpenAI integration in NeuralJourney is a complex but essential feature that adds a layer of intelligence to the game. Through fine-tuning and a well-thought-out two-step process, the game can understand and interact with player inputs in a meaningful way. This document has aimed to provide a comprehensive understanding of this integration, from the fine-tuning process to the code implementation and the role of the end sequence.
