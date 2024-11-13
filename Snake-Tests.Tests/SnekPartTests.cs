using NUnit.Framework;
using SignalR_Snake.Models;
using System.Drawing;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class SnekPartTests
    {
        [Test]
        public void PositionProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snekPart = new SnekPart();
            var expectedPosition = new Point(10, 20);

            // Act
            snekPart.Position = expectedPosition;

            // Assert
            Assert.AreEqual(expectedPosition, snekPart.Position);
            Assert.AreEqual(10, snekPart.Position.X);
            Assert.AreEqual(20, snekPart.Position.Y);
        }

        [Test]
        public void ColorProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snekPart = new SnekPart();
            var expectedColor = "Blue";

            // Act
            snekPart.Color = expectedColor;

            // Assert
            Assert.AreEqual(expectedColor, snekPart.Color);
        }

        [Test]
        public void NameProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snekPart = new SnekPart();
            var expectedName = "SnekHead";

            // Act
            snekPart.Name = expectedName;

            // Assert
            Assert.AreEqual(expectedName, snekPart.Name);
        }

        [Test]
        public void DefaultConstructor_ShouldInitializePropertiesToNullOrDefault()
        {
            // Arrange & Act
            var snekPart = new SnekPart();

            // Assert
            Assert.AreEqual(new Point(0, 0), snekPart.Position);
            Assert.IsNull(snekPart.Color);
            Assert.IsNull(snekPart.Name);
        }
    }
}
