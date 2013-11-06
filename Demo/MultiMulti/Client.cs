using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace MultiMulti
{
    public class Client
    {
        private readonly NetMQContext _context;
        private readonly string _id;

        public Client(NetMQContext context, string id)
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
            using (var socket = _context.CreateRequestSocket())
            {
                socket.Connect(address);
                ready.SetResult(true);
                while (token.IsCancellationRequested == false)
                {
                    var bytes = Encoding.ASCII.GetBytes(_id);
                    socket.Send(bytes);
                    var response = socket.Receive();
                    var message = Encoding.ASCII.GetString(response);
                    Console.WriteLine("[{0}] Received - {1}", _id, message);
                }
            }
        }
    }
}