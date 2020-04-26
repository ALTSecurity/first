using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using Microsoft.Ajax.Utilities;

namespace ALTSecurity.Web.Utility
{
    public static class OptimizationExtensions
    {
        /// <summary>
        /// Динамическая минификация js для Razor view
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="markup"></param>
        /// <returns></returns>
        public static MvcHtmlString JsMinify (this HtmlHelper helper, Func<object, object> markup)
        {
            if(helper == null || markup == null)
            {
                return MvcHtmlString.Empty;
            }
            
            string source = markup.DynamicInvoke(helper.ViewContext)?.ToString() ?? string.Empty;

            if (!BundleTable.EnableOptimizations)
            {
                return new MvcHtmlString(source);
            }

            Minifier minifier = new Minifier();
            string result = minifier.MinifyJavaScript(source, new CodeSettings
            {
                EvalTreatment = EvalTreatment.MakeImmediateSafe,
                PreserveImportantComments = false
            });

            return new MvcHtmlString(result);
        }


        /// <summary>
        /// Динамическая минификция css для Razor View
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="markup"></param>
        /// <returns></returns>
        public static MvcHtmlString CssMinify (this HtmlHelper helper, Func<object,object> markup)
        {
            if (helper == null || markup == null)
            {
                return MvcHtmlString.Empty;
            }

            string source = markup.DynamicInvoke(helper.ViewContext)?.ToString() ?? string.Empty;

            if (!BundleTable.EnableOptimizations)
            {
                return new MvcHtmlString(source);
            }

            Minifier minifier = new Minifier();
            string result = minifier.MinifyStyleSheet(source, new CssSettings
            {
                CommentMode = CssComment.None
            });

            return new MvcHtmlString(result);
        }
    }
}