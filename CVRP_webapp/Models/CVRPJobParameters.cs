namespace CVRP_webapp.Models
{
#pragma warning disable S101
    public class CVRPJobParameters
#pragma warning restore S101
    {
        public int MultistartCount { get; init; } = 0;
        public int HistoryCount { get; init; } = 0;
        public string Solver { get; init; } = "";
    }
}
