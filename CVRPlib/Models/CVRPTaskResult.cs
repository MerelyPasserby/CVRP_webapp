using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Models
{
#pragma warning disable S101
    public class CVRPTaskResult
#pragma warning restore S101
    {
        public string Solver { get; set; } = "";
        public string Time { get; set; } = "";
        public List<SolutionResult> Solutions { get; set; } = new List<SolutionResult>();
        public SolutionResult Best { get; set; } = new SolutionResult();
        public SolutionResult Worst {  get; set; } = new SolutionResult();
        public List<List<double>> Histories { get; set; } = new List<List<double>>();
    }
}
