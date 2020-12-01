using System.Web;
using System.Web.Optimization;

namespace DawtNetProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/create-article").Include(
                        "~/Scripts/create-article.js"));
            bundles.Add(new ScriptBundle("~/bundles/set-version").Include(
                        "~/Scripts/set-version.js"));
            bundles.Add(new ScriptBundle("~/bundles/article-list").Include(
                        "~/Scripts/article-list.js"));
            bundles.Add(new ScriptBundle("~/bundles/extract-chapters").Include(
                        "~/Scripts/extract-chapters.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/site.css",
                      "~/Content/ArticleList.css"));
                      //"~/Content/sakura.css"));
        }
    }
}
