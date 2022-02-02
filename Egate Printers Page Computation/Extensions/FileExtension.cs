using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;

namespace Egate_Printers_Page_Computation
{
    public class FileExtension : DependencyObject
    {
        public static readonly DependencyProperty FileNameProperty = DependencyProperty.RegisterAttached("FileName", typeof(string), typeof(FileExtension));
        public static readonly DependencyProperty DirectoryProperty = DependencyProperty.RegisterAttached("Directory", typeof(string), typeof(FileExtension));

        public static string GetFileName(DependencyObject obj)
        {
            return (string)obj.GetValue(FileNameProperty);
        }

        public static void SetFileName(DependencyObject obj, string value)
        {
            obj.SetValue(FileNameProperty, value);
        }

        public static string GetDirectory(DependencyObject obj)
        {
            return (string)obj.GetValue(DirectoryProperty);
        }

        public static void SetDirectory(DependencyObject obj, string value)
        {
            obj.SetValue(DirectoryProperty, value);
        }

        public static string GetFullFileName(DependencyObject obj)
        {
            string fileName = FileExtension.GetFileName(obj);
            string directory = FileExtension.GetDirectory(obj);
            return Path.Combine(directory, fileName);
        }
    }
}
