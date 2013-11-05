using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace PubSub
{
    public class Subscriber
    {
        private readonly NetMQContext _context;
        private readonly string _id;
        private readonly string _topic;

        public Subscriber(NetMQContext context, string id, string topic)
        {
            _context = context;
            _id = id;
            _topic = topic;
        }

        public Task Start(CancellationToken token, string address)
        {
            var ready = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => start(token, address, ready), token);
            return ready.Task;
        }

        private void start(CancellationToken token, string address, TaskCompletionSource<bool> ready)
        {
            using (var socket = _context.CreateSubscriberSocket())
            {
                socket.Subscribe(Encoding.ASCII.GetBytes(_topic));
                socket.Connect(address);
                ready.SetResult(true);

                while (token.IsCancellationRequested == false)
                {
                    socket.Receive(); //ignore topic
                    var messageBytes = socket.Receive();
                    var message = Encoding.ASCII.GetString(messageBytes);
                    Console.WriteLine("[{0}] Received - {1}", _id, message);
                }
            }
        }
    }
}