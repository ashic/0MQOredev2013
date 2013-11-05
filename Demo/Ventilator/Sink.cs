using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace Ventilator
{
    public class Sink
    {
        private readonly NetMQContext _context;
        private readonly string _id;
        private readonly int _numberToReceive;
        private readonly string _topic;

        public Sink(NetMQContext context, string id, int numberToReceive)
        {
            _context = context;
            _id = id;
            _numberToReceive = numberToReceive;
        }

        public Task Start(CancellationToken token, string address)
        {
            var ready = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => start(token, address, ready), token);
            return ready.Task;
        }

        private void start(CancellationToken token, string address, TaskCompletionSource<bool> ready)
        {
            using (var socket = _context.CreatePullSocket())
            {
                socket.Bind(address);
                ready.SetResult(true);

                int received = 0;

                while (token.IsCancellationRequested == false && received < _numberToReceive)
                {
                    var bytes = socket.Receive();
                    received++;
                    var taskId = Encoding.ASCII.GetString(bytes);
                    Console.WriteLine("[{0}] Received - {1}", _id, taskId);
                }

                if(received == _numberToReceive)
                    Console.WriteLine("[{0}] Received {1} results.", _id, _numberToReceive);
            }
        }
    }
}