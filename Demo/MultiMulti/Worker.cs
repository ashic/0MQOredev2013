using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace MultiMulti
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

        public Task Start(CancellationToken token, string address)
        {
            var ready = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => start(token, address, ready), token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return ready.Task;
        }

        private void start(CancellationToken token, string address, TaskCompletionSource<bool> ready)
        {
            using (var socket = _context.CreateResponseSocket())
            {
                socket.Connect(address);
                ready.SetResult(true);
                while (token.IsCancellationRequested == false)
                {
                    var bytes = socket.Receive();
                    var sender = Encoding.ASCII.GetString(bytes);
                    Console.WriteLine("[{0}] Received request from {1}", _id, sender);
                    Task.Delay(TimeSpan.FromSeconds(3), token).Wait(token);
                    var message = string.Format("{0} says {1}.", _id, DateTime.Now);
                    socket.Send(Encoding.ASCII.GetBytes(message));
                }
            }
        }
    }
}