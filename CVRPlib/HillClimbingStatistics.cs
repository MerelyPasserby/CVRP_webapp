using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib
{
    internal class HillClimbingStatistics
    {
        public List<double> History { get; init; } = new List<double>();

        public void AddEntry(object? sender, SolutionEventArgs e)
        {
            History.Add(e.ImprovedFValue);
        }
    }
}
