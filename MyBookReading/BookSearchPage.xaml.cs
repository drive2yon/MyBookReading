using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;
//using Plugin.GoogleAnalytics;

namespace MyBookReading
{
    public partial class BookSearchPage : ContentPage
    {
        BookShelf bookShelf;
        AmazonCredentials amazonKey;
        private bool bSearchTitle;
        public BookSearchPage(BookShelf bookShelf)
        {
            this.bookShelf = bookShelf;
            amazonKey = LoadCredentialsFile();
            InitializeComponent();
			this.Appearing += (object sender, System.EventArgs e) => this.entryTitle.Focus();


            //スクリーンのサイズを取得する
            var screenWidth = Application.Current.MainPage.Width;
            var screenHeight = Application.Current.MainPage.Height;

            ResizeView(screenWidth, screenHeight);

        }

        private void ResizeView(double width, double height)
        {
            entryScrollView.WidthRequest = width;

                     relativeLayout.Children.Add(cvLayer,
              Constraint.RelativeToParent(parent => 0), // xConstraint
              Constraint.RelativeToParent(parent => 0), // yConstraint
                         Constraint.RelativeToParent(parent => parent.Width), // widthConstraint
              Constraint.RelativeToParent(parent => parent.Height) // heightConstraint
            );

            relativeLayout.Children.Add(frLayer,
              Constraint.RelativeToParent(parent => (itemStackLayout.Width  / 2) - (frLayer.Width / 2)), // xConstraint
              Constraint.RelativeToParent(parent => (itemStackLayout.Height / 2) - (frLayer.Height / 2)), // yConstrain
                         Constraint.RelativeToParent(parent => Math.Min(itemStackLayout.Width,itemStackLayout.Height) / 4), // widthConstraint
              Constraint.RelativeToParent(parent => Math.Min(itemStackLayout.Width,itemStackLayout.Height) / 4) // heightConstraint
            );
        }

		async void Handle_SearchClicked(object sender, System.EventArgs e)
		{
            //GoogleAnalytics.Current.Tracker.SendView("MaiinPage");
            //GoogleAnalytics.Current.Tracker.SendEvent("Category", "Action", "Label", 0);

            string keyword;
            AmazonBookSearch.SearchType searchType;
            if(bSearchTitle){
                keyword = this.entryTitle.Text;
                searchType = AmazonBookSearch.SearchType.SearchBook_ByTitle;
            }else{
                keyword = this.entryAuthor.Text;
                searchType = AmazonBookSearch.SearchType.SearchBook_ByAuthor;
			}

            if( keyword==null || keyword.Length == 0)
            {
                await DisplayAlert("検索するには？", "１文字以上入力してください", "OK");
                return;
            }

            cvLayer.IsVisible = frLayer.IsVisible = true;
			AmazonBookSearch search = new AmazonBookSearch(amazonKey);
    		ObservableCollection<Book> books = new ObservableCollection<Book>();
            bool result = await search.Search(searchType, keyword, books);
			if(!result)
            {
				cvLayer.IsVisible = frLayer.IsVisible = false;
				await DisplayAlert("検索に失敗", "しばらくしてから検索してください", "OK");
                return;
            }
            else
            {
				await Navigation.PushAsync( new BookSearchResultPage(this.bookShelf, search, books) );
				cvLayer.IsVisible = frLayer.IsVisible = false;
				return;
			}
		}

        void Handle_FocusedTitle(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            this.labelTitle.TextColor = Color.Blue;
            this.labelTitle.FontAttributes = FontAttributes.Bold;
            this.labelAuthor.TextColor = Color.Default;
			this.labelAuthor.FontAttributes = FontAttributes.None;
            this.btnSearch.Text = "本のタイトルで検索";
            bSearchTitle = true;
		}

		void Handle_FocusedAuthor(object sender, Xamarin.Forms.FocusEventArgs e)
		{
            this.labelTitle.TextColor = Color.Default;
			this.labelTitle.FontAttributes = FontAttributes.None;
			this.labelAuthor.TextColor = Color.Blue;
			this.labelAuthor.FontAttributes = FontAttributes.Bold;
			this.btnSearch.Text = "本の著者で検索";
			bSearchTitle = false;
		}

		private AmazonCredentials LoadCredentialsFile()
		{
			//AmazonCredentials.json sample
			//{"associate_tag":"XXXXX","access_key_id":"XXXXX","secret_access_key":"XXXXX"}

			var assembly = typeof(BookSearchPage).GetTypeInfo().Assembly;
			Stream stream = assembly.GetManifestResourceStream("MyBookReading.Assets.AmazonCredentials.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
			AmazonCredentials amazon = JsonConvert.DeserializeObject<AmazonCredentials>(text);
			return amazon;
		}
	}
}
