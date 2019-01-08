using System.Web.Optimization;

namespace Win8Apps.App_Start
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new Bundle("~/js").Include(
                "~/Scripts/jquery-1.9.0.js",
                "~/scripts/jquery.share.js",
                "~/scripts/jquery.mousewheel.js",
                "~/scripts/amcharts.js"

                ));
            bundles.Add(new Bundle("~/js/modern").Include(
                "~/Scripts/modern/*.js"
                ));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/modern.css",
                "~/Content/modern-responsive.css",
                "~/Content/jquery.share.css",
                "~/Content/site.css"
                ));
            //bundles.Add(new StyleBundle("~/content/css-responsive").Include(
            //    //"~/Content/bootstrap-responsive.css",
            //    //"~/Content/modern-responsive.css"
            //    ));
        }
    }
}