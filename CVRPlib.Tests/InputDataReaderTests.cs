using CVRPlib.Models;

namespace CVRPlib.Tests
{
    public class InputDataReaderTests
    {
        [Test]
        public void Test()
        {
            //Arrange
            List<string> lines = new();
            using (var stream = new StreamReader(@"C:\Users\User\Desktop\Уник\М1к1с\Евристика\Лб\InputData\A-n32-k5.vrp"))
            {
                lines = stream.ReadToEnd().Split('\n').ToList();
            }

            //Act
            InputData data = InputDataReader.ParseData(lines);

            //Assert
            Assert.That(data, Is.Not.Null);
            Assert.That(data.Dimension, Is.EqualTo(32));
            Assert.That(data.Name, Is.EqualTo("A-n32-k5"));
            Assert.That(data.DistanceMatrix, Is.Not.Null);
            Assert.That(data.DistanceMatrix[0,0], Is.EqualTo(0));
        }
    }
}
