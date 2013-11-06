using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace Ventilator
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            var context = NetMQContext.Create();

            string sourceAddress = "tcp://127.0.0.1:9988";
            string sinkAddress = "tcp://127.0.0.1:9989";

            var source = new Source(context, "source", 100);
            var workers = new[]
            {
                new Worker(context, "W1"),
                new Worker(context, "W2"), 
                new Worker(context, "W3")
            };

            var sink = new Sink(context, "sink", 100);

            var token = new CancellationTokenSource();

            sink.Start(token.Token, sinkAddress).Wait(token.Token);
            foreach (var worker in workers)
                worker.Start(token.Token, sourceAddress, sinkAddress).Wait(token.Token);
            Task.Delay(1000, token.Token).Wait(token.Token);
            source.Start(token.Token, sourceAddress).Wait(token.Token);

            Console.WriteLine("Hit enter to terminate.");
            Console.ReadLine();
            token.Cancel();
            context.Dispose();
        }
    }
}
