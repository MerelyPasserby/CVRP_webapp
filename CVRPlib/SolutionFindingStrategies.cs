using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib
{
    internal static class SolutionFindingStrategies
    {
        public static Solution? FindBest(this List<Solution> neigbors, InputData data, ITargetFunction f, double res) 
            => neigbors.MinBy(s => f.Evaluate(s, data));
        public static Solution? FindFirst(this List<Solution> neigbors, InputData data, ITargetFunction f, double res)
        {
            double tmp;
            for (int i = 0; i < neigbors.Count; i++)
            {
                tmp = f.Evaluate(neigbors[i], data);
                if (tmp < res)
                {
                    return neigbors[i];
                }
            }
            return neigbors.Count > 0 ? neigbors[0] : null;
        }
        public static Solution? FindStochastic(this List<Solution> neigbors, InputData data, ITargetFunction f, double res) 
            => neigbors.Shuffle().Take((int)Math.Sqrt(neigbors.Count)).MinBy(s => f.Evaluate(s, data));
    }
}
