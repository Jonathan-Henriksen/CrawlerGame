using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Core.Interfaces.Engines;
using NeuralJourney.Core.Interfaces.Handlers;
using NeuralJourney.Core.Interfaces.Services;
using NeuralJourney.Core.Options;
using NeuralJourney.Infrastructure.Engines;
using NeuralJourney.Infrastructure.Handlers;
using NeuralJourney.Infrastructure.Services;
using Serilog;
using Serilog.Events;
using System.Net.Sockets;

namespace NeuralJourney.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
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
                // Do nothing on intended cancel
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
                    loggerConfiguration
                                .WriteTo.Sink<ClientConsoleSink>(restrictedToMinimumLevel: LogEventLevel.Information)
                                .WriteTo.File(path: "../../../../Logs/client-.txt", restrictedToMinimumLevel: LogEventLevel.Error, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}", rollingInterval: RollingInterval.Day);
                })
                .ConfigureServices((_, services) =>
                {
                    services.AddSingleton<IEngine, ClientEngine>();

                    services.AddTransient<IMessageService, MessageService>();

                    services.AddTransient<IInputHandler<TextReader>, ConsoleInputHandler>();
                    services.AddTransient<IInputHandler<NetworkStream>, NetworkInputHandler>();

                    services.AddOptions<ClientOptions>().BindConfiguration("Client");
                    services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<ClientOptions>>().Value);
                });
        }

    }
}
