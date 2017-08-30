using System;
using MyBookReading.iOS.Web;
using MyBookReading.Web;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(WebBrowserService))]
namespace MyBookReading.iOS.Web
{
	public class WebBrowserService : IWebBrowserService
	{
		public void Open(Uri uri)
		{
			UIApplication.SharedApplication.OpenUrl(uri);
		}
	}
}
