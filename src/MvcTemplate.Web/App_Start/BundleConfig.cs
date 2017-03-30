using System.Collections.Generic;
using System.Web.Optimization;

namespace MvcTemplate.Web
{
    public class BundleConfig : IBundleConfig
    {
        public void RegisterBundles(BundleCollection bundles)
        {
            RegisterScripts(bundles);
            RegisterStyles(bundles);
            RegisterOrder(bundles);
        }
        private void RegisterScripts(BundleCollection bundles)
        {
            Bundle privateBundle = new ScriptBundle("~/Scripts/Private/Bundle")
                .Include("~/Scripts/JQuery/jquery.js")
                .IncludeDirectory("~/Scripts/JQuery", "*.js", true)
                .Include("~/Scripts/JQueryUI/jquery-ui.js")
                .IncludeDirectory("~/Scripts/JQueryUI", "*.js", true)
                .IncludeDirectory("~/Scripts/MvcDatalist", "*.js", true)
                .IncludeDirectory("~/Scripts/MvcGrid", "*.js", true)
                .Include("~/Scripts/Bootstrap/*.js")
                .Include("~/Scripts/JsTree/*.js")
                .Include("~/Scripts/Shared/*.js");

            Bundle publicBundle = new ScriptBundle("~/Scripts/Public/Bundle")
                .Include("~/Scripts/JQuery/jquery.js")
                .IncludeDirectory("~/Scripts/JQuery", "*.js", true)
                .Include("~/Scripts/Bootstrap/*.js")
                .Include("~/Scripts/Shared/*.js");

            bundles.Add(privateBundle);
            bundles.Add(publicBundle);
        }
        private void RegisterStyles(BundleCollection bundles)
        {
            Bundle privateBundle = new StyleBundle("~/Content/Private/Bundle")
                .Include("~/Content/JQueryUI/*.css")
                .Include("~/Content/Bootstrap/*.css")
                .Include("~/Content/FontAwesome/*.css")
                .Include("~/Content/MvcGrid/*.css")
                .Include("~/Content/JsTree/*.css")
                .Include("~/Content/MvcDatalist/*.css")
                .IncludeDirectory("~/Content/Shared", "*.css", true);

            Bundle publicBundle = new StyleBundle("~/Content/Public/Bundle")
                .Include("~/Content/Bootstrap/*.css")
                .Include("~/Content/FontAwesome/*.css")
                .IncludeDirectory("~/Content/Shared", "*.css", true);

            bundles.Add(privateBundle);
            bundles.Add(publicBundle);
        }
        private void RegisterOrder(BundleCollection bundles)
        {
            foreach (Bundle bundle in bundles)
                bundle.Orderer = new NaturalOrder();
        }

        private class NaturalOrder : IBundleOrderer
        {
            public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
            {
                return files;
            }
        }
    }
}
