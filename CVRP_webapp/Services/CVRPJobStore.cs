using CVRPlib.Models;
using System.Collections.Concurrent;

namespace CVRP_webapp.Services
{
#pragma warning disable S101
    public class CVRPJobStore
#pragma warning restore S101
    {
        readonly ConcurrentDictionary<Guid, CVRPJob> _jobs = new();
        public void Add(CVRPJob job)
        {
            _jobs.TryAdd(job.Id, job);
        }
        public bool TryGetValue(Guid guid, out CVRPJob? job)
        {
            return _jobs.TryGetValue(guid, out job);
        }
    }
}
