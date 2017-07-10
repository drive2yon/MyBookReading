using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class BookSearchPage : ContentPage
    {
        AmazonCresidentials amazonKey;
        private bool bSearchTitle;
        public BookSearchPage()
        {
            amazonKey = LoadCredentialsFile();
            InitializeComponent();
			this.Appearing += (object sender, System.EventArgs e) => this.entryTitle.Focus();
		}

		void Handle_SearchClicked(object sender, System.EventArgs e)
		{
            string keyword;
            if(bSearchTitle){
                keyword = this.entryTitle.Text;
            }else{
                keyword = this.entryAuthor.Text;
            }

            if(keyword.Length == 0)
            {
                DisplayAlert("検索するには？", "１文字以上入力してください", "OK");
                return;
            }
            Navigation.PushAsync( new SearchBookResult(keyword, amazonKey) );
		}

        void Handle_FocusedTitle(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            this.labelTitle.TextColor = Color.Blue;
            this.labelTitle.FontAttributes = FontAttributes.Bold;
            this.labelAuthor.TextColor = Color.Default;
			this.labelAuthor.FontAttributes = FontAttributes.None;
            bSearchTitle = true;
		}

		void Handle_FocusedAuthor(object sender, Xamarin.Forms.FocusEventArgs e)
		{
            this.labelTitle.TextColor = Color.Default;
			this.labelTitle.FontAttributes = FontAttributes.None;
			this.labelAuthor.TextColor = Color.Blue;
			this.labelAuthor.FontAttributes = FontAttributes.Bold;
            bSearchTitle = false;
		}

		private AmazonCresidentials LoadCredentialsFile()
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
			AmazonCresidentials amazon = JsonConvert.DeserializeObject<AmazonCresidentials>(text);
			return amazon;
		}


		private async void OnButtonClicked(object sender, EventArgs e)
		{
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
		}

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
