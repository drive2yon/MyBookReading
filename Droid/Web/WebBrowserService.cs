using System;
using Android.Content;
using MyBookReading.Droid.Web;
using MyBookReading.Web;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebBrowserService))]

namespace MyBookReading.Droid.Web
{
	public class WebBrowserService : IWebBrowserService
	{
		public void Open(Uri uri)
		{
			Forms.Context.StartActivity(
				new Intent(Intent.ActionView,
					global::Android.Net.Uri.Parse(uri.AbsoluteUri)));
		}
	}
}
