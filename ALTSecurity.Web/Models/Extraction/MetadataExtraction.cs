using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NPOI.XWPF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI;
using iTextSharp.text.pdf;

namespace ALTSecurity.Web.Models.Extraction
{
    /// <summary>
    /// Опис назв занальних властивостей
    /// </summary>
    public static class CommonTags
    {
        public static readonly string Author = "AUTHOR";
        public static readonly string Created = "CREATED";
        public static readonly string Description = "DESCRIPTION";
        public static readonly string Modified = "MODIFIED";
        public static readonly string LastUserModified = "LASTUSERMODIFIED";
        public static readonly string Keywords = "KEYWORDS";
    }

    /// <summary>
    /// Допоміжний клас для отримання метаданих зображень
    /// </summary>
    public class BitmapExtractorHelper
    {
        private static readonly Dictionary<int, string> PropertyNames = new Dictionary<int, string>
        {
            { 0x0132, "PropertyTagDateTime"},
            { 0x0131, "PropertyTagSoftwareUsed" },
            { 0x010E, "PropertyTagImageDescription" },
            { 0x013B, "PropertyTagArtist" },
            { 0x013C, "PropertyTagHostComputer" }
        };

        public BitMapProperty GetProperty(PropertyItem item)
        {
            return new BitMapProperty
            {
                Name = PropertyNames[item.Id],
                Value = Encoding.ASCII.GetString(item.Value)
            };
        }
    }

    /// <summary>
    /// Сутність властивості метаданих
    /// </summary>
    public class BitMapProperty
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }

    public class MetadataExtraction
    {
        /// <summary>
        /// Перелік доступних розширень файлів
        /// </summary>
        readonly string EXT_PDF = ".pdf";
        readonly string EXT_PNG = ".png";
        readonly string EXT_JPG = ".jpg";
        readonly string EXT_DOCX = ".docx";
        readonly string EXT_XLSX = ".xlsx";


        /// <summary>
        /// Отримати метадані документу
        /// </summary>
        /// <param name="fileName">Назва файлу</param>
        /// <param name="fileData">Тіло файлу</param>
        /// <returns></returns>
        public Dictionary<string, string> GetFileMetadata(string fileName, byte[] fileData)
        {
            string fileExt = Path.GetExtension(fileName);
            Dictionary<string, string> metadata = new Dictionary<string, string>();

            if (fileExt == EXT_PDF || fileExt == EXT_PNG || fileExt == EXT_JPG)
            {
                if (fileExt == EXT_PNG || fileExt == EXT_JPG)
                {
                    using (MemoryStream msImg = new MemoryStream(fileData))
                    {
                        var img = Image.FromStream(msImg);
                        ParseBtmapPropsHelper(metadata, img.PropertyItems);
                    }
                }
                else
                {
                    using (PdfReader reader = new PdfReader(fileData))
                    {
                        ParsePdfPropsHelper(metadata, reader.Info);
                    }
                }
            }
            else if (fileExt == EXT_XLSX || fileExt == EXT_DOCX)
            {
                POIXMLProperties props;
                using (MemoryStream inStream = new MemoryStream(fileData))
                {
                    if (fileExt == EXT_DOCX)
                    {
                        XWPFDocument msDoc = new XWPFDocument(inStream);
                        props = msDoc.GetProperties();
                    }
                    else
                    {
                        XSSFWorkbook xssf = new XSSFWorkbook(inStream);
                        props = xssf.GetProperties();
                    }
                }

                ParseMsPropsHelper(metadata, props);
            }

            return metadata;
        }

        /// <summary>
        ///  Метадані документу розширенням .xls, .xlsx, .docx
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="props"></param>
        private void ParseMsPropsHelper(Dictionary<string, string> metadata, POIXMLProperties props)
        {
            if (props?.CoreProperties != null)
            {
                metadata.Add(CommonTags.Author, props.CoreProperties.Creator);
                metadata.Add(CommonTags.Created, props.CoreProperties.Created.ToString());
                metadata.Add(CommonTags.Description, props.CoreProperties.Description);
                metadata.Add(CommonTags.Modified, props.CoreProperties.Modified.ToString());
                metadata.Add(CommonTags.LastUserModified, props.CoreProperties.LastModifiedByUser);
                metadata.Add(CommonTags.Keywords, props.CoreProperties.Keywords);
            }
        }

        /// <summary>
        /// Метадані документу з розширенням .pdf
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="info"></param>
        private void ParsePdfPropsHelper(Dictionary<string, string> metadata, Dictionary<string, string> info)
        {
            if (info != null && info.Count > 0)
            {
                foreach (var item in info)
                {
                    string value = string.Empty;

                    if(item.Key == "CreationDate" || item.Key == "ModifyDate")
                    {
                        value = item.Value.Replace("D:", "").Split('+')[0];
                    }
                    else if(item.Key == "Producer" || item.Key == "Creator")
                    {
                        value = item.Value.Replace('®',' ');
                        var a = value;
                    }
                    else
                    {
                        value = item.Value;
                    }
                   
                    metadata.Add(item.Key, value);
                }
            }
        }

        /// <summary>
        /// Метадані зображень .png, .jpg
        /// </summary>
        /// <param name="metadata"></param>
        /// <param name="items"></param>
        private void ParseBtmapPropsHelper(Dictionary<string, string> metadata, PropertyItem[] items)
        {
            BitmapExtractorHelper bitmap = new BitmapExtractorHelper();
            if (items != null && items.Length > 0)
            {
                foreach (var item in items)
                {
                    var property = bitmap.GetProperty(item);
                    metadata.Add(property.Name, property.Value);
                }
            }
        }

    }
}