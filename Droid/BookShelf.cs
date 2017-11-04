using System;
using static MyBookReading.SettingPage;

[assembly: Xamarin.Forms.Dependency(typeof(MyBookReading.Droid.BookShelf))]
namespace MyBookReading.Droid
{
    public class BookShelf : IBookShelf
    {
        public string SaveFile(string filename, string dataText)
        {
            string filepath = GetFilePath(filename);
            System.IO.File.WriteAllText(filepath, dataText);
            return filepath;
        }
        public string GetFilePath(string filename)
        {
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            string filepath = path.Path + "/" + filename;
            return filepath;
        }
        public string LoadFile(string filepath)
        {
            return System.IO.File.ReadAllText(filepath);
        }
        public bool IsExistFile(string filepath)
        {
            return System.IO.File.Exists(filepath);
        }
    }
}
