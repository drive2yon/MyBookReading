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

            MyBookListPage myBookListPage = new MyBookListPage();
            BookSearchPage bookSearchPage = new BookSearchPage();

            this.Children.Add(new NavigationPage(myBookListPage)
            {
				Title = "MyBook",
				BarBackgroundColor = Color.Aquamarine,
			});
            this.Children.Add(new NavigationPage(bookSearchPage)
            {
                Title = "BookSearch",
                BarBackgroundColor = Color.Aquamarine,
            });

            this.Children.Add(new AmazonBookSearchPage()
            {
                Title="AmazonSearch",
            });

            myBookListPage.Title = "MyBook";
            bookSearchPage.Title = "BookSearch";

            this.Title = "My Book Reading";
        }
    }
}
