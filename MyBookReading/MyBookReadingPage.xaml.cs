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
        }
    }
}
