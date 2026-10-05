using System;
using System.Collections.Generic;
using System.Text;
using CVRPlib;
using CVRPlib.Models;

namespace CVRPlib.Tests
{
    public class Euclid2DTests
    {
        [Test]
        public void Test()
        {
            //Arrange
            Point p1 = new Point() { X = 1, Y = 2 };
            Point p2 = new Point() { X = 4, Y = 6 };

            //Act
            double d = new Euclid2DDistanceFunction().EvaluateDistance(p1, p2);

            //Assert
            Assert.That(d, Is.EqualTo(5.0));
        }
    }
}
