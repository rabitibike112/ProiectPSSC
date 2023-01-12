using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Proiect_lab6.Events;
using System.Threading;

namespace Proiect_lab6.Accomodation.EventProcessor
{
    internal class Worker : IHostedService
    {
        private readonly IEventListener eventListener;

        public Worker(IEventListener eventListener)
        {
            this.eventListener = eventListener;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Worker started...");
            return eventListener.StartAsync("", "", cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Worker stoped!");
            return eventListener.StopAsync(cancellationToken);
        }
    }
}
