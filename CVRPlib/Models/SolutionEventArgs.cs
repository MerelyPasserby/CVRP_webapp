using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Models
{
    public class SolutionEventArgs : EventArgs
    {
        public double ImprovedFValue { get; set; }
    }
}
