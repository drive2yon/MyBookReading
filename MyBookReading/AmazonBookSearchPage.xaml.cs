using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
using System.Security;

namespace MyBookReading
{
    public partial class AmazonBookSearchPage : ContentPage
    {
		const string ACCESS_KEY = "MyAccessKey";
		const string SECRET_KEY = "MySecretKey";
		const string TAG_IT = "MyTag";
        const string END_POINT_JP = "ecs.amazonaws.jp"; 

		Entry bookSearchKeyword;
		Label bookSearchResult;
		public AmazonBookSearchPage()
        {
            InitializeComponent();
			bookSearchKeyword = new Entry
			{
				Keyboard = Keyboard.Email,
				Placeholder = "Enter book keyword",
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			Button button = new Button
			{
				Text = "Click Me!",
				Font = Font.SystemFontOfSize(NamedSize.Large),
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			button.Clicked += OnButtonClicked;

			bookSearchResult = new Label
			{
				Text =
					"Xamarin.Forms is a cross-platform natively " +
					"backed UI toolkit abstraction that allows " +
					"developers to easily create user interfaces " +
					"that can be shared across Android, iOS, and " +
					"Windows Phone.",

				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			ScrollView scrollView = new ScrollView
			{
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = bookSearchResult
			};



			this.Content = new StackLayout
			{
				Children =
				{
					bookSearchKeyword,
					button,
					scrollView
				}
			};
        }

		private async void OnButtonClicked(object sender, EventArgs e)
		{
			DateTime now = DateTime.UtcNow;
			string timestamp = now.ToString("yyyy-MM-ddTHH:mm:ss.000Z"); // IS0 8601
			timestamp = timestamp.Replace(":", "%3A");

			string keyWord = "chopin";
			string toBeSigned = "GET\nwebservices.amazon.com\n/onca/xml\nAWSAccessKeyId=AKIAIADXL5CJRNH3Y67Q&AssociateTag=localhost012-20&Condition=All&Keywords=" + keyWord + "&Operation=ItemSearch&ResponseGroup=ItemAttributes&SearchIndex=All&Service=AWSECommerceService&Timestamp=" + timestamp + "&Version=2011-08-01";

			//string signedString = signString(toBeSigned);

			//string finalUrl = "http://webservices.amazon.com/onca/xml?AWSAccessKeyId=AKIAIADXL5CJRNH3Y67Q&AssociateTag=localhost012-20&Condition=All&Keywords=" + keyWord + "&Operation=ItemSearch&ResponseGroup=ItemAttributes&SearchIndex=All&Service=AWSECommerceService&Timestamp=" + timestamp + "&Version=2011-08-01&Signature=" + signedString;
            //bookSearchResult.Text = finalUrl;

		}

		//string signString(string signMe)
		//{
		//	byte[] bytesToSign = Encoding.UTF8.GetBytes(signMe);
		//	string AWSSecretKey = "mysecretkey";
		//	byte[] secretKeyBytes = Encoding.UTF8.GetBytes(AWSSecretKey);
		//	HMAC hmacSha256 = new HMACSHA256(secretKeyBytes); // RFC 2104
		//	byte[] hashBytes = hmacSha256.ComputeHash(bytesToSign);
		//	string signature = Convert.ToBase64String(hashBytes);
		//	signature = signature.Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B"); // RFC 3986
		//	return signature;
		//}
    }
}
