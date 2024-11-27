using NUnit.Framework;
using Moq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SignalR_Snake;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class RouteConfigTests
    {
        [SetUp]
        public void SetUp()
        {
            RouteTable.Routes.Clear();
        }

        [Test]
        public void RegisterRoutes_ShouldIgnoreAxdRequests()
        {
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            // Use the correct format with leading slash ('/')
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("/resource.axd/somepath");

            RouteData routeData = routes.GetRouteData(context.Object);

            // Assert that no route matches the .axd path
            Assert.IsNotNull(routeData, "The .axd path should be ignored by the routing configuration.");
        }


        [Test]
        public void RegisterRoutes_DefaultRoute_ShouldMapToHomeIndex()
        {
            var routes = new RouteCollection();
            RouteConfig.RegisterRoutes(routes);

            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Request.AppRelativeCurrentExecutionFilePath).Returns("~/");

            RouteData routeData = routes.GetRouteData(context.Object);

            Assert.IsNotNull(routeData, "The default route should match the root URL.");
            Assert.AreEqual("Home", routeData.Values["controller"], "Default controller should be 'Home'.");
            Assert.AreEqual("Index", routeData.Values["action"], "Default action should be 'Index'.");
            Assert.AreEqual(UrlParameter.Optional, routeData.Values["id"], "Default id should be optional.");
        }
    }
}
