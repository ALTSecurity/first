using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ALTSecurity.Web.Models.Extraction;
using SaltwaterTaffy.Container;

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


    public class ServiceResult
    {
        public string DomainName { get; set; }

        public DomainState State { get; set; }

        public List<Port> Services { get; set; }

        public List<string> Cve { get; set; }
    }


    public class DorkFile
    {
        public int FileId { get; set; }

        public string FileName { get; set; }

        public byte[] FileData { get; set; }

        public string Url { get; set; }

        public List<Metadata> Metadata { get; set; }


        public DorkFile()
        {
            Metadata = new List<Metadata>();
        }
    }

    public class Metadata
    {
        public MetadataType Type { get; set; }

        public string Value { get; set; }

        public bool IsActual { get; set; }

        public bool IsAuth { get; set; }

        public string Notice { get; set; }
    }

    public class Leakege
    {
        public string Url { get; set; }

        public int Mentions { get; set; }
    }


    public class CveRes
    {
        public string Service { get; set; }

        public string CveId { get; set; }
    }


    public enum MetadataType
    {
        author = 0,
        service = 1
    }
}
