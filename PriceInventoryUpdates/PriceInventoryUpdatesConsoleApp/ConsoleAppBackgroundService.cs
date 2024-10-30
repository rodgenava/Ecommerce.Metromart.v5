using Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using static Application.Enum;

namespace PriceInventoryUpdatesConsoleApp
{
    internal class ConsoleAppBackgroundService : BackgroundService
    {
        private readonly string[] _args;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public ConsoleAppBackgroundService(string[] args, IServiceScopeFactory serviceScopeFactory, IHostApplicationLifetime hostApplicationLifetime)
        {
            _args = args;
            _scopeFactory = serviceScopeFactory;
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await DoWorkAsync(stoppingToken);
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            IServiceScope scope = _scopeFactory.CreateScope();

            IServiceProvider serviceProvider = scope.ServiceProvider;

            var logger = serviceProvider.GetRequiredService<ILogger<ConsoleAppBackgroundService>>();

            try
            {
                ISendUpdatesServiceFactory sendUpdatesServiceFactory = serviceProvider.GetRequiredService<ISendUpdatesServiceFactory>();
                ISendPandaMartUpdatesServiceFactory sendPandaUpdatesServiceFactory = serviceProvider.GetRequiredService<ISendPandaMartUpdatesServiceFactory>();

                int argumentsCount = _args.Length;
                bool withArgs = argumentsCount > 0;
                int argumentsIterator = 0;
                bool loop = true;

                do
                {
                    Console.WriteLine("push-full-updates: Push full updates to MertroMart.\npush-consignment-updates: Push hourly updates to MertroMart.\nX - Exit");

                    Console.Write("Input: ");

                    string option = withArgs ? _args[argumentsIterator].Trim().ToUpper() : (Console.ReadLine() ?? string.Empty).Trim().ToUpper();

                    Console.WriteLine();

                    logger.LogTrace("Evaluating option \"{option}\"...", option);

                    var stopwatch = Stopwatch.StartNew();

                    switch (option)
                    {
                        case "PUSH-FULL-UPDATES":
                            logger.LogTrace("Sending full updates to MetroMart...");
                            logger.LogTrace("Sending full updates to PandaMart...");
                            await sendUpdatesServiceFactory.CreateSendUpdatesService(updateType: UpdateType.Full).RunAsync(cancellationToken: stoppingToken);
                            await sendPandaUpdatesServiceFactory.CreateSendPandaMartUpdatesService(updateType: UpdateTypepanda.Full).RunAsync(cancellationToken: stoppingToken);
                            break;
                        case "PUSH-CONSIGNMENT-UPDATES":
                            logger.LogTrace("Sending consignment updates to MetroMart...");
                            logger.LogTrace("Sending consignment updates to PandaMart...");
                            await sendUpdatesServiceFactory.CreateSendUpdatesService(updateType: UpdateType.ConsignmentOnly).RunAsync(cancellationToken: stoppingToken);
                            await sendPandaUpdatesServiceFactory.CreateSendPandaMartUpdatesService(updateType: UpdateTypepanda.ConsignmentOnly).RunAsync(cancellationToken: stoppingToken);
                            break;
                        case "X":
                            loop = false;
                            break;
                        default:
                            Console.WriteLine("Unrecognized option.");
                            break;
                    }

                    stopwatch.Stop();

                    Console.WriteLine();

                    logger.LogTrace("Operation completed in {EllapsedTime}s", (stopwatch.ElapsedMilliseconds / 1000m).ToString("#,##0.###"));
                }
                while ((!withArgs && loop) || ++argumentsIterator <= argumentsCount - 1);
            }
            catch(Exception ex) when (ex is not TaskCanceledException)
            {
                logger.LogError(
                    exception: ex,
                    message: "A fatal error occurred ({exceptionType}).",
                    ex.GetType().Name);

            }
            finally
            {
                scope.Dispose();
                _hostApplicationLifetime.StopApplication();
            }

        }
    }
}
