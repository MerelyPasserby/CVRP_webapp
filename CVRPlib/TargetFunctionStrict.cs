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
            double penalty = 0.0;

            foreach(var route in solution.Routes)
            {
                for(int i = 0; i < route.Count; i++)
                {
                    capacity += route[i].Demand;
                    if (capacity > data.Capacity)
                    {
                        penalty += (capacity - data.Capacity) * 1000;
                    }
                }
                capacity = 0;
            }

            double target = 0.0;

            foreach (var route in solution.Routes)
            {
                for (int i = 0; i < route.Count - 1; i++)
                {
                    int startAppId = route[i].Id;
                    int endAppId = route[i + 1].Id;

                    int startPos = data.Applications.FindIndex(app => app.Id == startAppId);
                    int endPos = data.Applications.FindIndex(app => app.Id == endAppId);

                    target += data.DistanceMatrix[startPos, endPos];
                }
                
            }
            
            return target + penalty;
        }
    }
}
