# Logging with Serilog and Seq

## Overview
Structured logging is an indispensable tool in modern software development, particularly in complex, interactive applications like NeuralJourney. Unlike traditional logging, which often only captures text messages, structured logging captures key-value pairs that provide a rich context for each event. In NeuralJourney, we leverage the power of Serilog and Seq to implement structured logging. This approach significantly enhances our debugging, monitoring, and analytics capabilities, making it easier to understand the game's behavior and troubleshoot issues.

## Configuration
### Setting Up the Logging Infrastructure
Serilog provides a highly configurable logging pipeline, which is crucial for tailoring the logging experience to meet specific needs. You can control various aspects, such as the minimum log level, and enrich log events with additional data. Serilog's concept of "sinks" allows you to direct these enriched log events to multiple destinations. In our case, one of the primary sinks is Seq, a specialized log server designed to handle structured log data. Seq not only stores the logs but also provides powerful querying and visualization features.

Here's a sample configuration in the `Startup.cs` file that sets up Serilog and Seq:

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.Console()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();
```

This configuration sets the minimum log level to Debug and directs log events to both the console for immediate feedback and a Seq server for persistent storage and advanced querying.

## Logging in Code
### Templating and Structured Data
Serilog introduces the concept of message templates, a more powerful alternative to traditional string formatting. Message templates capture structured data directly within log statements. This is particularly useful for capturing dynamic values within the application's flow, such as player actions or game states. For example:

```csharp
Log.Information("Player {PlayerName} moved {Direction}", playerName, direction);
```

Here, `{PlayerName}` and `{Direction}` are placeholders that get replaced by the `playerName` and `direction` variables, respectively. These properties are also captured and can be queried later in Seq.

### Context Enrichment in Logs
Serilog offers two primary mechanisms for enriching logs with additional context: `LogContext.PushProperty` and `Logger.ForContext`.

#### Scope-based Context with LogContext.PushProperty
`LogContext.PushProperty` is a powerful feature that allows you to add properties that get attached to all log events within a certain scope. This is particularly useful for adding context that spans multiple operations or stages in the game. 

```csharp
using (LogContext.PushProperty("Player", player))
{
    Log.Information("Player entered the game");
    Log.Information("Player moved north");
}
```

#### Targeted Context with Logger.ForContext
`Logger.ForContext` allows you to create a logger instance that automatically includes specified properties in all log events. This is useful when you need to add context that is specific to a certain component or operation within the game.

```csharp
Log.ForContext("CommandContext", commandContext).Information("Command executed");

// Or

var logger = Log.ForContext("CommandContext", commandContext);
logger.Information("Command executed");
```

## Advanced Log Filtering and Analysis with Seq
Structured logging fundamentally changes how you can interact with your logs. It turns them from mere text outputs into a rich dataset with queryable attributes. This is where Seq comes into play as a powerful log server designed to work seamlessly with structured logging frameworks like Serilog. Seq provides an intuitive user interface and a query language that allows you to filter, analyze, and visualize your logs in real-time.

When you need to isolate logs for debugging or analytics, the structured data captured in each log event becomes invaluable. For example, if you're investigating an issue related to a specific player's actions, you can use Seq to query all logs where the `PlayerId` matches that particular player. Similarly, if you're analyzing how often a certain type of command is executed, Seq allows you to filter logs based on the `CommandType` attribute.

Here's a simple Seq query example that filters logs related to a specific player:

```sql
select * from stream where PlayerId = 'player123'
```

And another that counts the occurrences of each command type:

```sql
select CommandType, count(*) from stream group by CommandType
```

Seq not only makes it easy to find the logs you are interested in but also allows you to create dashboards, set up alerts, and even integrate with other tools, making it a powerful tool for in-depth analysis and monitoring.

## Conclusion
Logging is more than just an afterthought; it's a critical component of any complex application. In NeuralJourney, the integration of Serilog and Seq provides a robust, flexible, and highly configurable logging system. It captures rich, structured data that can be queried effectively, making it an invaluable tool for development, debugging, and monitoring. This document aims to serve as both an educational guide and a detailed explanation of how structured logging is implemented in NeuralJourney, providing developers with the knowledge and tools they need to understand and leverage the power of structured logging.
