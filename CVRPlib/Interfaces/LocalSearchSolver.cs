using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Interfaces
{
    internal abstract class LocalSearchSolver : ISolver
    {
        public event EventHandler<SolutionEventArgs>? OnSolutionImpoved;
        protected void Improved(SolutionEventArgs e) => OnSolutionImpoved?.Invoke(this, e);
        protected static Solution GenerateInitialSolution(InputData data)
        {
            Solution solution = new Solution() { Routes = new List<List<Application>>() };

            List<Application> applications = data.Applications.ToList();
            var shuffled = applications.Shuffle().ToList();
            int j = 0;
            int i = 0;

            int startCarNeeded = (int)(applications.Sum(a => a.Demand) / data.Capacity);
            int addon = (int)Math.Sqrt(applications.Count);
            //int addon = (int)(startCarNeeded * 0.2) < 5 ? 5 : (int)(startCarNeeded * 0.2);
            startCarNeeded += addon;
            startCarNeeded = Math.Min(startCarNeeded, data.MaxCarCount);

            double fullness = Math.Min(applications.Max(a => a.Demand) / data.Capacity + 0.1, 1);
            fullness = Math.Max(fullness, 0.75);
            double carInitialFullness = data.Capacity * fullness;

            Application depot = applications.Find(a => a.Id == data.DepotId) ?? data.Applications[0];
            shuffled.Remove(depot);

            while (solution.Routes.Count < startCarNeeded)
            {
                solution.Routes.Add(new List<Application>());
                solution.Routes[i].Add(depot);
                double currentCapacity = 0;

                while (j < shuffled.Count && currentCapacity + shuffled[j].Demand < carInitialFullness)
                {
                    solution.Routes[i].Add(shuffled[j]);
                    currentCapacity += shuffled[j].Demand;
                    j++;
                }

                solution.Routes[i].Add(depot);
                i++;
            }

            if (solution.Routes.Count > data.MaxCarCount || shuffled.Count != j)
            {
                solution = GenerateInitialSolution(data);
            }

            return solution;
        }
        public Solution Solve(InputData data, ITargetFunction targetFunction)
        {
            Solution initial = GenerateInitialSolution(data);

            Solution best = Outer(initial, targetFunction, data);

            PostFix(best);

            return best;
        }
        protected static void PostFix(Solution best)
        {
            for (int i = best.Routes.Count - 1 ; i >= 0; i--)
            {
                if (best.Routes[i].Count == 2)
                {
                    best.Routes.Remove(best.Routes[i]);
                }
            }
        }
        protected Solution Outer(Solution initial, ITargetFunction targetFunction, InputData data)
        {
            Improved(new SolutionEventArgs() { ImprovedFValue = targetFunction.Evaluate(initial, data) });
            var res = Euristic(initial, targetFunction, data, mode: 1);

            if (res.Routes.Count < 2)
            {
                return res;
            }

            bool found = false;

            while (!found)
            {
                found = true;

                Solution bestSolution = res;
                double bestFValue = targetFunction.Evaluate(bestSolution, data);

                for (int r1 = 0; r1 < res.Routes.Count; r1++)
                {
                    int firstClientInd = 1;
                    int lastClientInd = res.Routes[r1].Count - 2;

                    for (int i = firstClientInd; i <= lastClientInd; i++)
                    {
                        for (int r2 = 0; r2 < res.Routes.Count; r2++)
                        {
                            if (r1 == r2)
                            {
                                continue;
                            }

                            int targetLastClientInd = res.Routes[r2].Count - 2;

                            for (int j = firstClientInd; j <= targetLastClientInd + 1; j++)
                            {
                                Solution newSolution = NeigbourhoodGenerator.CloneSolution(res);

                                List<Application> newSource = newSolution.Routes[r1];
                                List<Application> newTarget = newSolution.Routes[r2];

                                Application movedApplication = newSource[i];

                                newSource.RemoveAt(i);
                                newTarget.Insert(j, movedApplication);

                                double fValue = targetFunction.Evaluate(newSolution, data);

                                if (fValue < bestFValue)
                                {
                                    found = false;
                                    bestSolution = newSolution;
                                    bestFValue = fValue;
                                }
                            }
                        }
                    }
                }
                if (!found)
                {
                    Improved(new SolutionEventArgs() { ImprovedFValue = bestFValue });
                    res = Euristic(bestSolution, targetFunction, data, mode: 1);
                }
            }
            return res;
        }
        protected abstract Solution Euristic(Solution initial, ITargetFunction f, InputData data, int mode = 0);
    }
}
