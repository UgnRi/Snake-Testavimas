using NUnit.Framework;
using System.Web.Optimization;
using SignalR_Snake;
using System.Linq;

namespace SignalR_Snake.Tests
{
    [TestFixture]
    public class BundleConfigTests
    {
        [SetUp]
        public void SetUp()
        {
            BundleTable.Bundles.Clear(); // Clear any previous bundles to ensure test isolation
        }

        [Test]
        public void RegisterBundles_ShouldAddJQueryBundle()
        {
            var bundles = new BundleCollection();

            BundleConfig.RegisterBundles(bundles);

            var jqueryBundle = bundles.GetBundleFor("~/bundles/jquery");
            Assert.IsNotNull(jqueryBundle, "jQuery bundle should be registered.");
            Assert.IsTrue(jqueryBundle is ScriptBundle, "jQuery bundle should be a ScriptBundle.");
            Assert.IsTrue(jqueryBundle.Transforms.Any(transform => transform is JsMinify), "jQuery bundle should use JsMinify.");
        }

        [Test]
        public void RegisterBundles_ShouldAddJQueryValidationBundle()
        {
            var bundles = new BundleCollection();

            BundleConfig.RegisterBundles(bundles);

            var jqueryValBundle = bundles.GetBundleFor("~/bundles/jqueryval");
            Assert.IsNotNull(jqueryValBundle, "jQuery validation bundle should be registered.");
            Assert.IsTrue(jqueryValBundle is ScriptBundle, "jQuery validation bundle should be a ScriptBundle.");
            Assert.IsTrue(jqueryValBundle.Transforms.Any(transform => transform is JsMinify), "jQuery validation bundle should use JsMinify.");
        }

        [Test]
        public void RegisterBundles_ShouldAddModernizrBundle()
        {
            var bundles = new BundleCollection();

            BundleConfig.RegisterBundles(bundles);

            var modernizrBundle = bundles.GetBundleFor("~/bundles/modernizr");
            Assert.IsNotNull(modernizrBundle, "Modernizr bundle should be registered.");
            Assert.IsTrue(modernizrBundle is ScriptBundle, "Modernizr bundle should be a ScriptBundle.");
        }

        [Test]
        public void RegisterBundles_ShouldAddBootstrapBundle()
        {
            var bundles = new BundleCollection();

            BundleConfig.RegisterBundles(bundles);

            var bootstrapBundle = bundles.GetBundleFor("~/bundles/bootstrap");
            Assert.IsNotNull(bootstrapBundle, "Bootstrap bundle should be registered.");
            Assert.IsTrue(bootstrapBundle is ScriptBundle, "Bootstrap bundle should be a ScriptBundle.");
        }

        [Test]
        public void RegisterBundles_ShouldAddCssBundle()
        {
            var bundles = new BundleCollection();

            BundleConfig.RegisterBundles(bundles);

            var cssBundle = bundles.GetBundleFor("~/Content/css");
            Assert.IsNotNull(cssBundle, "CSS bundle should be registered.");
            Assert.IsTrue(cssBundle is StyleBundle, "CSS bundle should be a StyleBundle.");
        }
    }
}
