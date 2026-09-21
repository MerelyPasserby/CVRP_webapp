using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib
{
    public class TargetFunctionStrict : ITargetFunction
    {
        public double Evaluate(Solution solution, InputData data)
        {
            double capacity = 0.0;

            foreach(var route in solution.Routes)
            {
                for(int i = 0; i < route.Count; i++)
                {
                    capacity += route[i].Demand;
                    if (capacity > data.Capacity)
                    {
                        return double.MaxValue;
                    }
                }
                capacity = 0;
            }

            double target = 0.0;

            foreach (var route in solution.Routes)
            {
                for (int i = 0; i < route.Count - 1; i++)
                {
                    target += data.DistanceMatrix[i, i + 1];
                }
                
            }
            
            return target;
        }
    }
}
