using NUnit.Framework;
using System.Web.Mvc;
using System.Linq;
using SignalR_Snake;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class FilterConfigTests
    {
        [Test]
        public void RegisterGlobalFilters_AddsHandleErrorAttribute()
        {
            var filters = new GlobalFilterCollection();

            FilterConfig.RegisterGlobalFilters(filters);

            Assert.That(filters.Count, Is.EqualTo(1), "The filters collection should contain exactly one filter.");
            Assert.That(filters.Any(f => f.Instance is HandleErrorAttribute), "The filter should be of type HandleErrorAttribute.");
        }
    }
}
