using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using NeuralJourney.Client;
using NeuralJourney.Logic.Engines;
using NeuralJourney.Logic.Handlers;
using NeuralJourney.Logic.Options;
using NeuralJourney.Logic.Services;
using Serilog;
using System.Net.Sockets;

using var host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

var cts = new CancellationTokenSource();

try
{
    var client = await services.GetRequiredService<IEngine>().Init(cts.Token);
    await client.Run(cts.Token);
}
catch (OperationCanceledException)
{
    cts.Dispose();
}
catch (Exception ex)
{
    cts.Cancel();
    cts.Dispose();

    var logger = services.GetRequiredService<ILogger>();
    logger.Error(ex, ex.Message);
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