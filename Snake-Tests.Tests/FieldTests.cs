using NUnit.Framework;
using SignalR_Snake.Models;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class FieldTests
    {
        [Test]
        public void DefaultConstructor_ShouldInitializeWidthTo3000()
        {
            // Arrange & Act
            var field = new Field();

            // Assert
            Assert.AreEqual(3000, field.Width);
        }

        [Test]
        public void DefaultConstructor_ShouldInitializeHeightTo3000()
        {
            // Arrange & Act
            var field = new Field();

            // Assert
            Assert.AreEqual(3000, field.Height);
        }

        [Test]
        public void Width_ShouldAllowSettingAndGettingValue()
        {
            // Arrange
            var field = new Field();
            var newWidth = 4000;

            // Act
            field.Width = newWidth;

            // Assert
            Assert.AreEqual(newWidth, field.Width);
        }

        [Test]
        public void Height_ShouldAllowSettingAndGettingValue()
        {
            // Arrange
            var field = new Field();
            var newHeight = 4000;

            // Act
            field.Height = newHeight;

            // Assert
            Assert.AreEqual(newHeight, field.Height);
        }
    }
}
