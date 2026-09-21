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

        public Solution GetSolution()
        {
            return _solver.Solve(_data, _targetFunction);
        }
    }
}
