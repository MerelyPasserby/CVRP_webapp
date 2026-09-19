using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Interfaces
{
    public interface ISolver
    {
        public Solution Solve(InputData data, ITargetFunction targetFunction);
    }
}
