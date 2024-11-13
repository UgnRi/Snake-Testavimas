using System.Web.Mvc;
using NUnit.Framework;
using SignalR_Snake.Controllers;
using SignalR_Snake.Models;

namespace Snake_Tests
{
    public class HomeControllerTests
    {

        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new HomeController();
        }

        [Test]
        public void Index_Get_Returns_View()
        {
            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);  
        }


    }

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");  // Explicitly specify the view name
        }

        [HttpPost]
        public ActionResult Index(Snake snake)
        {
            return RedirectToAction("Snek", "Home", snake);
        }

        public ActionResult Snek(Snake snake)
        {
            return View(snake);  // Returning the snake model to the Snek view
        }
    }
}
