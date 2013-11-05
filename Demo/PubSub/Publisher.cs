using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;

namespace PubSub
{
    public class Publisher
    {
        private readonly NetMQContext _context;
        private readonly string _id;

        public Publisher(NetMQContext context, string id)
        {
            _context = context;
            _id = id;
        }

        public Task Start(CancellationToken token, string address)
        {
            var ready = new TaskCompletionSource<bool>();
            Task.Factory.StartNew(() => start(token, address, ready), token);
            return ready.Task;
        }

        private void start(CancellationToken token, string address, TaskCompletionSource<bool> ready)
        {
            using (var socket = _context.CreatePublisherSocket())
            {
                socket.Bind(address);
                ready.SetResult(true);
                long count = 0;
                while (token.IsCancellationRequested == false)
                {
                    count++;
                    Task.Delay(TimeSpan.FromSeconds(1), token).Wait(token);
                    var item = getNewsItem();
                    var topicBytes = Encoding.ASCII.GetBytes(item.Item1);
                    socket.Send(topicBytes, topicBytes.Length, sendMore:true);
                    string news = string.Format("{0}: {1}", count, item.Item2);
                    byte[] bytes = Encoding.ASCII.GetBytes(news);
                    socket.Send(bytes);
                }
            }
        }

        static readonly Random _rand = new Random(DateTime.Now.Millisecond);

        private static readonly Tuple<string, string>[] _items =
            {
                new Tuple<string, string>(@"news/uk", "Skillsmatter hosting Haskell Exchange in 2014."),
                new Tuple<string, string>(@"news/se", "Oredev is under way.") 
            };
        private static Tuple<string, string> getNewsItem()
        {
            var index = _rand.Next(0, 2);
            return _items[index];
        }
    }
}