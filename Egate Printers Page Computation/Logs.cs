using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Egate_Printers_Page_Computation
{
    public static class Logs
    {
        public static void Write(string msg)
        {
            Logs.DoWrite(msg);
        }

        public static void WriteException(Exception ex)
        {
            Logs.DoWrite(ex.ToString());
        }

        private static void DoWrite(string msg)
        {
            string directory = @".\logs";
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string file = Path.Combine(directory, "logs.txt");
            string content = string.Format("[{0}] {1}\n", DateTime.Now.ToString("MM-dd-yyyy hhh:mm:ss"), msg);
            File.AppendAllText(file, content);
        }
    }
}
