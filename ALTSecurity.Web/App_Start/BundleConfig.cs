using System.Web.Optimization;

namespace ALTSecurity.Web.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/plugins").Include(
                        "~/scripts/jquery-cookie.js",
                        "~/scripts/jquery.serializeToJSON.js",
                        "~/scripts/jquery.loadmask.js"
                        ));

            bundles.Add(new StyleBundle("~/bundles/css/plugins").Include(
                        "~/content/css/jquery.loadmask.css"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                       "~/scripts/jquery.validate.min.js",
                       "~/scripts/jquery.validate.unobtrusive.min.js"
                       ));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/scripts/moment.min.js",
                        "~/scripts/moment-with-locales.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                        "~/scripts/bootstrap.min.js",
                        "~/scripts/umd/popper.min.js"
                        ));

            bundles.Add(new StyleBundle("~/bundles/css/bootstrap").Include(
                        "~/content/bootstrap.min.css"
                ));

            bundles.Add(new StyleBundle("~/content/datatables/media/css/datatables").Include(
                  "~/content/datatables/media/css/jquery.datatables.*",
                  "~/content/datatables/extensions/select/css/select.datatables.*",
                  "~/content/datatables/extensions/fixedheader/css/fixedheader.datatables.*",
                  "~/content/datatables/extensions/responsive/css/responsive.datatables.*",
                  "~/content/datatables/extensions/buttons/css/buttons.datatables.*"
                  ));

            bundles.Add(new StyleBundle("~/content/datatables/media/css/datatables-bs4").Include(
                       "~/content/datatables/media/css/dataTables.bootstrap4.*",
                       "~/content/datatables/extensions/select/css/select.bootstrap4.*",
                       "~/content/datatables/extensions/fixedheader/css/fixedHeader.bootstrap4.*",
                       "~/content/datatables/extensions/responsive/css/responsive.bootstrap4.*",
                       "~/content/datatables/extensions/buttons/css/buttons.bootstrap4.*"
                       ));

            // DataTables
            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                        "~/scripts/datatables/media/js/jquery.datatables*",
                        "~/scripts/datatables/extensions/select/js/datatables.select*",
                        "~/scripts/datatables/extensions/fixedheader/js/datatables.fixedheader*",
                        "~/scripts/datatables/extensions/responsive/js/datatables.responsive*",
                        "~/scripts/datatables/extensions/buttons/js/datatables.buttons*",
                        "~/scripts/datatables/extensions/buttons/js/buttons.colvis*"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/datatables-bs4").Include(
                        "~/scripts/datatables/media/js/jquery.datatables*",
                        "~/scripts/datatables/media/js/dataTables.bootstrap4.*",
                        "~/scripts/datatables/extensions/select/js/datatables.select*",
                        "~/scripts/datatables/extensions/fixedheader/js/datatables.fixedheader*",
                        "~/scripts/datatables/extensions/responsive/js/datatables.responsive*",
                        "~/scripts/datatables/extensions/buttons/js/datatables.buttons*",
                        "~/scripts/datatables/extensions/buttons/js/buttons.colvis*"
                  ));

            // DataTables Editor
            bundles.Add(new ScriptBundle("~/bundles/datatables/editor").Include(
                        "~/scripts/datatables/extensions/keytable/js/datatables.keytable.*",
                        "~/scripts/datatablesplugins/datatables.inlineeditor.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/datatables/exports").Include(
                "~/scripts/datatables/extensions/buttons/js/buttons.flash.*",
                 "~/scripts/datatables/extensions/buttons/js/buttons.html5*",
                  "~/scripts/datatables/extensions/buttons/js/buttons.print*"
                ));

            bundles.Add(new StyleBundle("~/bundles/css/template").Include(
                       "~/content/css/sb-admin-2.min.css",
                       "~/content/css/main.css"
                       ));

            BundleTable.EnableOptimizations = true;
        }
    }
}