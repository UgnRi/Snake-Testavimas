using NUnit.Framework;
using System.Collections.Generic;
using SignalR_Snake.Models.Observer;
using SignalR_Snake.Models;

namespace Snake_Tests
{
    public class ObserverTests
    {


        // Concrete implementation of the Subject (the hub)
        public class SnakeHub : ISnakeSubject
        {
            private List<ISnakeObserver> observers = new List<ISnakeObserver>();

            public void RegisterObserver(ISnakeObserver observer)
            {
                observers.Add(observer);
            }

            public void RemoveObserver(ISnakeObserver observer)
            {
                observers.Remove(observer);
            }

            public void NotifySnakeUpdated(SignalR_Snake.Models.Snake snake)
            {
                foreach (var observer in observers)
                {
                    observer.OnSnakeUpdated(snake);
                }
            }

            public void NotifySnakeDied(SignalR_Snake.Models.Snake snake)
            {
                foreach (var observer in observers)
                {
                    observer.OnSnakeDied(snake);
                }
            }
        }

        // Concrete implementation of an observer
        public class SnakeObserver : ISnakeObserver
        {
            public bool WasSnakeUpdatedCalled { get; private set; }
            public bool WasSnakeDiedCalled { get; private set; }

            public void Reset()
            {
                WasSnakeUpdatedCalled = false;
                WasSnakeDiedCalled = false;
            }

            void ISnakeObserver.OnSnakeUpdated(SignalR_Snake.Models.Snake snake)
            {
                WasSnakeUpdatedCalled = true;
            }

            void ISnakeObserver.OnSnakeDied(SignalR_Snake.Models.Snake snake)
            {
                WasSnakeDiedCalled = true;
            }
        }

        // Test class
        [TestFixture]
        public class SnakeHubTests
        {
            private SnakeHub _snakeHub;
            private SnakeObserver _observer;

            [SetUp]
            public void SetUp()
            {
                _snakeHub = new SnakeHub();
                _observer = new SnakeObserver();
            }

            [Test]
            public void RegisterObserver_ShouldNotifyOnSnakeUpdated()
            {
                // Arrange
                var snake = new Snake { Name = "Snek"};
                _snakeHub.RegisterObserver(_observer);

                // Act
                _snakeHub.NotifySnakeUpdated(snake);

                // Assert
                Assert.IsTrue(_observer.WasSnakeUpdatedCalled);
            }

            [Test]
            public void RegisterObserver_ShouldNotifyOnSnakeDied()
            {
                // Arrange
                var snake = new Snake { Name = "Snek"}; // Death scenario
                _snakeHub.RegisterObserver(_observer);

                // Act
                _snakeHub.NotifySnakeDied(snake);

                // Assert
                Assert.IsTrue(_observer.WasSnakeDiedCalled);
            }

            [Test]
            public void RemoveObserver_ShouldNotNotifyAfterRemoval()
            {
                // Arrange
                var snake = new Snake { Name = "Snek"};
                _snakeHub.RegisterObserver(_observer);
                _snakeHub.RemoveObserver(_observer);

                // Act
                _snakeHub.NotifySnakeUpdated(snake);

                // Assert
                Assert.IsFalse(_observer.WasSnakeUpdatedCalled);
            }

            [Test]
            public void NotifySnakeUpdated_ShouldNotifyAllObservers()
            {
                // Arrange
                var snake = new Snake { Name = "Snek" };
                var observer2 = new SnakeObserver();
                _snakeHub.RegisterObserver(_observer);
                _snakeHub.RegisterObserver(observer2);

                // Act
                _snakeHub.NotifySnakeUpdated(snake);

                // Assert
                Assert.IsTrue(_observer.WasSnakeUpdatedCalled);
                Assert.IsTrue(observer2.WasSnakeUpdatedCalled);
            }

            [Test]
            public void NotifySnakeDied_ShouldNotifyAllObservers()
            {
                // Arrange
                var snake = new Snake { Name = "Snek" }; // Death scenario
                var observer2 = new SnakeObserver();
                _snakeHub.RegisterObserver(_observer);
                _snakeHub.RegisterObserver(observer2);

                // Act
                _snakeHub.NotifySnakeDied(snake);

                // Assert
                Assert.IsTrue(_observer.WasSnakeDiedCalled);
                Assert.IsTrue(observer2.WasSnakeDiedCalled);
            }
        }
    }
}
