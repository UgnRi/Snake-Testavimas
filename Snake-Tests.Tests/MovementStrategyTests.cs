using NUnit.Framework;
using SignalR_Snake.Models.Strategies;
using System.Drawing;

namespace Snake_Tests
{
    [TestFixture]
    public class MovementStrategyTests
    {
        [Test]
        [TestCase(0, 10, 10, 0)]
        [TestCase(90, 10, 0, 10)]
        [TestCase(180, 10, -10, 0)]
        [TestCase(270, 10, 0, -10)]
        [TestCase(45, 10, 7, 7)]
        public void NormalMovementStrategy_Move_ShouldUpdatePosition(double direction, int speed, int expectedX, int expectedY)
        {
            var normalMovementStrategy = new NormalMovementStrategy();
            var startPosition = new Point(0, 0);
            Point result = normalMovementStrategy.Move(startPosition, direction, speed);
            Assert.AreEqual(new Point(expectedX, expectedY), result, $"The position should move as expected for direction {direction} degrees and speed {speed}.");
        }

        [Test]
        [TestCase(0, 10, 20, 0)]
        [TestCase(90, 10, 0, 20)]
        [TestCase(180, 10, -20, 0)]
        [TestCase(270, 10, 0, -20)]
        [TestCase(45, 10, 14, 14)]
        public void BoostMovementStrategy_Move_ShouldDoubleTheDistance(double direction, int speed, int expectedX, int expectedY)
        {
            var boostMovementStrategy = new BoostMovementStrategy();
            var startPosition = new Point(0, 0);
            Point result = boostMovementStrategy.Move(startPosition, direction, speed);
            Assert.AreEqual(new Point(expectedX, expectedY), result, $"The position should move twice the distance as expected for direction {direction} degrees and speed {speed}.");
        }

        [Test]
        public void NormalMovementStrategy_Move_WithNegativeSpeed_ShouldMoveInOppositeDirection()
        {
            var normalMovementStrategy = new NormalMovementStrategy();
            var startPosition = new Point(0, 0);
            double direction = 0;
            int speed = -10;
            Point result = normalMovementStrategy.Move(startPosition, direction, speed);
            Assert.AreEqual(new Point(-10, 0), result, "The position should move in the opposite direction due to negative speed.");
        }

        [Test]
        public void BoostMovementStrategy_Move_WithZeroSpeed_ShouldNotMove()
        {
            var boostMovementStrategy = new BoostMovementStrategy();
            var startPosition = new Point(0, 0);
            double direction = 90;
            int speed = 0;
            Point result = boostMovementStrategy.Move(startPosition, direction, speed);
            Assert.AreEqual(new Point(0, 0), result, "The position should not change when speed is zero.");
        }
    }
}