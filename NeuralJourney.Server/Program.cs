using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Core.Commands;
using NeuralJourney.Core.Commands.Admin;
using NeuralJourney.Core.Commands.Players;
using NeuralJourney.Core.Commands.Players.Middleware;
using NeuralJourney.Core.Interfaces.Commands;
using NeuralJourney.Core.Interfaces.Engines;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Models.World;
using NeuralJourney.Core.Options;
using NeuralJourney.Infrastructure.Engines;
using NeuralJourney.Infrastructure.Handlers;
using NeuralJourney.Infrastructure.Services;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.Filters;
using Serilog.Templates;
using Serilog.Templates.Themes;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
using var cts = new CancellationTokenSource();

var services = scope.ServiceProvider;

var engine = services.GetRequiredService<IEngine>();

try
{
    Console.CancelKeyPress += (sender, eventArgs) =>
    {
        eventArgs.Cancel = true;
        cts.Cancel();
    };

    await engine.Run(cts.Token);
}
catch (OperationCanceledException ex)
{
    services.GetRequiredService<ILogger>().Error(ex, ex.Message);
}
catch (Exception ex)
{
    cts.Cancel();
    services.GetRequiredService<ILogger>().Error(ex, ex.Message);
}
finally
{
    cts.Dispose();

    services.GetRequiredService<ILogger>().Information("The server is shutting down");
    await Task.Delay(2000); // Let the message hang for 2 seconds before closing the window
}

static IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", false);
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var options = hostingContext.Configuration.GetRequiredSection("Server:Serilog").Get<SerilogOptions>()
                ?? throw new InvalidOperationException("Serilog configuration is missing");

            var logLevel = Enum.TryParse(options.LogLevel, out LogEventLevel level) ? level : LogEventLevel.Information;
            var logFilePath = options.LogFilePath ?? "Logs/";

            var consoleTemplate = new ExpressionTemplate(options.ConsoleOutputTemplate ?? "{@m}", theme: TemplateTheme.Code);
            var fileTemplate = new ExpressionTemplate(options.FileOutputTemplate ?? "{@m} {x}");

            var nonErrorLogEventCondition = new Func<LogEvent, bool>(e => e.Level != LogEventLevel.Error && e.Level != LogEventLevel.Fatal);
            var errorLogEventCondition = new Func<LogEvent, bool>(e => !nonErrorLogEventCondition(e));

            loggerConfiguration
                    .MinimumLevel.Is(logLevel)
                    .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithFilter(new IgnorePropertyByNameExceptionFilter("HResult", "StackTrace")))
                    .WriteTo.File(path: logFilePath, formatter: fileTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: logLevel)
                    .WriteTo.Console(formatter: consoleTemplate);
        })
        .ConfigureServices((_, services) =>
        {
            // Register Handlers
            services.AddTransient<IInputHandler<TextReader>, ConsoleInputHandler>();
            services.AddTransient<IInputHandler<Player>, PlayerInputHandler>();

            services.AddSingleton<IConnectionHandler, NetworkConnectionHandler>();

            services.AddSingleton<IPlayerHandler, PlayerHandler>();

            // Register Services
            services.AddSingleton<IClockService, ClockService>();
            services.AddTransient<IMessageService, MessageService>();
            services.AddSingleton<IOpenAIService, OpenAIService>();

            services.AddSingleton<IEngine, ServerEngine>();

            // Register Command Execution
            services.AddSingleton<ICommandFactory, CommandFactory>();

            services.AddTransient<ICommandDispatcher, CommandDispatcher>();
            services.AddTransient<ICommandStrategyFactory, CommandStrategyFactory>();
            services.AddTransient<ICommandStrategy, AdminCommandStrategy>();
            services.AddTransient<ICommandStrategy, PlayerCommandStrategy>();

            RegisterCommandMiddleware(services);

            // Configure Options
            var serverConfig = _.Configuration.GetRequiredSection("Server");

            services.Configure<GameOptions>(serverConfig.GetSection("Game"))
                    .Configure<OpenAIOptions>(serverConfig.GetSection("OpenAI"))
                    .Configure<SerilogOptions>(serverConfig.GetSection("Serilog"))
                    .Configure<NetworkOptions>(serverConfig.GetSection("Network"));

            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GameOptions>>().Value);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OpenAIOptions>>().Value);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SerilogOptions>>().Value);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<NetworkOptions>>().Value);
        });
}

static IServiceCollection RegisterCommandMiddleware(IServiceCollection services)
{
    return services
        .AddTransient<ICommandMiddleware, InputValidation>()
        .AddTransient<ICommandMiddleware, CompletionTextRequester>()
        .AddTransient<ICommandMiddleware, CompletionTextParser>()
        .AddTransient<ICommandMiddleware, CommandInstantiator>()
        .AddTransient<ICommandMiddleware, CommandExecutor>()
        .AddTransient<ICommandMiddleware, ResultProcessor>();
}
