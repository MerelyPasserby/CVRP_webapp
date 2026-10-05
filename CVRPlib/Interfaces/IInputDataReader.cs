using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Interfaces
{
    public interface IInputDataReader
    {
        public InputData ParseData(List<string> input);
    }
}
