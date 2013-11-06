using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Devices;

namespace MultiMulti
{
    public class Server
    {
        private readonly NetMQContext _context;
        private readonly string _id;

        public Server(NetMQContext context, string id)
        {
            _context = context;
            _id = id;
        }

        public void Start(CancellationToken token, string front, string back)
        {
            var queue = new QueueDevice(_context, front, back);
            queue.Start();
        }
    }
}