using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace ALTSecurity.Web.Utility
{
    public class DataTables
    {
        public string Id { get; set; }

        public string Dom { get; set; }

        public string ClassName { get; set; }

        public object Data { get; set; }

        public List<DataTableColumn> Columns { get; set; }

        public List<DataTableButton> Buttons { get; set; }

        public DataTablesOrdering Order { get; set; }

        public bool Searching { get; set; }

        public bool Select { get; set; }

        public DataTableSelection Selection { get; set; }

        public string RowSelect { get; set; }


        public DataTables(string id)
        {
            Id = id;
        }
    }


    public class DataTableColumn
    {
        public string title { get; set; }

        public string className { get; set; }

        public bool orderable { get; set; } = true;

        public string data { get; set; }

        public string render { get; set; }

        public bool visible { get; set; } = true;

    }

    public class DataTableButton
    {
        public string className { get; set; }

        public string text { get; set; }

        public string action { get; set; }
    }

    public class DataTableChecbox
    {
        public bool selectRow { get; set; }
    }

    public class DataTableSelection
    {
        public SelectionStyle Style { get; set; }

        public string selector { get; set; }
    }


    public class DataTablesOrdering
    {
        public int Column { get; set; }

        public OrderingType Type { get; set; }
    }


    public enum SelectionStyle
    {
        single = 0,
        multi = 1,
        checkbox = 2,
        os = 3
    }

    public enum OrderingType
    {
        desc = 0,
        asc = 1
    }


    public static class DataTablesExtension
    {
        public static IHtmlString DataTables(this HtmlHelper helper, DataTables dataTable)
        {
            StringBuilder dataTablesBuilder = new StringBuilder();

            if(dataTable.Select && dataTable.Selection.Style == SelectionStyle.checkbox)
            {
                dataTable.Columns.Insert(0, new DataTableColumn
                {
                    className = "select-checkbox",
                    orderable = false

                });

                dataTable.Selection.Style = SelectionStyle.os;
                dataTable.Selection.selector = "td:first-child";
            }

            if (dataTable != null)
            {
                dataTablesBuilder.Append("<script>");

                dataTablesBuilder.Append(@"window." + dataTable.Id + " =  $('#" + dataTable.Id + "').DataTable({");
                dataTablesBuilder.Append("dom:'" + dataTable.Dom + "',");

                //ordering
                if (dataTable.Order != null)
                {
                    dataTablesBuilder.Append("order:[");
                    dataTablesBuilder.Append("[" + dataTable.Order.Column + ",");
                    dataTablesBuilder.Append(@"'" + (dataTable.Order.Type == OrderingType.desc ? "desc" : "asc") + "']");
                    dataTablesBuilder.Append("],");
                }

                //columns
                if (dataTable.Columns != null && dataTable.Columns.Count > 0)
                {
                    dataTablesBuilder.Append("columns:[");
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var column = dataTable.Columns.ElementAt(i);

                        dataTablesBuilder.Append("{");
                        if (!string.IsNullOrEmpty(column.title))
                        {
                            dataTablesBuilder.Append("\"title\":'" + column.title + "'");
                        }
                        if (!string.IsNullOrEmpty(column.data))
                        {
                            dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"data\":'" + column.data + "'");
                        }
                        if (!string.IsNullOrEmpty(column.className))
                        {
                            //dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"сlassName\":'" + column.className + "'");
                            dataTablesBuilder.Append(",");
                        }
                        if (!column.orderable)
                        {
                            //dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"orderable\":" + column.orderable.ToString().ToLower() + "");
                            dataTablesBuilder.Append(",");
                        }
                        if (!column.visible)
                        {
                            dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"visible\":" + column.visible.ToString().ToLower() + "");
                        }
                        if (!string.IsNullOrEmpty(column.render))
                        {
                            dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"render\":" + column.render);
                        }
                        dataTablesBuilder.Append("}");
                        if (i != dataTable.Columns.Count - 1)
                        {
                            dataTablesBuilder.Append(",");
                        }
                    }
                    dataTablesBuilder.Append("],");
                }

                //buttons
                if (dataTable.Buttons != null && dataTable.Buttons.Count > 0)
                {
                    dataTablesBuilder.Append("buttons:[");
                    for (var i = 0; i < dataTable.Buttons.Count; i++)
                    {
                        var button = dataTable.Buttons.ElementAt(i);

                        dataTablesBuilder.Append("{");
                        dataTablesBuilder.Append("\"text\":'" + button.text + "'");

                        dataTablesBuilder.Append(",");
                        dataTablesBuilder.Append("\"className\":' btn btn-outline-primary " + (!string.IsNullOrEmpty(button.className) ? button.className : string.Empty) + "'");

                        if (!string.IsNullOrEmpty(button.action))
                        {
                            dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"action\": function(e, dt, node, config){" + button.action + "}");
                        }
                        dataTablesBuilder.Append("}");
                        if (i != dataTable.Buttons.Count - 1)
                        {
                            dataTablesBuilder.Append(",");
                        }
                    }
                    dataTablesBuilder.Append("],");
                }

                dataTablesBuilder.Append("data:" + JsonConvert.SerializeObject(dataTable.Data) + ",");
                dataTablesBuilder.Append("\"searching\":" + dataTable.Searching.ToString().ToLower() + ",");

                //selection
                if (dataTable.Selection != null)
                {
                    dataTablesBuilder.Append("select: " + dataTable.Select.ToString().ToLower() + ",");
                    dataTablesBuilder.Append("\"select\":");
                    dataTablesBuilder.Append("{ \"style\":'" + dataTable.Selection.Style.ToString() + "'");
                    if (!string.IsNullOrEmpty(dataTable.Selection.selector))
                    {
                        dataTablesBuilder.Append(",");
                        dataTablesBuilder.Append("\"selector\": '" + dataTable.Selection.selector + "'");
                    }

                    dataTablesBuilder.Append("},");
                }

                dataTablesBuilder.Append("\"bInfo\":false,");

                //language settings
                dataTablesBuilder.Append("\"language\":{");
                dataTablesBuilder.Append("\"lengthMenu\": '" + Resources.Plugins.lengthMenu + "',");
                dataTablesBuilder.Append("\"paginate\":{");
                dataTablesBuilder.Append("\"first\": '" + Resources.Plugins.first +"',");
                dataTablesBuilder.Append("\"last\": '" + Resources.Plugins.last + "',");
                dataTablesBuilder.Append("\"next\": '" + Resources.Plugins.next + "',");
                dataTablesBuilder.Append("\"previous\": '" + Resources.Plugins.previous + "'");
                dataTablesBuilder.Append("},");
                dataTablesBuilder.Append("\"emptyTable\": '" + Resources.Plugins.emptyTable + "',");
                dataTablesBuilder.Append("}");

                dataTablesBuilder.Append("});");

                //events
                if (!string.IsNullOrEmpty(dataTable.RowSelect))
                {
                    dataTablesBuilder.AppendLine("window." + dataTable.Id + ".on('select', function(e, dt, type, indexes){" + dataTable.RowSelect + "});");
                }

                dataTablesBuilder.Append("</script>");
            }
            return new MvcHtmlString(dataTablesBuilder.ToString());
        }
    }
}