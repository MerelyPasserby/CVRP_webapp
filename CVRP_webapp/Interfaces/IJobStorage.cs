using CVRPlib.Models;

namespace CVRP_webapp.Interfaces
{
    public interface IJobStorage
    {
        Task SaveAsync(CVRPJob job);
        Task<CVRPJob?> GetAsync(Guid id);
    }
}
