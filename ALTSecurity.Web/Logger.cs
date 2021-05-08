using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;

namespace ALTSecurity.Web
{
    public static class Logger
    {
        static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"Log.txt");

        public static void Init()
        {
            using (var fs = File.Create(FilePath))
            {
                using (TextWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine("Application start...");
                }
            };
        }

        public static void LogException(string method, Exception ex)
        {
            using (TextWriter tw = new StreamWriter(FilePath, true))
            {
                tw.WriteLine();
                tw.Write(DateTime.Now.ToString() + " " + method + " " + ex.Message);
            }
        }

        public static void LogMessage(string method, string message)
        {
            using (TextWriter tw = new StreamWriter(FilePath, true))
            {
                tw.WriteLine();
                tw.Write(DateTime.Now.ToString() + " " + method + " " + message);
            }
        }

        public static void LogMessage(string method, List<string> messages)
        {
            using (TextWriter tw = new StreamWriter(FilePath, true))
            {
                tw.WriteLine();
                tw.Write(DateTime.Now.ToString() + method + " :");

                foreach (string ms in messages)
                {
                    tw.Write("\n");
                    tw.Write(ms);
                }
            }
        }
    }
}