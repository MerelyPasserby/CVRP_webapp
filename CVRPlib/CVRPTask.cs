using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib
{
#pragma warning disable S101
    public class CVRPTask
#pragma warning restore S101
    {
        readonly InputData _data;
        readonly ISolver _solver;
        readonly ITargetFunction _targetFunction;

        public CVRPTask(InputData data, ISolver solver, ITargetFunction targetFunction)
        {
            _data = data;
            _solver = solver;
            _targetFunction = targetFunction;
        }

        public CVRPTaskResult GetSolution(int multistartCount = 1, int historyCount = 0)
        {
            CVRPTaskResult res = new CVRPTaskResult();
            HillClimbingStatistics stats = new HillClimbingStatistics();

            for (int i = 0; i < multistartCount; i++)
            {   
                if(i < historyCount)
                {
                    stats = new HillClimbingStatistics();
                    _solver.OnSolutionImpoved += stats.AddEntry;
                }
                
                Solution solution = _solver.Solve(_data, _targetFunction);

                double solutionFValue = _targetFunction.Evaluate(solution, _data);
                res.Solutions.Add(new SolutionResult() { Solution = solution, FValue = solutionFValue });
                
                if (i < historyCount)
                {
                    res.Histories.Add(stats.History);
                    _solver.OnSolutionImpoved -= stats.AddEntry;
                }             
            }

            return res;
            
        }
    }
}
