using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.GoogleAnalytics;

namespace MyBookReading.Droid
{
    [Activity(Label = "MyBookReading.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme"/*, MainLauncher = true*/, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            GoogleAnalytics.Current.Config.TrackingId = "UA-119889066-1";
            GoogleAnalytics.Current.Config.AppId = this.PackageManager.GetPackageInfo(this.PackageName, 0).PackageName;
            GoogleAnalytics.Current.Config.AppName = "図書館ほんだな";
            GoogleAnalytics.Current.Config.AppVersion = this.PackageManager.GetPackageInfo(this.PackageName, 0).VersionName;
            GoogleAnalytics.Current.InitTracker();

            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }
    }
}
