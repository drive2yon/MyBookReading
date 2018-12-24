using System;
using Android.App;
using Android.Content;
using Xamarin.Forms;
using static MyBookReading.SettingPage;

[assembly: Xamarin.Forms.Dependency(typeof(MyBookReading.Droid.BookShelf))]
namespace MyBookReading.Droid
{
    public class BookShelf : IBookShelf
    {
        /// <summary>
        /// 図書館ほんだなデータのキャッシュファイル作成完了通知イベント
        /// </summary>
        /// <param name="sender">通知元のクラスオブジェクト</param>
        /// <param name="chacheFilePath">キャッシュファイルのフルパス</param>
        public delegate void CreateCacheFileEventHandler(object sender, string chacheFilePath);

        /// <summary>
        /// 図書館ほんだなデータのキャッシュファイル作成完了通知IF
        /// </summary>
        public interface ICreateCacheFileEvent
        {
            event CreateCacheFileEventHandler CreateCacheFileEventHandler;
        };

        /// <summary>
        /// 図書館ほんだなデータの読み込み通知イベント
        /// </summary>
        public event FleEventHandler OnFileLoaded;

        /// <summary>
        /// 指定したファイル名で図書バックアップデータを保存する。
        /// 保存先はAndroid-APIで取得できる外部ストレージパスの直下となる。
        /// </summary>
        /// <param name="filename">保存ファイル名</param>
        /// <param name="dataText">保存データ(json)</param>
        /// <returns>保存したファイルのフルパス</returns>
        public string SaveFile(string filename, string dataText)
        {
            //外部ストレージのDocumentフォルダ パス取得
            var path = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments);
            string filepath = path.Path + "/" + filename;
            System.IO.File.WriteAllText(filepath, dataText);
            return filepath;
        }

        /// <summary>
        /// 図書バックアップデータファイルを読み込む。
        /// </summary>
        public void LoadFile()
        {
            MainActivity.ActivityContext.CreateCacheFileEventHandler += (sender, filePath) =>
            {
                if(OnFileLoaded == null)
                {
                    return;
                }

                if(filePath == null)
                {
                    OnFileLoaded(this, null);
                }else{
                    string jsonText = System.IO.File.ReadAllText(filePath);
                    OnFileLoaded(this, jsonText);
                }
            };
            MainActivity.ActivityContext.StartFileLoadExplorer();
        }

        /// <summary>
        /// フルパスで指定したファイルが存在するか判定する。
        /// </summary>
        /// <param name="filepath">ファイルフルパス</param>
        /// <returns>trueの場合はファイルが存在する</returns>
        public bool IsExistFile(string filepath)
        {
            return System.IO.File.Exists(filepath);
        }
    }
}
