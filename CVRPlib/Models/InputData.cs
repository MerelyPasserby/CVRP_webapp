namespace CVRPlib.Models
{
    public class InputData
    {
        public DistanceFunction DistanceFunction { get; init; }
        public int MaxCarCount { get; init; }
        public double Capacity { get; init; }
        public List<Application> Applications { get; init; } = new List<Application>();
        public int DepotId { get; init; }
        public double[,] DistanceMatrix { get; init; } = new double[1,1];
    }

    public enum DistanceFunction
    {
        EUC_2D
    }


}
