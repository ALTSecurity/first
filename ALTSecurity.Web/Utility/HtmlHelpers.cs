using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;

namespace ALTSecurity.Web.Utility
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Преобразование в булевое значение для js
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MvcHtmlString JsBool(this HtmlHelper helper, bool value)
        {
            return new MvcHtmlString(value ? "true" : "false");
        }


        /// <summary>
        /// Построение модального окна на базе Bootstrap4
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="body"></param>
        /// <param name="size"></param>
        /// <param name="buttons"></param>
        /// <param name="scrollable"></param>
        /// <returns></returns>
        public static MvcHtmlString ModalWindow(this HtmlHelper helper, string id, string title, Func<object, object> body, ModalWindowSize size = ModalWindowSize.Medium, 
            ModalWindowButtons buttons = ModalWindowButtons.OkCancel, bool scrollable = false)
        {
            if (helper == null || body == null)
            {
                return MvcHtmlString.Empty;
            }

            TagBuilder container = new TagBuilder("div");
            container.MergeAttribute("id", id);
            container.MergeAttribute("tabindex", "-1");
            container.MergeAttribute("role", "dialog");
            container.MergeAttribute("aria-labelledby", id);
            container.MergeAttribute("aria-hidden", "true");

            container.AddCssClass("modal fade");

            StringBuilder markup = new StringBuilder();

            string dialogClass = string.Empty;
            switch (size)
            {
                case ModalWindowSize.Small:
                    dialogClass += "modal-dialog modal-sm";
                    break;
                case ModalWindowSize.Large:
                    dialogClass += "modal-dialog modal-lg";
                    break;
                case ModalWindowSize.ExtraLarge:
                    dialogClass += "modal-dialog modal-xl";
                    break;
                default:
                    break;
            }

            if (scrollable)
            {
                dialogClass += " modal-dialog-scrollable";
            }

            markup.AppendLine($"<div class=\"modal-dialog { dialogClass }\" role=\"document\">");
            markup.AppendLine($"<div class=\"modal-content\">");

            markup.AppendLine($"<div class=\"modal-header\">");
            markup.AppendLine($"<h5 class=\"modal-title\" id=\"{ id + "Label" }\"> { title } </h5>");
            markup.AppendLine("<button type=\"button\" class=\"close\" data-dismiss=\"modal\" aria-label=\"Close\">");
            markup.AppendLine("<span aria-hidden=\"true\">&times;</span>");
            markup.AppendLine("</button>");
            markup.AppendLine("</div>");

            string bodyContent = body.DynamicInvoke(helper.ViewContext)?.ToString() ?? string.Empty;
            markup.AppendLine("<div class=\"modal-body\">");
            markup.AppendLine(bodyContent);
            markup.AppendLine("</div>");

            string footerContent = string.Empty;
            switch (buttons)
            {
                case ModalWindowButtons.Ok:
                    footerContent += $"<button type=\"button\" data-result=\" { (int)ModalResult.ok } \" class=\"btn btn-primary\"> { Resources.Global.ok } </button>";
                    break;
                case ModalWindowButtons.Cancel:
                    footerContent += $"<button type=\"button\" data-result=\" { (int)ModalResult.cancel } \" class=\"btn btn-primary\"> { Resources.Global.close } </button>";
                    break;
                case ModalWindowButtons.OkCancel:
                    footerContent += $"<button type=\"button\" data-result=\" { (int)ModalResult.ok } \" class=\"btn btn-primary\"> { Resources.Global.ok } </button>";
                    footerContent += $"<button type=\"button\" data-result=\" { (int)ModalResult.cancel } \" class=\"btn btn-secondary\"> { Resources.Global.close } </button>";
                    break;
                default:
                    break;
            }

            markup.AppendLine($"<div class=\"modal-footer\">");
            markup.AppendLine(footerContent);
            markup.AppendLine("</div></div>");

            container.InnerHtml = markup.ToString();

            return new MvcHtmlString(container.ToString(TagRenderMode.Normal));
        }
    }
}

public enum ModalWindowSize
{
    Small = 0,
    Medium = 1,
    Large = 2,
    ExtraLarge = 3
}

public enum ModalWindowButtons
{
    None = 0,
    Ok = 1,
    Cancel = 2,
    OkCancel = 3
}

public enum ModalResult
{
    ok = 0,
    cancel = 1
}