﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyBookReading
{
	//sample xml generated by http://webservices.amazon.co.jp/scratchpad/index.html#
    //ResponseGroup: Images,Small

    //convert xml to json by VisualStudioCode "SmartPaset" plugin,
	//and generated json desirialize class by http://json2csharp.com/#
	public class Header
	{
		public string _Name { get; set; }
		public string _Value { get; set; }
	}

	public class HTTPHeaders
	{
		public Header Header { get; set; }
	}

	public class Argument
	{
		public string _Name { get; set; }
		public string _Value { get; set; }
	}

	public class Arguments
	{
		public List<Argument> Argument { get; set; }
	}

	public class OperationRequest
	{
		public HTTPHeaders HTTPHeaders { get; set; }
		public string RequestId { get; set; }
		public Arguments Arguments { get; set; }
		public string RequestProcessingTime { get; set; }
	}

	public class ItemSearchRequest
	{
		public string Keywords { get; set; }
		public List<string> ResponseGroup { get; set; }
		public string SearchIndex { get; set; }
	}

	public class Request
	{
		public string IsValid { get; set; }
		public ItemSearchRequest ItemSearchRequest { get; set; }
	}

	public class ItemLink
	{
		public string Description { get; set; }
		public string URL { get; set; }
	}

	public class ItemLinks
	{
		public List<ItemLink> ItemLink { get; set; }
	}

	public class Height
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class SmallImage
	{
		public string URL { get; set; }
		public Height Height { get; set; }
		public Width Width { get; set; }
	}

	public class Height2
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width2
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class MediumImage
	{
		public string URL { get; set; }
		public Height2 Height { get; set; }
		public Width2 Width { get; set; }
	}

	public class Height3
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width3
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class LargeImage
	{
		public string URL { get; set; }
		public Height3 Height { get; set; }
		public Width3 Width { get; set; }
	}

	public class Height4
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width4
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class SwatchImage
	{
		public string URL { get; set; }
		public Height4 Height { get; set; }
		public Width4 Width { get; set; }
	}

	public class Height5
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width5
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class SmallImage2
	{
		public string URL { get; set; }
		public Height5 Height { get; set; }
		public Width5 Width { get; set; }
	}

	public class Height6
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width6
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class ThumbnailImage
	{
		public string URL { get; set; }
		public Height6 Height { get; set; }
		public Width6 Width { get; set; }
	}

	public class Height7
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width7
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class TinyImage
	{
		public string URL { get; set; }
		public Height7 Height { get; set; }
		public Width7 Width { get; set; }
	}

	public class Height8
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width8
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class MediumImage2
	{
		public string URL { get; set; }
		public Height8 Height { get; set; }
		public Width8 Width { get; set; }
	}

	public class Height9
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class Width9
	{
		public string _Units { get; set; }
		public string __text { get; set; }
	}

	public class LargeImage2
	{
		public string URL { get; set; }
		public Height9 Height { get; set; }
		public Width9 Width { get; set; }
	}

	public class ImageSet
	{
		public SwatchImage SwatchImage { get; set; }
		public SmallImage2 SmallImage { get; set; }
		public ThumbnailImage ThumbnailImage { get; set; }
		public TinyImage TinyImage { get; set; }
		public MediumImage2 MediumImage { get; set; }
		public LargeImage2 LargeImage { get; set; }
		public string _Category { get; set; }
	}

	public class ImageSets
	{
		public ImageSet ImageSet { get; set; }
	}

	public class ItemAttributes
	{
		public object Creator { get; set; }
		public string Manufacturer { get; set; }
		public string ProductGroup { get; set; }
		public string Title { get; set; }
		public object Author { get; set; }
	}

	public class Item
	{
		public string ASIN { get; set; }
		public string DetailPageURL { get; set; }
		public ItemLinks ItemLinks { get; set; }
		public SmallImage SmallImage { get; set; }
		public MediumImage MediumImage { get; set; }
		public LargeImage LargeImage { get; set; }
		public ImageSets ImageSets { get; set; }
		public ItemAttributes ItemAttributes { get; set; }
	}

	public class Items
	{
		public Request Request { get; set; }
		public string TotalResults { get; set; }
		public string TotalPages { get; set; }
		public string MoreSearchResultsUrl { get; set; }
		public List<Item> Item { get; set; }
	}

	public class ItemSearchResponse
	{
		public OperationRequest OperationRequest { get; set; }
		public Items Items { get; set; }
		public string _xmlns { get; set; }
	}

	public class RootObject
	{
		public ItemSearchResponse ItemSearchResponse { get; set; }
	}


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

        private async void OnButtonClicked(object sender, EventArgs e)
        {
            string url = makeRequestURL("xamarin");
            bookSearchResult.Text = url;
            bookSearchResult.Text = await VisitUrl(url);
        }

        private string makeRequestURL(string keyWord)
        {
            DateTime now = DateTime.UtcNow;
            string strTime = now.ToString("yyyy-MM-ddTHH:mm:ss.000Z"); // IS0 8601
            strTime = strTime.Replace(":", "%3A");

            string PARAM_TIME_STAMP = "&Timestamp=" + strTime.Replace(":", "%3A");
            string PARAM_AWSKEY_AND_TAG = "AWSAccessKeyId=" + amazonKey.access_key_id + "&AssociateTag=" + amazonKey.associate_tag;
            string PARAM_KEYWORD = "&Keywords=" + keyWord;
            const string PARAM_OPERATION = "&Operation=ItemSearch&ResponseGroup=Images%2CSmall&SearchIndex=Books&Service=AWSECommerceService";
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

        private static async Task<string> VisitUrl(string url)
        {
			System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
			try
            {
                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync();

                Stream st = response.GetResponseStream();
                StreamReader sr = new StreamReader(st);
                XDocument doc = XDocument.Load(sr);
                string jsonText = JsonConvert.SerializeXNode(doc);

				RootObject jsonObject = JsonConvert.DeserializeObject<RootObject>(jsonText);


				foreach( var item in jsonObject.ItemSearchResponse.Items.Item)
                {
                    Debug.WriteLine(item.ItemAttributes.Title);
                    strBuilder.Append(item.ItemAttributes.Title).AppendLine();
                }
			}
            catch (System.Net.WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                HttpStatusCode code = (response == null) ? HttpStatusCode.InternalServerError : response.StatusCode;
            }
            return strBuilder.ToString();
        }
    }
}
