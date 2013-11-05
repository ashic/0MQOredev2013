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

            string serverAddress = "inproc://foo";

            var server = new Server(context, "server1");
            var client = new Client(context, "client1");

            var token = new CancellationTokenSource();

            server.Start(token.Token, serverAddress).Wait(token.Token);
            client.Start(token.Token, serverAddress).Wait(token.Token);

            Console.WriteLine("Hit enter to terminate.");
            Console.ReadLine();
            token.Cancel();
            context.Dispose();
        }
    }
}
