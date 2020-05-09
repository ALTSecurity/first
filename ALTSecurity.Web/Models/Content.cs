using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ALTSecurity.Web.Models
{
    public enum LayoutMode
    {
        nav = 0,
        blank = 1
    }

    [Serializable]
    public class Model
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }
}
