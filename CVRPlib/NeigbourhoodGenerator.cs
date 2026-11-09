using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib
{
    internal static class NeigbourhoodGenerator
    {
        public static List<Solution> GetNeigbourhood(Solution sol)
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

        public static List<(Solution, (int i1, int j1, int i2, int j2))> GetNeigbourhoodTabu((Solution s, (int i1, int j1, int i2, int j2)) sol)
        {
            List<(Solution, (int f, int fb, int l, int la))> neigbours = new();
            List<Application> route;

            for (int r = 0; r < sol.s.Routes.Count; r++)
            {
                route = sol.s.Routes[r];

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
                        var newSolution = CloneSolution(sol.s);
                        var newRoute = newSolution.Routes[r];

                        TwoOptSwap(newRoute, i, j - i + 1);

                        int f = newRoute[i].Id;
                        int fb = newRoute[i-1].Id;
                        int l = newRoute[j].Id;
                        int la = newRoute[j+1].Id;

                        neigbours.Add((newSolution, (f, fb, l, la)));
                    }
                }
            }

            return neigbours;
        }

        static void TwoOptSwap(List<Application> route, int startInd, int count) => route.Reverse(startInd, count);
        public static Solution CloneSolution(Solution s) => new Solution { Routes = s.Routes.Select(r => new List<Application>(r)).ToList() };
    }
}
