using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Models
{
    public class CVRPTaskResult
    {
        public List<SolutionResult> Solutions { get; set; } = new List<SolutionResult>();
        public List<List<double>> Histories { get; set; } = new List<List<double>>();
    }
}
