using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace MultiMulti
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

            //string frontEnd = "tcp://127.0.0.1:9988";
            //string backendEnd = "tcp://127.0.0.1:9989";

            string frontEnd = "inproc://front";
            string backendEnd = "inproc://back";

            var worker = new Worker(context, "worker1");
            var client1 = new Client(context, "client1");
            var client2 = new Client(context, "client2");
            var client3 = new Client(context, "client3");
            var server = new Server(context, "server");

            var token = new CancellationTokenSource();

            server.Start(token.Token, frontEnd, backendEnd);
            worker.Start(token.Token, backendEnd).Wait(token.Token);
            client1.Start(token.Token, frontEnd).Wait(token.Token);
            client2.Start(token.Token, frontEnd).Wait(token.Token);
            client3.Start(token.Token, frontEnd).Wait(token.Token);


            var workerCount = 1;
            while (true)
            {
                Console.WriteLine("'add' to add a worker, 'quit' to quit.");
                var input = Console.ReadLine();

                if (input == "add")
                {
                    workerCount++;
                    string id = string.Format("worker{0}", workerCount);
                    new Worker(context, id).Start(token.Token, backendEnd).Wait(token.Token);
                }
                else if (input == "quit")
                    break;
                else
                    Console.WriteLine("Sorry...invalid input.");
            }


            token.Cancel();
            context.Dispose();
        }
    }
}
