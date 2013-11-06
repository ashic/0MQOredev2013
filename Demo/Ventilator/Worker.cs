using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace Ventilator
{
    public class Worker
    {
        private readonly NetMQContext _context;
        private readonly string _id;

        public Worker(NetMQContext context, string id)
        {
            _context = context;
            _id = id;
        }

        public Task Start(CancellationToken token, string source, string sink)
        {
            var ready = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => start(token, source, sink, ready), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return ready.Task;
        }

        private void start(CancellationToken token, string source, string sink, TaskCompletionSource<bool> ready)
        {
            using (var sourceSocket = _context.CreatePullSocket())
            using(var sinkSocket = _context.CreatePushSocket())
            {
                sourceSocket.Connect(source);
                sinkSocket.Connect(sink);
                ready.SetResult(true);

                while (token.IsCancellationRequested == false)
                {
                    var messageBytes = sourceSocket.Receive();
                    var message = Encoding.ASCII.GetString(messageBytes);

                    Console.WriteLine("[{0}] Processing task {1}", _id, message);
                    Task.Delay(50, token).Wait(token);

                    sinkSocket.Send(messageBytes);
                }
            }
        }
    }
}