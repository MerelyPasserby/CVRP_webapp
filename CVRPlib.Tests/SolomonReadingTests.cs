using CVRPlib.Models;
using CVRPlib.DataReaders;
using System;
using System.Collections.Generic;
using System.Text;

namespace CVRPlib.Tests
{
    public class SolomonReadingTests
    {
        [Test]
        public void Test()
        {
            //Arrange
            List<string> lines = new();
            using (var file = new StreamReader(@"C:\Users\User\Desktop\Уник\М1к1с\Евристика\Лб\InputData\Solomon\C1_2_1.txt"))
            {
                lines = file.ReadToEnd().Split("\n").ToList();
            }

            //Act
            InputData data = new SolomonInputDataReader().ParseData(lines);

            //Assert
            Assert.That(data, Is.Not.Null);
            Assert.That(data.DistanceMatrix, Is.Not.Null);
            Assert.That(data.DistanceMatrix[0][0], Is.EqualTo(0));
            Assert.That(data.MaxCarCount, Is.EqualTo(50));
        }
    }
}
