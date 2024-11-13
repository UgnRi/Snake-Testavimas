using NUnit.Framework;
using SignalR_Snake.Models;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class SnekScoreTests
    {
        [Test]
        public void SnakeNameProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snekScore = new SnekScore();
            var expectedName = "TestSnake";

            // Act
            snekScore.SnakeName = expectedName;

            // Assert
            Assert.AreEqual(expectedName, snekScore.SnakeName);
        }

        [Test]
        public void LengthProperty_ShouldSetAndGetCorrectly()
        {
            // Arrange
            var snekScore = new SnekScore();
            var expectedLength = 5;

            // Act
            snekScore.Length = expectedLength;

            // Assert
            Assert.AreEqual(expectedLength, snekScore.Length);
        }

        [Test]
        public void DefaultConstructor_ShouldInitializePropertiesToNullOrDefault()
        {
            // Arrange & Act
            var snekScore = new SnekScore();

            // Assert
            Assert.IsNull(snekScore.SnakeName);
            Assert.AreEqual(0, snekScore.Length);
        }
    }
}
