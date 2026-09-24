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

        public List<(Solution sol, double fValue)> GetSolution(int multistartCount = 1)
        {
            List<(Solution res, double fValue)> res = new List<(Solution res, double fValue)> ();

            for (int i = 0; i < multistartCount; i++)
            {             
                Solution solution = _solver.Solve(_data, _targetFunction);
                double solutionFValue = _targetFunction.Evaluate(solution, _data);
                res.Add((solution, solutionFValue));
            }

            return res;
            
        }
    }
}
