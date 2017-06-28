using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class AmazonBookSearchPage : ContentPage
    {
        private AmazonCresidentials amazonKey;
        private const string MARKET_PLACE_URL = "webservices.amazon.co.jp";
        private const string DESTINATION = "ecs.amazonaws.jp";

        const string END_POINT_JP = "ecs.amazonaws.jp";

        Entry bookSearchKeyword;
        Label bookSearchResult;
        public AmazonBookSearchPage(AmazonCresidentials key)
        {
            amazonKey = key;

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

        private void OnButtonClicked(object sender, EventArgs e)
        {
            string url = makeRequestURL("nosql");
            bookSearchResult.Text = url;

			//var myXMLstring = "";
			//Task task = new Task(() =>
			//{
			//	myXMLstring = AccessTheWebAsync(url).Result;
			//});
			//task.Start();
			//task.Wait();
			//Debug.WriteLine(myXMLstring);

            VisitUrl(url);
        }

        private string makeRequestURL(string keyWord)
        {
            DateTime now = DateTime.UtcNow;
            string strTime = now.ToString("yyyy-MM-ddTHH:mm:ss.000Z"); // IS0 8601
            strTime = strTime.Replace(":", "%3A");

            string PARAM_TIME_STAMP = "&Timestamp=" + strTime.Replace(":", "%3A");
            string PARAM_AWSKEY_AND_TAG = "AWSAccessKeyId=" + amazonKey.access_key_id + "&AssociateTag=" + amazonKey.associate_tag;
            string PARAM_KEYWORD = "&Keywords=" + keyWord;
            const string PARAM_OPERATION = "&Operation=ItemSearch&ResponseGroup=Images%2CItemAttributes%2COffers&SearchIndex=Books&Service=AWSECommerceService";
            const string PARAM_VERSION = "&Version=2011-08-01";

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
            strBuilder.Append("GET\n")
                      .Append(MARKET_PLACE_URL + "\n")
                      .Append("/onca/xml\n")
                      .Append(PARAM_AWSKEY_AND_TAG + PARAM_KEYWORD + PARAM_OPERATION + PARAM_TIME_STAMP + PARAM_VERSION);
            string toBeSigned = strBuilder.ToString();
            string signedString = signString(toBeSigned);

            string requestUrl = "http://" + MARKET_PLACE_URL + "/onca/xml?" + PARAM_AWSKEY_AND_TAG + PARAM_KEYWORD + PARAM_OPERATION + PARAM_TIME_STAMP + PARAM_VERSION + "&Signature=" + signedString;
            strBuilder.Clear();
            strBuilder.Append("http://")
                      .Append(MARKET_PLACE_URL)
                      .Append("/onca/xml?")
                      .Append(PARAM_AWSKEY_AND_TAG + PARAM_KEYWORD + PARAM_OPERATION + PARAM_TIME_STAMP + PARAM_VERSION)
                      .Append("&Signature=" + signedString);
            bool test = strBuilder.ToString().Equals(requestUrl);
            return strBuilder.ToString();
        }

        string signString(string signMe)
        {
            HmacSha256 hmac = new HmacSha256();
            byte[] hashBytes = hmac.Hash(signMe, amazonKey.secret_access_key);
            string signature = Convert.ToBase64String(hashBytes);
            signature = signature.Replace("/", "%2F").Replace("=", "%3D").Replace("+", "%2B"); // RFC 3986
            return signature;
        }

        private static async void VisitUrl(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync();

				var respStream = response.GetResponseStream();
				respStream.Flush();

                StreamReader sr = new StreamReader(respStream);
				string strContent = sr.ReadToEnd();
				Debug.WriteLine(strContent);

				XDocument doc = XDocument.Load(sr);
				string NAMESPACE = "http://webservices.amazon.com/AWSECommerceService/2011-08-01";
                //doc.
	   //         for (var i = 0; i < 10; i++)
    //            {
	   //            XmlNode titleNode = doc.GetElementsByTagName("Title", NAMESPACE).Item(i);
	   //            string title = titleNode.InnerText;
	   //            XmlNode priceNode = doc.GetElementsByTagName("FormattedPrice", NAMESPACE).Item(i);
	   //            string price = priceNode.InnerText;
				//	XmlNode imageNode = doc.GetElementsByTagName("SmallImage", NAMESPACE).Item(i);
				//	string image = imageNode.InnerText;
				//}
			}
            catch (System.Net.WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                HttpStatusCode code = (response == null) ? HttpStatusCode.InternalServerError : response.StatusCode;
            }
        }

		async Task<String> AccessTheWebAsync(String url)
		{
			// You need to add a reference to System.Net.Http to declare client.
			HttpClient client = new HttpClient();

			// GetStringAsync returns a Task<string>. That means that when you await the 
			// task you'll get a string (urlContents).
			Task<string> getStringTask = client.GetStringAsync(url);

			// You can do work here that doesn't rely on the string from GetStringAsync.
			//DoIndependentWork();

			// The await operator suspends AccessTheWebAsync. 
			//  - AccessTheWebAsync can't continue until getStringTask is complete. 
			//  - Meanwhile, control returns to the caller of AccessTheWebAsync. 
			//  - Control resumes here when getStringTask is complete.  
			//  - The await operator then retrieves the string result from getStringTask. 
			string urlContents = await getStringTask;

			// The return statement specifies an integer result. 
			// Any methods that are awaiting AccessTheWebAsync retrieve the length value. 
			return urlContents;
		}
    }
}
