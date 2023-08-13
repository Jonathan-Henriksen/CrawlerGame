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

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
using var cts = new CancellationTokenSource();

var services = scope.ServiceProvider;

var engine = services.GetRequiredService<IEngine>();

try
{
    Console.CancelKeyPress += delegate (object? sender, ConsoleCancelEventArgs e)
    {
        e.Cancel = true;
        engine.Stop();
        cts.Cancel();
    };

    await engine.Run(cts.Token);
}
catch (OperationCanceledException ex)
{
    // Do nothing on expected cancellation. Engine is handling shutdown itself.
}
catch (Exception ex)
{
    cts.Cancel();
    services.GetRequiredService<ILogger>().Error(ex, ex.Message);
}
finally
{
    cts.Dispose();

    services.GetRequiredService<ILogger>().Information("Quitting the game");
    await Task.Delay(3000); // Let the message hang for 3 seconds before closing the window.
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

            var fileTemplate = new ExpressionTemplate(options.FileOutputTemplate ?? "{@m} {x}");

            loggerConfiguration
                .MinimumLevel.Is(logLevel)
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder().WithDefaultDestructurers().WithFilter(new IgnorePropertyByNameExceptionFilter("HResult", "StackTrace")))
                .WriteTo.File(path: logFilePath, formatter: fileTemplate, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: logLevel)
                .WriteTo.Conditional(logEvent => logEvent.Level == LogEventLevel.Information,
                    sink => sink.Sink<ClientConsoleSink>()
            );
        }, preserveStaticLogger: true)
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<IEngine, ClientEngine>();

            services.AddTransient<IMessageService, MessageService>();

            services.AddTransient<IInputHandler<TextReader>, ConsoleInputHandler>();
            services.AddTransient<IInputHandler<NetworkStream>, NetworkInputHandler>();

            services.AddOptions<ClientOptions>().BindConfiguration("Client:Network");
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ClientOptions>>().Value);
        });
}
