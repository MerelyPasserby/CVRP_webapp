using CVRPlib.Interfaces;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Solvers
{
    internal class TabuSearchSolver : LocalSearchSolver
    {
        protected override Solution Euristic(Solution initial, ITargetFunction f, InputData data, int mode = 0)
        {
            throw new NotImplementedException();
        }
    }
}
