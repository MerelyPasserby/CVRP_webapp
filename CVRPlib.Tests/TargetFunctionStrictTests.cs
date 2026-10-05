using CVRPlib.DataReaders;
using CVRPlib.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Tests
{
    public class TargetFunctionStrictTests
    {
        [Test]
        public void NormalTest()
        {
            //Arrange
            List<string> lines = new();
            using (var stream = new StreamReader(@"C:\Users\User\Desktop\Уник\М1к1с\Евристика\Лб\InputData\CVRPlib\test.txt"))
            {
                lines = stream.ReadToEnd().Split('\n').ToList();
            }
            InputData data = new InputDataReader().ParseData(lines);
            Solution solution = new Solution() { Routes = new List<List<Application>> { new List<Application>(data.Applications[0..3]), new List<Application>(data.Applications[3..])  }  };
            TargetFunctionStrict f = new TargetFunctionStrict();

            //Act
            double actual = f.Evaluate(solution, data);

            //Assert
            Assert.That(actual, Is.Not.EqualTo(double.MaxValue));
            Assert.That(actual, Is.EqualTo(3));
        }

        [Test]
        public void ReturnMaxValueTest()
        {
            //Arrange
            List<string> lines = new();
            using (var stream = new StreamReader(@"C:\Users\User\Desktop\Уник\М1к1с\Евристика\Лб\InputData\CVRPlib\test.txt"))
            {
                lines = stream.ReadToEnd().Split('\n').ToList();
            }
            InputData data = new InputDataReader().ParseData(lines);
            Solution solution = new Solution() { Routes = new List<List<Application>> { new List<Application>(data.Applications)} };
            TargetFunctionStrict f = new TargetFunctionStrict();

            //Act
            double actual = f.Evaluate(solution, data);

            //Assert
            Assert.That(actual, Is.EqualTo(double.MaxValue));
            Assert.That(actual, Is.Not.EqualTo(3));
        }
    }
}
