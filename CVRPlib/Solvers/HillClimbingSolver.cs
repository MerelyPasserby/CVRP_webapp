using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CVRPlib.Solvers
{
    public class HillClimbingSolver : ISolver
    {
        public event EventHandler<SolutionEventArgs>? OnSolutionImpoved;
        void Improved(SolutionEventArgs e)
        {
            OnSolutionImpoved?.Invoke(this, e);
        }
        static List<Solution> GetNeigbourhood(Solution sol)
        {
            List<Solution> neigbours = new List<Solution>();
            List<Application> route;

            for (int r = 0; r < sol.Routes.Count; r++)
            {
                route = sol.Routes[r];

                if (route.Count < 4)
                {
                    continue;
                }

                int firstClientInd = 1;
                int lastClientInd = route.Count - 2;

                for (int i = firstClientInd; i < lastClientInd; i++)
                {
                    for (int j = i + 1; j < lastClientInd + 1; j++)
                    {
                        var newSolution = CloneSolution(sol);
                        var newRoute = newSolution.Routes[r];

                        TwoOptSwap(newRoute, i, j - i + 1);

                        neigbours.Add(newSolution);
                    }
                }
            }

            return neigbours;
        }
        static void TwoOptSwap(List<Application> route, int startInd, int count)
        {
            route.Reverse(startInd, count);
        }
        static Solution CloneSolution(Solution s)
        {
            return new Solution
            {
                Routes = s.Routes
                    .Select(r => new List<Application>(r))
                    .ToList()
            };
        }
        static Solution GenerateInitialSolution(InputData data)
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
        static void PostFix(Solution best)
        {
            for (int i = 0; i < best.Routes.Count; i++)
            {
                if (best.Routes[i].Count == 2)
                {
                    best.Routes.Remove(best.Routes[i]);
                    i--;
                }
            }
        }
        Solution Outer(Solution initial, ITargetFunction targetFunction, InputData data)
        {
            Improved(new SolutionEventArgs() { ImprovedFValue = targetFunction.Evaluate(initial, data) });
            var res = HillClimbing(initial, targetFunction, data, mode: 1);

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
                                Solution newSolution = CloneSolution(res);

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
                    res = HillClimbing(bestSolution, targetFunction, data, mode: 1);
                }
            }
            return res;
        }
        Solution HillClimbing(Solution initial, ITargetFunction f, InputData data, int mode = 0)
        {
            Solution res = initial;
            Solution? best;
            List<Solution> neigbors;

            Func<List<Solution>, InputData, ITargetFunction, double, Solution?> func = mode switch
            {
                0 => FindBest,
                1 => FindFirst,
                2 => FindStochastic,
                _ => FindBest
            };

            bool found = false;

            while (!found)
            {
                neigbors = GetNeigbourhood(res);
                best = func(neigbors, data, f, f.Evaluate(res, data));

                if (best != null && f.Evaluate(best, data) < f.Evaluate(res, data))
                {
                    res = best;
                    Improved(new SolutionEventArgs() { ImprovedFValue = f.Evaluate(res, data) });
                }
                else
                {
                    found = true;
                }
            }

            return res;
        }
        static Solution? FindBest(List<Solution> neigbors, InputData data, ITargetFunction f, double res)
        {
            return neigbors.MinBy(s => f.Evaluate(s, data));
        }
        static Solution? FindFirst(List<Solution> neigbors, InputData data, ITargetFunction f, double res)
        {
            double tmp;
            for(int i = 0; i < neigbors.Count; i++)
            {
                tmp = f.Evaluate(neigbors[i], data);
                if(tmp < res)
                {
                    return neigbors[i];
                }
            }
            return neigbors.Count > 0 ? neigbors[0] : null;
        }
        static Solution? FindStochastic(List<Solution> neigbors, InputData data, ITargetFunction f, double res)
        {
            return neigbors.Shuffle().Take((int)Math.Sqrt(neigbors.Count)).MinBy(s => f.Evaluate(s, data));
        }
    }
}
