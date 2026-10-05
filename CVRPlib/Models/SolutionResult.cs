using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Models
{
    public class SolutionResult
    {
        public Solution Solution { get; init; } = new Solution();
        public double FValue { get; init; }
    }
}
