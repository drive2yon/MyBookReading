using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MyBookReading.Droid
{
    [Activity(Label = "MyBookReading.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme"/*, MainLauncher = true*/, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, BookShelf.ICreateCacheFileEvent
    {
        public static MainActivity ActivityContext; //別のクラスから関数を呼ぶためにstaticで持つ

        private int IntentCode; //OnActivityResult の判定用コード

        public event BookShelf.CreateCacheFileEventHandler CreateCacheFileEventHandler;

        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());

            ActivityContext = this;
        }

        /// <summary>
        /// 外部intentを利用してファイル選択画面を表示する。
        /// ファイルを選択した結果はOnActivityResultで受け取る。
        /// </summary>
        public void StartFileLoadExplorer()
        {
            Intent intent = new Intent(Intent.ActionGetContent);
            intent.SetType("text/plain");

            IntentCode = DateTime.Now.Millisecond;

            MainActivity.ActivityContext.StartActivityForResult(Intent.CreateChooser(intent, "Select Load File"), IntentCode);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode != IntentCode)
            {
                return;
            }

            if (resultCode != Result.Ok)
            {
                return;
            }

            if (CreateCacheFileEventHandler == null)
            {
                return;
            }

            Java.IO.File cacheFile = SaveBookShelfTempFile(data);

            this.CreateCacheFileEventHandler(this, cacheFile.Path);
        }

        /// <summary>
        /// 外部intentで選択したファイルをアプリ専用ディレクトリ領域に一時フィアルとして保存する。
        /// </summary>
        private Java.IO.File SaveBookShelfTempFile(Intent data)
        {
            //出力ファイルを用意する
            Java.IO.File cacheFile = new Java.IO.File(this.ExternalCacheDir, "bookshelf_cache.txt");
            try
            {
                //入力ファイル
                using (System.IO.Stream fis = this.ContentResolver.OpenInputStream(data.Data))
                {
                    //出力ファイル
                    Java.IO.FileOutputStream fos = new Java.IO.FileOutputStream(cacheFile);

                    byte[] buf = new byte[32768];   // 一時バッファ
                    int len = 0;
                    while (true)
                    {
                        len = fis.Read(buf, 0, buf.Length);
                        if (len > 0)
                        {
                            fos.Write(buf, 0, len);
                        }
                        else
                        {
                            break;
                        }
                    }
                    //ファイルに書き込む
                    fos.Flush();
                    fos.Close();
                    fis.Close();
                }
                return cacheFile;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
