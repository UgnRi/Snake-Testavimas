using NUnit.Framework;
using System.Text.RegularExpressions;
using SignalR_Snake.Utilities;

namespace Snake_Tests
{
    public class RandomColorSingletonHelperTests
    {
        [Test]
        public void Instance_ShouldReturnSameInstance()
        {
            // Arrange & Act
            var instance1 = RandomColorSingletonHelper.Instance;
            var instance2 = RandomColorSingletonHelper.Instance;

            // Assert
            Assert.AreSame(instance1, instance2, "Instance method should return the same singleton instance.");
        }

        [Test]
        public void GenerateRandomColor_ShouldReturnValidHexColor()
        {
            // Arrange
            var helper = RandomColorSingletonHelper.Instance;
            var hexColorPattern = @"^#[0-9A-Fa-f]{6}$"; // Regex pattern for a valid hex color

            // Act
            var color = helper.GenerateRandomColor();

            // Assert
            Assert.IsTrue(Regex.IsMatch(color, hexColorPattern),
                $"Generated color '{color}' does not match the hex color format.");
        }

        [Test]
        public void GenerateRandomColor_ShouldGenerateDifferentColors()
        {
            // Arrange
            var helper = RandomColorSingletonHelper.Instance;

            // Act
            var color1 = helper.GenerateRandomColor();
            var color2 = helper.GenerateRandomColor();

            // Assert
            Assert.AreNotEqual(color1, color2,
                "Two consecutive calls to GenerateRandomColor should produce different colors.");
        }

    }
}
