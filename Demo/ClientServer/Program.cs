using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.zmq;

namespace ClientServer
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

            var server = new Server(context, "server1");
            var client = new Client(context, "client1");

            var token = new CancellationTokenSource();

            server.Start(token.Token, serverAddress).Wait(token.Token);
            client.Start(token.Token, serverAddress).Wait(token.Token);

            Console.WriteLine("Hit enter to add another client.");
            Console.ReadLine();
            new Client(context, "client2").Start(token.Token, serverAddress).Wait(token.Token);

            Console.WriteLine("Hit enter to terminate.");
            Console.ReadLine();
            token.Cancel();
            context.Dispose();
        }
    }
}
