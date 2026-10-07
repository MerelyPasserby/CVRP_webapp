using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;

namespace CVRPlib.DataReaders
{
    public class InputDataReader : IInputDataReader
    {
        public InputData ParseData(List<string> input)
        {
            int i = 0;
            string line = input[0];

            IDistanceFunction d = new Euclid2DDistanceFunction();   
            int maxCarCount = 0; 
            DistanceFunction dist = DistanceFunction.EUC_2D;
            double cap = 0.0;
            List<(int Id, Point Point)> points = new();
            List<double> demands = new();
            int depotId = 1;

            while (line.Trim() != "EOF")
            {
                var parts = line.Trim().Split(' ');

                switch (parts[0])
                {            
                    case "COMMENT":
                        int ind = parts.ToList().FindIndex(part => part.Contains("trucks:"));
                        string value = parts[ind + 1].Trim('.', ',', ';');
                        maxCarCount = (Convert.ToInt32(value) + 1) * 2;
                        i++;
                        break;                    
                    case "EDGE_WEIGHT_TYPE":
                        (dist, d) = parts[^1] switch
                        {
                            "EUC_2D" => (DistanceFunction.EUC_2D, new Euclid2DDistanceFunction()),
                            _ => (DistanceFunction.EUC_2D, new Euclid2DDistanceFunction())
                        };
                        i++;
                        break;
                    case "CAPACITY":
                        cap = Convert.ToDouble(parts[^1]);
                        i++;
                        break;
                    case "NODE_COORD_SECTION":
                        i++;

                        while (input[i].Trim() != "DEMAND_SECTION")
                        {
                            parts = input[i].Trim().Split(' ');

                            points.Add(new() { Id = Convert.ToInt32(parts[0]) - 1, Point = new() { X = Convert.ToDouble(parts[1]), Y = Convert.ToDouble(parts[2]) } });

                            i++;
                        }                

                        break;
                    case "DEMAND_SECTION":
                        i++;

                        while (input[i].Trim() != "DEPOT_SECTION")
                        {
                            parts = input[i].Trim().Split(' ');
                        
                            demands.Add(Convert.ToDouble(parts[1]));

                            i++;
                        }

                        break;
                    case "DEPOT_SECTION":
                        i++;
                        depotId = Convert.ToInt32(input[i].Trim()) - 1;
                        i += 2;
                        break;
                    default:
                        i++;
                        break;
                }

                line = input[i];
            }

            List<Application> applications = new List<Application>();
            for(int j = 0; j < points.Count; j++)
            {
                applications.Add(new Application() { Id = points[j].Id, Point = points[j].Point, Demand = demands[j]});
            }

            int appsCount = applications.Count;

            double[,] m = new double[appsCount, appsCount];
            for(int j = 0; j < appsCount; j++)
            {
                for(int k = 0; k < appsCount; k++)
                {
                    m[j, k] = d.EvaluateDistance(applications[j].Point, applications[k].Point);
                }
            }

            return new InputData() 
            { 
                MaxCarCount = maxCarCount,                      
                DistanceFunction = dist, 
                Capacity = cap, 
                Applications = applications, 
                DepotId = depotId,
                DistanceMatrix = m };
        }
    }
}
