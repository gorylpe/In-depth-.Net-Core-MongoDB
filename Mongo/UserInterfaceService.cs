using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Mongo
{
    public class UserInterfaceService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public UserInterfaceService(IHostApplicationLifetime hostApplicationLifetime)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Yield();
                var command = Console.ReadLine();
                switch (command)
                {
                    case "hello":
                        Console.WriteLine("Hello World!");
                        break;
                    default:
                        _hostApplicationLifetime.StopApplication();
                        return;
                }
            }
        }
    }
}