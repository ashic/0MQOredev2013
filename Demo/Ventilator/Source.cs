using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace Ventilator
{
    public class Source
    {
        private readonly NetMQContext _context;
        private readonly string _id;
        private readonly int _numberToPush;

        public Source(NetMQContext context, string id, int numberToPush)
        {
            _context = context;
            _id = id;
            _numberToPush = numberToPush;
        }

        public Task Start(CancellationToken token, string address)
        {
            var ready = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => start(token, address, ready), token);
            return ready.Task;
        }

        private void start(CancellationToken token, string address, TaskCompletionSource<bool> ready)
        {
            using (var socket = _context.CreatePushSocket())
            {
                socket.Bind(address);
                ready.SetResult(true);
                long count = 0;
                while (token.IsCancellationRequested == false && count < _numberToPush)
                {
                    count++;
                    //Task.Delay(TimeSpan.FromSeconds(1), token).Wait(token);
                    var item = count.ToString(CultureInfo.InvariantCulture);
                    var bytes = Encoding.ASCII.GetBytes(item);
                    socket.Send(bytes);
                }

                Console.WriteLine("[Source {0}] Pushed {1} tasks.", _id, count);
            }
        }
    }
}