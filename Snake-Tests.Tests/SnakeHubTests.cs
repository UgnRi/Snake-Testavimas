using Moq;
using NUnit.Framework;
using SignalR_Snake.Hubs;
using SignalR_Snake.Models;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using SignalR_Snake.Models.Observer;
using System.Drawing;

namespace SnakeHubTests
{
    [TestFixture]
    public class SnakeHubTests
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

        [TearDown]
        public void TearDown()
        {
            SnakeHub.Sneks.Clear();
            SnakeHub.Foods.Clear();
        }


        [Test]
        public void NewSnek_ShouldAddNewSnake()
        {
            // Arrange
            string snakeName = "TestSnake";
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");

            // Act
            _snakeHub.NewSnek(snakeName);

            // Assert
            Assert.AreEqual(1, SnakeHub.Sneks.Count);
            Assert.AreEqual(snakeName, SnakeHub.Sneks[0].Name);
            Assert.AreEqual("test-connection-id", SnakeHub.Sneks[0].ConnectionId);
        }

        [Test]
        public void RegisterObserver_ShouldAddObserver()
        {
            // Arrange
            var observerMock = new Mock<ISnakeObserver>();

            // Act
            _snakeHub.RegisterObserver(observerMock.Object);

            // Assert
            Assert.Contains(observerMock.Object, _snakeHub.observers);
        }

        [Test]
        public void RemoveObserver_ShouldRemoveObserver()
        {
            // Arrange
            var observerMock = new Mock<ISnakeObserver>();
            _snakeHub.RegisterObserver(observerMock.Object);

            // Act
            _snakeHub.RemoveObserver(observerMock.Object);

            // Assert
            Assert.IsFalse(_snakeHub.observers.Contains(observerMock.Object));
        }

        [Test]
        public void NotifySnakeUpdated_ShouldNotifyAllObservers()
        {
            // Arrange
            var observerMock = new Mock<ISnakeObserver>();
            _snakeHub.RegisterObserver(observerMock.Object);
            var snake = new Snake { Name = "TestSnake" };

            // Act
            _snakeHub.NotifySnakeUpdated(snake);

            // Assert
            observerMock.Verify(o => o.OnSnakeUpdated(snake), Times.Once);
        }

        public interface IPositionClient
        {
            void AllPos(List<SnekPart> snakeParts, Point myPoint, List<Food> foods);
        }

        [Test]
        public void AllPos_ShouldSendAllPositionsToCaller()
        {
            // Arrange
            string snakeName = "TestSnake";

            // Set up Context.ConnectionId
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.Context = _mockContext.Object;

            // Add a new snake
            _snakeHub.NewSnek(snakeName);

            // Set up Clients.Caller mock
            var mockCaller = new Mock<IPositionClient>();
            _mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);
            _snakeHub.Clients = _mockClients.Object;

            // Act
            _snakeHub.AllPos();

            // Assert
            mockCaller.Verify(c => c.AllPos(It.IsAny<List<SnekPart>>(), It.IsAny<Point>(), It.IsAny<List<Food>>()), Times.Once);
        }

        //5
        //[Test]
        //public void Score_ShouldSendOrderedScoresToCaller()
        //{
        //    // Arrange
        //    _snakeHub.NewSnek("Snake1");
        //    _snakeHub.NewSnek("Snake2");
        //    SnakeHub.Sneks[0].Parts.Add(new SnekPart());
        //    var mockCaller = new Mock<dynamic>();
        //    _mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);
        //
        //    // Act
        //    _snakeHub.Score();
        //
        //    // Assert
        //    mockCaller.Verify(c => c.Score(It.IsAny<IEnumerable<SnekScore>>()), Times.Once);
        //}

        //6
        [Test]
        public void SendDir_ShouldUpdateSnakeDirection()
        {
            // Arrange
            string snakeName = "TestSnake";
            double newDirection = 90;

            // Set up Context.ConnectionId
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.Context = _mockContext.Object;

            // Add a new snake
            _snakeHub.NewSnek(snakeName);

            // Act
            _snakeHub.SendDir(newDirection);

            // Assert
            Assert.AreEqual(newDirection, SnakeHub.Sneks[0].Dir);
        }


        [Test]
        public void Speed_ShouldToggleSnakeSpeed()
        {
            // Arrange
            string snakeName = "TestSnake";

            // Set up Context.ConnectionId
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id");
            _snakeHub.Context = _mockContext.Object;

            // Add a new snake
            _snakeHub.NewSnek(snakeName);

            // Store the initial speed state
            bool initialSpeed = SnakeHub.Sneks[0].Fast;

            // Act
            _snakeHub.Speed();  // This should toggle the speed

            // Assert
            Assert.AreNotEqual(initialSpeed, SnakeHub.Sneks[0].Fast);
        }


        //8
        public interface IScoreClient
        {
            void Score(IEnumerable<SnekScore> scores);
        }

        [Test]
        public void Score_ShouldSendOrderedScoresToCaller()
        {
            // Arrange
            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id-1");
            _snakeHub.Context = _mockContext.Object;

            _snakeHub.NewSnek("Snake1");

            _mockContext.Setup(c => c.ConnectionId).Returns("test-connection-id-2");
            _snakeHub.Context = _mockContext.Object;

            _snakeHub.NewSnek("Snake2");

            SnakeHub.Sneks[0].Parts.Add(new SnekPart());  // Increase the score of the first snake

            // Create a mock for the caller
            var mockCaller = new Mock<IScoreClient>();
            _mockClients.Setup(clients => clients.Caller).Returns(mockCaller.Object);
            _snakeHub.Clients = _mockClients.Object;

            // Act
            _snakeHub.Score();

            // Assert
            mockCaller.Verify(c => c.Score(It.IsAny<IEnumerable<SnekScore>>()), Times.Once);
        }


    }
}



