# OpenAI Integration in NeuralJourney

## Overview

This document serves as a comprehensive guide detailing the integration of OpenAI's GPT models in NeuralJourney. It covers the fine-tuning process, the structure of the training data, API interactions, and the rationale behind each step.

## Prerequisites

- OpenAI API Key
- .NET SDK
- Familiarity with RESTful APIs

## Significance of End Sequence `###END###`

The end sequence `###END###` is a pivotal element in both the training data and the API requests. It serves as a delimiter to signify the end of the prompt or the generated text. This is crucial for several reasons:

- **Delimiter**: It acts as a boundary marker, helping the model to recognize where the task ends.
- **Model Guidance**: It aids the model in generating more accurate completions by providing a clear endpoint.
- **Data Parsing**: It facilitates the parsing of the model's output, especially useful when separating commands from generated text.

## Two-Step Process: Classification and Parameter/Text Generation

### Rationale

1. **Efficient Token Usage**: The initial classification step minimizes the tokens required for the fine-tuned model, thereby optimizing costs. This is particularly important given OpenAI's token-based pricing.
  
2. **Reduced Model Complexity**: The classification acts as a pre-processing step, simplifying the task for the fine-tuned model. This is beneficial for both computational efficiency and model accuracy.

3. **Contextually Relevant Responses**: The player's original input text is used in the second step to generate a response that mirrors the user's intent. This makes the interaction more engaging and less generic.

### Command Classification with OpenAI

#### Why `max_tokens` is Set to 1

The `max_tokens` parameter is set to 1 because the model is being used for classification. The model only needs to generate a single token that represents the classified command, making it a more efficient use of tokens.

#### Training and Fine-Tuning

The model is trained on a dataset of player commands and their corresponding game-specific actions. Fine-tuning is performed to adapt the base GPT model to this specific classification task.

##### Training Data Format

\`\`\`json
{
  "prompt": "enter the password to reveal the hidden door\n\n###END###\n\n",
  "completion": " unlock"
}
\`\`\`

#### API Interaction

##### Code Snippet

\`\`\`csharp
var request = new CompletionRequest(prompt: $"{inputText}\n\n###END###\n\n", max_tokens: 1, model: _options.CommandClassificationModel);
var completionResult = await _openApi.Completions.CreateCompletionAsync(request);
\`\`\`

### Parameter and Text Generation with OpenAI

#### Training and Fine-Tuning

The model is trained on a dataset that includes both the command and the contextually relevant text.

##### Training Data Format

\`\`\`json
{
  "prompt": "map|study the map\n\n###END###\n\n",
  "completion": " map|you studied the map##END##"
}
\`\`\`

#### API Interaction

##### Code Snippet

\`\`\`csharp
var promptText = $"{context.CommandKey.Identifier}|{previousInput}{context.InputText}\n\n###\n\n";
var completionResult = await _openApi.Completions.CreateCompletionAsync(promptText);
context.CompletionText = completionResult?.Completions.FirstOrDefault()?.Text.TrimStart();
\`\`\`

## Configuration and Customization

### API Configuration

- **API Key**: Store your API key securely to prevent unauthorized access.
- **Rate Limiting**: Be aware of the API rate limits and implement appropriate error-handling mechanisms.

### Fine-Tuning Configuration

- **Training Data**: Ensure your training data is diverse and representative of the tasks you expect the model to perform.
- **Model Parameters**: Choose appropriate model parameters for fine-tuning, such as learning rate, batch size, etc.
