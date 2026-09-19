using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Models
{
    public class Application
    {
        public int Id { get; init; }
        public Point Point { get; init; } = new Point();
        public double Demand { get; init; }
    }
}
