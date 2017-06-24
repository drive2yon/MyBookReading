using System;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class MyBookReadingPage : TabbedPage
    {
        public MyBookReadingPage()
        {
            InitializeComponent();

            MyBookListPage myBookListPage = new MyBookListPage();
            BookSearchPage bookSearchPage = new BookSearchPage();
            AmazonBookSearchPage amazonPage = new AmazonBookSearchPage();

            this.Children.Add(myBookListPage);
            this.Children.Add(bookSearchPage);
            this.Children.Add(amazonPage);

            myBookListPage.Title = "MyBook";
            bookSearchPage.Title = "BookSearch";
            amazonPage.Title = "AmazonSearch";

            LoadCredentialsFile();
        }

        private void LoadCredentialsFile()
        {
            var assembly = typeof(MyBookReadingPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MyBookReading.Assets.TestCredentials.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
        }
    }
}
