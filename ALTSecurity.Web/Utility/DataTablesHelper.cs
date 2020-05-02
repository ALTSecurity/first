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

        public DataTablesOrdering Order { get; set; }

        public bool Searching { get; set; }

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

        public string data { get; set; }

        public string render { get; set; }

        public bool visible { get; set; } = true;

    }


    public class DataTableSelection
    {
        public SelectionStyle Style { get; set; }
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
        checkbox = 2
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
            if (dataTable != null)
            {
                dataTablesBuilder.Append("<script>");
              
                dataTablesBuilder.Append(@"window." + dataTable.Id + " =  $('#" + dataTable.Id + "').DataTable({");
                dataTablesBuilder.Append("dom:'" + dataTable.Dom + "',");
                
                if (dataTable.Order != null)
                {
                    dataTablesBuilder.Append("order:[");
                    dataTablesBuilder.Append("[" + dataTable.Order.Column + ",");
                    dataTablesBuilder.Append(@"'" + (dataTable.Order.Type == OrderingType.desc ? "desc" : "asc") + "']");
                    dataTablesBuilder.Append("],");
                }

                if (dataTable.Columns != null && dataTable.Columns.Count > 0)
                {
                    dataTablesBuilder.Append("columns:[");
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        var column = dataTable.Columns.ElementAt(i);

                        dataTablesBuilder.Append("{");
                        dataTablesBuilder.Append("\"title\":'" + column.title + "',");
                        dataTablesBuilder.Append("\"data\":'" + column.data + "'");
                        if (!column.visible)
                        {
                            dataTablesBuilder.Append(",");
                            dataTablesBuilder.Append("\"visible\":" + column.visible.ToString().ToLower() + ",");
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

                dataTablesBuilder.Append("data:" + JsonConvert.SerializeObject(dataTable.Data) + ",");
                dataTablesBuilder.Append("\"searching\":" + dataTable.Searching.ToString().ToLower() + ",");

                if (dataTable.Selection != null)
                {
                    dataTablesBuilder.Append("select: true,");
                    dataTablesBuilder.Append("\"select\":");
                    dataTablesBuilder.Append("{ \"style\":" + (dataTable.Selection.Style == SelectionStyle.single ? "'single'" : ""));
                    dataTablesBuilder.Append("},");
                }

                dataTablesBuilder.Append("\"bInfo\":false,");

                //language settings
                dataTablesBuilder.Append("\"language\":{");
                dataTablesBuilder.Append($"\"lengthMenu\": '{ Resources.Plugins.lengthMenu }',");
                dataTablesBuilder.Append("\"paginate\":{");
                dataTablesBuilder.Append($"\"first\": '{ Resources.Plugins.first }',");
                dataTablesBuilder.Append($"\"last\": '{ Resources.Plugins.last }',");
                dataTablesBuilder.Append($"\"next\": '{ Resources.Plugins.next }',");
                dataTablesBuilder.Append($"\"previous\": '{ Resources.Plugins.previous }'");
                dataTablesBuilder.Append("}");
                dataTablesBuilder.Append("}");

                dataTablesBuilder.Append("});");

                //events
                dataTablesBuilder.AppendLine("window." + dataTable.Id + ".on('select', function(e, dt, type, indexes){" + dataTable.RowSelect + "});");
               
                dataTablesBuilder.Append("</script>");
            }
            return new MvcHtmlString(dataTablesBuilder.ToString());
        }
    }
}