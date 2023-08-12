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
            loggerConfiguration
                        .Enrich.WithExceptionDetails()
                        .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}")
                        .WriteTo.File(path: "../../../../Logs/server-.txt", restrictedToMinimumLevel: LogEventLevel.Error, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day);
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
            services.AddOptions<GameOptions>().BindConfiguration("Game");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<GameOptions>>().Value);

            services.AddOptions<OpenAIOptions>().BindConfiguration("OpenAI");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<OpenAIOptions>>().Value);

            services.AddOptions<ServerOptions>().BindConfiguration("Server");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ServerOptions>>().Value);
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