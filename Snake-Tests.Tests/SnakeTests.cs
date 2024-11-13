using NUnit.Framework;
using SignalR_Snake.Models;
using SignalR_Snake.Models.Strategies;
using System.Collections.Generic;
using System.Drawing;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class SnakeTests
    {
        [Test]
        public void Constructor_ShouldInitializePropertiesWithDefaultValues()
        {
            // Arrange & Act
            var snake = new Snake();

            // Assert
            Assert.AreEqual(10, snake.Width);
            Assert.IsFalse(snake.Fast);
            Assert.AreEqual(4, snake.Speed);
            Assert.AreEqual(5, snake.Dir);
            Assert.AreEqual(8, snake.SpeedTwo);
            Assert.IsInstanceOf<NormalMovementStrategy>(snake.MovementStrategy);
        }

        [Test]
        public void NameProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snake = new Snake();
            var expectedName = "TestSnake";

            // Act
            snake.Name = expectedName;

            // Assert
            Assert.AreEqual(expectedName, snake.Name);
        }

        [Test]
        public void ConnectionIdProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snake = new Snake();
            var expectedConnectionId = "test-connection-id";

            // Act
            snake.ConnectionId = expectedConnectionId;

            // Assert
            Assert.AreEqual(expectedConnectionId, snake.ConnectionId);
        }

        [Test]
        public void ColorProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snake = new Snake();
            var expectedColor = "Green";

            // Act
            snake.Color = expectedColor;

            // Assert
            Assert.AreEqual(expectedColor, snake.Color);
        }

        [Test]
        public void PartsProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snake = new Snake();
            var parts = new List<SnekPart> { new SnekPart { Position = new Point(0, 0), Color = "Green" } };

            // Act
            snake.Parts = parts;

            // Assert
            Assert.AreEqual(parts, snake.Parts);
            Assert.AreEqual(1, snake.Parts.Count);
            Assert.AreEqual("Green", snake.Parts[0].Color);
        }

        [Test]
        public void ToggleMovementStrategy_ShouldSwitchToBoostMovementStrategy_WhenFastIsTrue()
        {
            // Arrange
            var snake = new Snake();
            snake.Fast = true;

            // Act
            snake.ToggleMovementStrategy();

            // Assert
            Assert.IsInstanceOf<BoostMovementStrategy>(snake.MovementStrategy);
        }

        [Test]
        public void ToggleMovementStrategy_ShouldSwitchToNormalMovementStrategy_WhenFastIsFalse()
        {
            // Arrange
            var snake = new Snake();
            snake.Fast = false;

            // Act
            snake.ToggleMovementStrategy();

            // Assert
            Assert.IsInstanceOf<NormalMovementStrategy>(snake.MovementStrategy);
        }

        [Test]
        public void ToggleMovementStrategy_ShouldRetainBoostMovementStrategy_WhenCalledMultipleTimesAndFastIsTrue()
        {
            // Arrange
            var snake = new Snake();
            snake.Fast = true;

            // Act
            snake.ToggleMovementStrategy();
            snake.ToggleMovementStrategy();  // Toggle again

            // Assert
            Assert.IsInstanceOf<BoostMovementStrategy>(snake.MovementStrategy);
        }

        [Test]
        public void ToggleMovementStrategy_ShouldSwitchBackToNormal_WhenFastToggledToFalse()
        {
            // Arrange
            var snake = new Snake();
            snake.Fast = true;

            // Act
            snake.ToggleMovementStrategy();  // Should set to BoostMovementStrategy
            snake.Fast = false;
            snake.ToggleMovementStrategy();  // Should set back to NormalMovementStrategy

            // Assert
            Assert.IsInstanceOf<NormalMovementStrategy>(snake.MovementStrategy);
        }
    }
}
