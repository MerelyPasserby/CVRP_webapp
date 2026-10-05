using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.DataReaders
{
    public class SolomonInputDataReader : IInputDataReader
    {
        public InputData ParseData(List<string> input)
        {
            int i = 0;   
            
            while (!input[i].Trim().Contains("NUMBER"))
            {
                i++;
            }
            i++;

            string[] parts = input[i].Trim().Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            int maxCarNumber = int.Parse(parts[0]);
            double capacity = double.Parse(parts[1]);
            i++;

            while (!input[i].Trim().Contains("DEMAND"))
            {
                i++;
            }
            i += 2;

            List<Application> apps = new List<Application>();

            while (input[i].Trim() != "")
            {
                parts = input[i].Trim().Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

                apps.Add(new Application() { Id = int.Parse(parts[0]), 
                    Demand = double.Parse(parts[3]), 
                    Point = new Point() { X = double.Parse(parts[1]), Y = double.Parse(parts[2]) } });

                i++;
            }

            int depotId = apps[0].Id;

            DistanceFunction distanceFunction = DistanceFunction.EUC_2D;
            IDistanceFunction d = new Euclid2DDistanceFunction();

            double[,] m = new double[apps.Count, apps.Count];

            for (int j = 0; j < apps.Count; j++)
            {
                for(int k = 0; k < apps.Count; k++)
                {
                    m[j, k] = d.EvaluateDistance(apps[j].Point, apps[k].Point);
                }
            }

            return new InputData()
            {
                Applications = apps,
                DepotId = depotId,
                MaxCarCount = maxCarNumber,
                Capacity = capacity,
                DistanceMatrix = m,
                DistanceFunction = distanceFunction,
            };
        }
    }
}
