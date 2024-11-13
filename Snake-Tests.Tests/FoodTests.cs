using NUnit.Framework;
using SignalR_Snake.Models;
using System.Drawing;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class FoodTests
    {
        [Test]
        public void PositionProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var food = new Food();
            var expectedPosition = new Point(50, 100);

            // Act
            food.Position = expectedPosition;

            // Assert
            Assert.AreEqual(expectedPosition, food.Position);
            Assert.AreEqual(50, food.Position.X);
            Assert.AreEqual(100, food.Position.Y);
        }

        [Test]
        public void ColorProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var food = new Food();
            var expectedColor = "Red";

            // Act
            food.Color = expectedColor;

            // Assert
            Assert.AreEqual(expectedColor, food.Color);
        }

        [Test]
        public void DefaultConstructor_ShouldInitializePropertiesToNullOrDefault()
        {
            // Arrange & Act
            var food = new Food();

            // Assert
            Assert.IsNull(food.Color);
            Assert.AreEqual(new Point(0, 0), food.Position);
        }
    }
}
