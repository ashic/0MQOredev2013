using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NetMQ;

namespace PubSub
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

            string serverAddress = "tcp://127.0.0.1:9988";

            var publisher = new Publisher(context, "server1");
            var allSubscriber = new Subscriber(context, "clientAll", "");
            var ukSubscriber = new Subscriber(context, "clientUK", @"news/uk");
            var seSubscriber = new Subscriber(context, "clientSE", @"news/se");

            var token = new CancellationTokenSource();

            publisher.Start(token.Token, serverAddress).Wait(token.Token);
            allSubscriber.Start(token.Token, serverAddress).Wait(token.Token);
            ukSubscriber.Start(token.Token, serverAddress).Wait(token.Token);
            seSubscriber.Start(token.Token, serverAddress).Wait(token.Token);

            Console.WriteLine("Hit enter to terminate.");
            Console.ReadLine();
            token.Cancel();
            context.Dispose();
        }
    }
}
