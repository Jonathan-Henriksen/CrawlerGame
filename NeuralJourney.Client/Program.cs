using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Client;
using NeuralJourney.Core.Interfaces.Engines;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Options;
using NeuralJourney.Infrastructure.Engines;
using NeuralJourney.Infrastructure.Handlers;
using NeuralJourney.Infrastructure.Services;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions.Core;
using Serilog.Exceptions;
using System.Net.Sockets;
using Serilog.Templates;
using Serilog.Exceptions.Filters;
using Serilog.Formatting.Compact;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
using var cts = new CancellationTokenSource();

var services = scope.ServiceProvider;

var logger = services.GetRequiredService<ILogger>();

try
{
    var engine = services.GetRequiredService<IEngine>();
    Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e) // Gracefully shuut down on request by user
    {
        logger.Debug("Client initializd shutdown");

        e.Cancel = true;
        engine.Stop();
        cts.Cancel();
    };

    await engine.Run(cts.Token);
}
catch (OperationCanceledException)
{
    // Do nothing on expected cancellation. Engine is handling shutdown itself
}
catch (Exception ex)
{
    cts.Cancel();
    logger.Fatal(ex, ex.Message); // Unexpected error that crashed the application
}
finally
{
    cts.Dispose();

    logger.Information("Shutting down");
    await Task.Delay(2000); // Let the message hang for 2 seconds before closing the window.
}

// Configure Application
static IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", false);
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            var options = hostingContext.Configuration.GetRequiredSection("Client:Serilog").Get<SerilogOptions>()
                ?? throw new InvalidOperationException("Serilog configuration is missing");

            var logLevel = Enum.TryParse(options.LogLevel, out LogEventLevel level) ? level : LogEventLevel.Information;
            var logFilePath = options.LogFilePath ?? "Logs/";

            loggerConfiguration
                .MinimumLevel.Is(logLevel)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithFilter(new IgnorePropertyByNameExceptionFilter("HResult", "StackTrace")))
                .Enrich.WithProperty("Application", "Client")
                .WriteTo.Seq(serverUrl: "http://localhost:5341/", restrictedToMinimumLevel: logLevel)
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Information,
                    sink => sink.Sink<ClientConsoleSink>()
            );
        })
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IEngine, ClientEngine>();

            services.AddTransient<IMessageService, MessageService>();

            services.AddTransient<IInputHandler<TextReader>, ConsoleInputHandler>();
            services.AddTransient<IInputHandler<TcpClient>, NetworkInputHandler>();

            services.AddOptions<ClientOptions>().BindConfiguration("Client:Network");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ClientOptions>>().Value);
        });
}
