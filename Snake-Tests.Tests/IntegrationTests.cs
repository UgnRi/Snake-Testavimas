using Moq;
using NUnit.Framework;
using SignalR_Snake.Hubs;
using SignalR_Snake.Models;
using Microsoft.AspNet.SignalR.Hubs;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SignalR_Snake.Models.Strategies;

namespace SignalR_Snake.Tests
{
    public interface IScoreClient
    {
        void Score(IEnumerable<SnekScore> scores);
    }

    [TestFixture]
    public class SnakeHubIntegrationTests
    {
        private SnakeHub _snakeHub;
        private Mock<IHubCallerConnectionContext<dynamic>> _mockClients;
        private Mock<HubCallerContext> _mockContext;

        [SetUp]
        public void SetUp()
        {
            _snakeHub = new SnakeHub();
            _mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            _mockContext = new Mock<HubCallerContext>();

            _snakeHub.Clients = _mockClients.Object;
            _snakeHub.Context = _mockContext.Object;
        }

        [Test]
        public void AddSnake_ShouldExistInHubSneksList()
        {
            // Arrange
            string snakeName = "TestSnake";
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");

            // Act
            _snakeHub.NewSnek(snakeName);

            // Assert
            Assert.IsTrue(SnakeHub.Sneks.Exists(s => s.Name == snakeName && s.ConnectionId == "test-connection-id"));
        }

        [Test]
        public void ToggleSpeed_ShouldUpdateMovementStrategy()
        {
            // Arrange
            string snakeName = "TestSnake";
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.Context = _mockContext.Object;

            // Add a new snake with initial Fast = false (NormalMovementStrategy)
            _snakeHub.NewSnek(snakeName);
            var snake = SnakeHub.Sneks.Find(s => s.Name == snakeName);

            // Assert initial movement strategy is NormalMovementStrategy
            Assert.IsInstanceOf<NormalMovementStrategy>(snake.MovementStrategy, "Expected NormalMovementStrategy when Fast is false.");

            // Act - Set Fast to true and call ToggleMovementStrategy explicitly
            snake.Fast = true;
            snake.ToggleMovementStrategy();  // Manually trigger the toggle

            // Assert
            Assert.IsInstanceOf<BoostMovementStrategy>(snake.MovementStrategy, "Expected BoostMovementStrategy when Fast is true.");

            // Act - Set Fast to false and call ToggleMovementStrategy again
            snake.Fast = false;
            snake.ToggleMovementStrategy();  // Manually trigger the toggle

            // Assert
            Assert.IsInstanceOf<NormalMovementStrategy>(snake.MovementStrategy, "Expected NormalMovementStrategy when Fast is false.");
        }



        [Test]
        public void Score_ShouldReturnOrderedScoresByLength()
        {
            // Arrange
            _snakeHub.NewSnek("Snake1");
            _snakeHub.NewSnek("Snake2");

            // Add parts to the first snake to increase its score
            var snake1 = SnakeHub.Sneks.Find(s => s.Name == "Snake1");
            snake1.Parts.Add(new SnekPart()); // Increment length

            // Create a mock for the caller using the IScoreClient interface
            var mockCaller = new Mock<IScoreClient>();
            _mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);

            // Act
            _snakeHub.Score();

            // Assert that the snake with the longer length appears first in ordered scores
            mockCaller.Verify(c => c.Score(It.Is<IEnumerable<SnekScore>>(scores =>
                scores.First().SnakeName == "Snake1" &&
                scores.First().Length == snake1.Parts.Count
            )), Times.Once);
        }

        [Test]
        public void AllPos_ShouldSendAllPositionsToCaller()
        {
            // Arrange
            string snakeName = "TestSnake";
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.NewSnek(snakeName);

            var mockCaller = new Mock<IPositionClient>();
            _mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);

            // Act
            _snakeHub.AllPos();

            // Assert
            mockCaller.Verify(c => c.AllPos(It.IsAny<List<SnekPart>>(), It.IsAny<Point>(), It.IsAny<List<Food>>()), Times.Once);
        }

        [Test]
        public void SendDir_ShouldUpdateSnakeDirection()
        {
            // Arrange
            string snakeName = "TestSnake";
            double newDirection = 90;

            // Set up Context.ConnectionId to simulate a connected client
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.Context = _mockContext.Object;

            // Add a new snake with the specified ConnectionId
            _snakeHub.NewSnek(snakeName);

            // Act
            _snakeHub.SendDir(newDirection);

            // Assert
            var snake = SnakeHub.Sneks.Find(s => s.ConnectionId == "test-connection-id");
            Assert.IsNotNull(snake, "Snake should exist in the Sneks list.");
            Assert.AreEqual(newDirection, snake.Dir);
        }


        [Test]
        public void Speed_ShouldToggleSnakeSpeed()
        {
            // Arrange
            string snakeName = "TestSnake";
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.NewSnek(snakeName);
            var snake = SnakeHub.Sneks.Find(s => s.Name == snakeName);
            bool initialSpeed = snake.Fast;

            // Act
            _snakeHub.Speed(); // Toggle speed

            // Assert
            Assert.AreNotEqual(initialSpeed, snake.Fast);
        }
    }

    // Helper interface for the AllPos method mock
    public interface IPositionClient
    {
        void AllPos(List<SnekPart> snakeParts, Point myPoint, List<Food> foods);
    }
}
