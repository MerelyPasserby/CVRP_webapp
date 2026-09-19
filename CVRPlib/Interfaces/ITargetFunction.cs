using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Interfaces
{
    public interface ITargetFunction
    {
        double Evaluate(Solution solution, InputData data);
    }
}
