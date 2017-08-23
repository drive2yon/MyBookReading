using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class BookSearchPage : ContentPage
    {
        AmazonCredentials amazonKey;
        private bool bSearchTitle;
        public BookSearchPage()
        {
            amazonKey = LoadCredentialsFile();
            InitializeComponent();
			this.Appearing += (object sender, System.EventArgs e) => this.entryTitle.Focus();
        }

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);

			entryScrollView.WidthRequest = relativeLayout.Width;

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
				await Navigation.PushAsync( new BookSearchResultPage(keyword, amazonKey, books) );
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


		//private async void OnButtonClicked(object sender, EventArgs e)
		//{
   //         string url = "https://www.googleapis.com/books/v1/volumes?q=" + searchTitleEntry.Text;
			//HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			//HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync(); // (note: await added, method call changed to GetResponseAsync()
			//Stream dataStream = response.GetResponseStream();
			//StreamReader reader = new StreamReader(dataStream);
			//string responseFromServer = reader.ReadToEnd();

            // Clean up the streams and the response.
            //reader.Close();
            //response.Close();
            //response.Close();
            //string authorList = googleBooksJsonToText(responseFromServer);
            //bookSearchResult.Text = authorList;
            //bookSearchResult.Text = responseFromServer;
		//}

        private string googleBooksJsonToText(string jsonString)
        {
            string retText = "nothing";
            int totalItems = 0;
			JObject json = JObject.Parse(jsonString);
			if ((JArray)json["items"] != null) //If there are Items, then set the number in the totalItems variable
			{
				JArray items = (JArray)json["items"];
				totalItems = items.Count;
			}

			//If we didn't get any results, say that and exit. If we actually did get results, then process
			if (totalItems == 0)
			{
                retText = "No results found";
			}
			else
			{
				//List<Book> books = new List<Book>(); //create a list of books

				////The loop runs as many times as there are items. Max will be 20, since I'm forcing only 20 results to be returned
				//// with maxResults in the search parameter string.
				//for (int i = 0; i < totalItems; i++)
				//{
				//	//This adds each new book object with the object initialization syntax and the custom constructor. I'm using string interpolation (the $ before the string)
				//	//to get the variable "i". Right now, I'm only pulling the first author name and the first category, hence the hardcoded 0 for those values.
				//	books.Add(new Book(
				//		(string)json.SelectToken($"items[{i}].volumeInfo.industryIdentifiers[0].identifier"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.title"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.authors[0]"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.publisher"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.publishedDate"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.description"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.categories[0]"),
				//		(string)json.SelectToken($"items[{i}].volumeInfo.imageLinks.thumbnail")
				//		));
				//}

				////int count = 1; //initialize a counter to tell the user
				//			   //Nested loops - for each book, print every property name and value. It will print the entire list.
				//foreach (var book in books)
				//{
    //                retText += book.Author;
    //                ////Console.WriteLine("Result {0}", count);
    //                //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(book))
    //                //{
    //                //	string name = descriptor.Name;
    //                //	object value = descriptor.GetValue(book);
    //                //	Console.WriteLine("{0} = {1}", name, value);
    //                //}
    //                ////Console.WriteLine("--------------------------------------"); //separator for easy reading
    //                //retText = "---";
				//	//count++; // keep count from 1 -> whatever the totalItems is, with a max of 20 of course.
				//}
			}

			return retText;
        }
	}
}
