using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using SignalR_Snake.Hubs;
using SignalR_Snake.Models;
using SignalR_Snake.Models.Strategies;
using SignalR_Snake.Models.Observer;
using Microsoft.AspNet.SignalR.Hubs;
using System.Reflection;


namespace Snake_Tests
{
    public class SnakeHubTestsTests
    {
        private SnakeHub _snakeHub;
        private StubHubCallerConnectionContext<dynamic> _stubContext;

        [SetUp]
        public void SetUp()
        {
            // Initialize the SnakeHub and stub context
            _snakeHub = new SnakeHub();
            _stubContext = new StubHubCallerConnectionContext<dynamic>();

            // Use reflection to set the private static clientsStatic field in SnakeHub
            var clientsStaticField = typeof(SnakeHub).GetField("clientsStatic", BindingFlags.NonPublic | BindingFlags.Static);
            clientsStaticField.SetValue(null, _stubContext);  // Set static field to stub context

        }

    }

    public class StubClient
    {
        public void Died()
        {
            // Simulate a no-op for the Died method
        }

        public void AllPos(List<SnekPart> parts, Point myPoint, List<Food> foods)
        {
            // Simulate a no-op for AllPos
        }

        public void Score(List<SnekScore> scores)
        {
            // Simulate a no-op for Score
        }
    }

    public class StubHubConnectionContext<T> : IHubConnectionContext<T>
    {
        public T All { get; private set; }
        public T AllExcept(params string[] excludeConnectionIds) { return default(T); }
        public T Client(string connectionId) { return default(T); }
        public T Clients(IList<string> connectionIds) { return default(T); }
        public T Group(string groupName, params string[] excludeConnectionIds) { return default(T); }
        public T Groups(IList<string> groupNames, params string[] excludeConnectionIds) { return default(T); }
        public T User(string userId) { return default(T); }
        public T Users(IList<string> userIds) { return default(T); }
    }

    public class StubHubCallerConnectionContext<T> : StubHubConnectionContext<T>, IHubCallerConnectionContext<T>
    {
        public List<string> Calls = new List<string>(); // Track method calls
        public T Caller { get; private set; }
        public dynamic CallerState { get; private set; }
        public T Others { get; private set; }

        public T OthersInGroup(string groupName)
        {
            Calls.Add("OthersInGroup");
            return default(T);
        }

        public T OthersInGroups(IList<string> groupNames)
        {
            Calls.Add("OthersInGroups");
            return default(T);
        }

        public void Died()
        {
            Calls.Add("Died");
        }

        public void AllPos(List<SnekPart> parts, Point myPoint, List<Food> foods)
        {
            Calls.Add("AllPos");
        }

        public void Score(List<SnekScore> scores)
        {
            Calls.Add("Score");
        }
    }
}
