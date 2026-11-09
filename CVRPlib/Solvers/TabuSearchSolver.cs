using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CVRPlib.Solvers
{
    public class TabuSearchSolver : LocalSearchSolver
    {
        private int[,] shortMemory = null!;
        private int[,] longMemory = null!;

        private void InitMemory(int size)
        {
            shortMemory = new int[size, size];

            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    shortMemory[i, j] = 0;
                }
            }

            longMemory = new int[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    longMemory[i, j] = 0;
                }
            }
        }

        private void ResetMemory()
        {
            for (int i = 0; i < shortMemory.GetLength(0); i++)
            {
                for (int j = 0; j < shortMemory.GetLength(1); j++)
                {
                    shortMemory[i, j] = 0;
                }
            }

            for (int i = 0; i < longMemory.GetLength(0); i++)
            {
                for (int j = 0; j < longMemory.GetLength(1); j++)
                {
                    longMemory[i, j] = 0;
                }
            }
        }

        protected override Solution Euristic(Solution initial, ITargetFunction f, InputData data, int mode = 0)
        {
            (Solution s, (int f, int fb, int l, int la) edges) res = (initial, (0,0,0,0));
            Solution? best = res.s;
            List<(Solution s, (int f, int fb, int l, int la) edges)> neigbors;
            List<((Solution s, (int f, int fb, int l, int la) edges) sol, double value)> values = new();

            InitMemory(data.Applications.Count);

            //can experiment
            int iter = 99;
            int a = 50;

            bool found = false;

            do
            {
                neigbors = NeigbourhoodGenerator.GetNeigbourhoodTabu(res);

                foreach (var neigbour in neigbors)
                {
                    if (shortMemory[neigbour.edges.fb, neigbour.edges.f] == 0 && shortMemory[neigbour.edges.l, neigbour.edges.la] == 0)
                    {
                        values.Add((neigbour, f.Evaluate(neigbour.s, data) + a * (longMemory[neigbour.edges.fb, neigbour.edges.f] + longMemory[neigbour.edges.l, neigbour.edges.la])));
                    }
                    else
                    {
                        if(f.Evaluate(neigbour.s, data) < f.Evaluate(best, data))
                        {
                            res = neigbour;
                            best = neigbour.s;
                            found = true;
                            Improved(new SolutionEventArgs() { ImprovedFValue = f.Evaluate(res.s, data) });
                            break;
                        }
                    }
                }

                if (!found)
                {
                    res = values.MinBy(s => s.value).sol;
                    Improved(new SolutionEventArgs() { ImprovedFValue = f.Evaluate(res.s, data)});

                    if (f.Evaluate(res.s, data) < f.Evaluate(best, data))
                    {
                        best = res.s;
                    }                
                }

                //can experiment
                shortMemory[res.edges.fb, res.edges.f] = 5;
                shortMemory[res.edges.l, res.edges.la] = 5;
                longMemory[res.edges.fb, res.edges.f] += 1;
                longMemory[res.edges.l, res.edges.la] += 1;

                for (int i = 0; i < shortMemory.GetLength(0); i++)
                {
                    for (int j = 0; j < shortMemory.GetLength(1); j++)
                    {
                        if (shortMemory[i, j] > 0)
                        {
                            shortMemory[i, j]--;
                        }
                    }
                }

                //Can experiment
                if (iter % 50 == 0) 
                {
                    ResetMemory();
                }

                found = false;
                values.Clear();

            } while (iter-- != 0);

            return best;
        }
    }
}
