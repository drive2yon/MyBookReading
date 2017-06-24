using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class MyBookReadingPage : TabbedPage
    {
        public MyBookReadingPage()
        {
            InitializeComponent();

			AmazonCresidentials amazonKey = LoadCredentialsFile();

			MyBookListPage myBookListPage = new MyBookListPage();
            BookSearchPage bookSearchPage = new BookSearchPage();
            AmazonBookSearchPage amazonPage = new AmazonBookSearchPage(amazonKey);

            this.Children.Add(myBookListPage);
            this.Children.Add(bookSearchPage);
            this.Children.Add(amazonPage);

            myBookListPage.Title = "MyBook";
            bookSearchPage.Title = "BookSearch";
            amazonPage.Title = "AmazonSearch";

        }

        private AmazonCresidentials LoadCredentialsFile()
        {
			//AmazonCredentials.json sample
			//{"associate_tag":"XXXXX","access_key_id":"XXXXX","secret_access_key":"XXXXX"}

			var assembly = typeof(MyBookReadingPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MyBookReading.Assets.AmazonCredentials.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
            AmazonCresidentials amazon = JsonConvert.DeserializeObject<AmazonCresidentials>(text);
            return amazon;
        }
    }
}
