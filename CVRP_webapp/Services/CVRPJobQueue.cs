using System.Threading.Channels;

namespace CVRP_webapp.Services
{
#pragma warning disable S101
    public class CVRPJobQueue
#pragma warning restore S101
    {
        readonly Channel<Guid> _queue = Channel.CreateUnbounded<Guid>();
        public ValueTask EnqueueAsync(Guid jobId)
        {
            return _queue.Writer.WriteAsync(jobId);
        }
        public ValueTask<Guid> DequeueAsync(CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAsync(cancellationToken); 
        }
    }
}
