using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using CVRPlib.Models;
using Point = CVRPlib.Models.Point;

namespace CVRPlib.Interfaces
{
    public interface IDistanceFunction
    {
        double EvaluateDistance(Point a, Point b);
    }
}
