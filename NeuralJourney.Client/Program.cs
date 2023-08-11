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
using System.Net.Sockets;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
using var cts = new CancellationTokenSource();

var services = scope.ServiceProvider;

try
{
    using var engine = services.GetRequiredService<IEngine>();
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

IHostBuilder CreateHostBuilder(string[] strings)
{
    return Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            config.AddJsonFile("appsettings.json", false);
        })
        .UseSerilog((hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
            loggerConfiguration.WriteTo.Sink(new ClientConsoleSink());
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