using MvcTemplate.Web;
using System;
using System.Web.Optimization;
using Xunit;
using Xunit.Extensions;

namespace MvcTemplate.Tests.Unit.Web
{
    public class BundleConfigTests
    {
        #region RegisterBundles(BundleCollection bundles)

        [Theory]
        [InlineData("~/Scripts/Public/Bundle")]
        [InlineData("~/Scripts/Private/Bundle")]
        public void RegisterBundles_ForScripts(String path)
        {
            BundleCollection bundles = new BundleCollection();

            new BundleConfig().RegisterBundles(bundles);

            Assert.IsType<ScriptBundle>(bundles.GetBundleFor(path));
        }

        [Theory]
        [InlineData("~/Content/Public/Bundle")]
        [InlineData("~/Content/Private/Bundle")]
        public void RegisterBundles_ForStyles(String path)
        {
            BundleCollection bundles = new BundleCollection();

            new BundleConfig().RegisterBundles(bundles);

            Assert.IsType<StyleBundle>(bundles.GetBundleFor(path));
        }

        #endregion
    }
}
