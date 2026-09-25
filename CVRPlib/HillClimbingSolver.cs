using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib
{
    public class HillClimbingSolver : ISolver
    {
        static List<Solution> GetNeigbourhood(Solution sol)
        {
            List<Solution> neigbours = new List<Solution>();
            List<Application> route;

            for(int r = 0; r < sol.Routes.Count; r++)
            {
                route = sol.Routes[r];

                if(route.Count < 4)
                {
                    continue;
                }

                int firstClientInd = 1; //first after depot so 0 + 1
                int lastClientInd = route.Count - 2; // last before depot, so -1 for it is index and -1 for depot

                for(int i = firstClientInd;  i < lastClientInd; i++)
                {
                    for(int j = i + 1; j < lastClientInd + 1; j++)
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
            List<Application> applications = data.Applications.ToList();
            double overalDemand = data.Applications.Sum(a => a.Demand);
            int minCarsNeeded = (int)(overalDemand / data.Capacity) + 1;

            minCarsNeeded = Math.Min(minCarsNeeded, data.MaxCarCount);

            Solution solution = new Solution() { Routes = new List<List<Application>>(minCarsNeeded * 2)};

            var shuffled = applications.Shuffle().ToList();
            int j = 0;

            Application depot = applications.Find(a => a.Id == data.DepotId) ?? data.Applications[0];
            shuffled.Remove(depot);

            for(int i = 0; i < minCarsNeeded; i++)
            {
                solution.Routes.Add(new List<Application>());
                solution.Routes[i].Add(depot);
                double currentCapacity = 0;

                while (currentCapacity < data.Capacity)
                {
                    if(j >= shuffled.Count)
                    {
                        break;
                    }

                    solution.Routes[i].Add(shuffled[j]);
                    currentCapacity += shuffled[j].Demand;
                    j++;
                }

                solution.Routes[i].Add(depot);
            }

            while(j < shuffled.Count)
            {
                solution.Routes.Add(new List<Application>());
                solution.Routes[^1].Add(depot);
                double currentCapacity = 0;

                while (currentCapacity < data.Capacity)
                {
                    if (j >= shuffled.Count)
                    {
                        break;
                    }

                    solution.Routes[^1].Add(shuffled[j]);
                    currentCapacity += shuffled[j].Demand;
                    j++;
                }

                solution.Routes[^1].Add(depot);
            }

            if(solution.Routes.Count > data.MaxCarCount)
            {
                solution = GenerateInitialSolution(data);
            }

            return solution;
        }

        public Solution Solve(InputData data, ITargetFunction targetFunction)
        {
            Solution initial = GenerateInitialSolution(data);

            Solution best = Outer(initial, targetFunction, data);

            foreach(var route in best.Routes)
            {
                if (route.Count == 2)
                {
                    best.Routes.Remove(route);
                }
            }

            return best;           
        }

        static Solution Outer(Solution initial,  ITargetFunction targetFunction, InputData data)
        {   
            var res = HillClimbing(initial, targetFunction, data);

            if(res.Routes.Count > 1)
            {
                bool found = false;

                while (!found)
                {
                    found = true;

                    Solution bestSolution = res;
                    double bestFValue = targetFunction.Evaluate(bestSolution, data);

                    for (int r1 = 0; r1 < res.Routes.Count; r1++)
                    {
                        List<Application> sourceRoute = res.Routes[r1];
                        int firstClientInd = 1; //first after depot so 0 + 1
                        int lastClientInd = sourceRoute.Count - 2; // last before depot, so -1 for it is index and -1 for depot

                        for (int i = firstClientInd; i <= lastClientInd; i++)
                        {
                            for (int r2 = 0; r2 < res.Routes.Count; r2++)
                            {
                                if (r1 == r2)
                                {
                                    continue;
                                }

                                List<Application> targetRoute = res.Routes[r2];
                                int targetLastClientInd = targetRoute.Count - 2;

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
                        res = HillClimbing(bestSolution, targetFunction, data);
                    }
                }             
            }

            return res;
        }

        static Solution HillClimbing(Solution initial, ITargetFunction f, InputData data)
        {
            Solution res = initial;
            Solution? best;
            List<Solution> neigbors;

            bool found = false;

            while (!found)
            {
                neigbors = GetNeigbourhood(res);
                best = neigbors.MinBy(s => f.Evaluate(s, data));

                if (best != null && f.Evaluate(best, data) < f.Evaluate(res, data))
                {
                    res = best;
                }
                else
                {
                    found = true;
                }
            }

            return res;
        }
    }
}
