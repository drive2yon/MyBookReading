using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class BookSearchPage : ContentPage
    {
        Entry bookSearchKeyword;
        Label bookSearchResult;
        public BookSearchPage()
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
			//string GoogleBooksApiKey = "Your Google Books API Key here";
            //maxResults is set to 20 so that we only get a small list
            //            WebRequest request = WebRequest.Create("https://www.googleapis.com/books/v1/volumes?q=" + bookSearchKeyword.Text);
            //			WebResponse response = request.GetResponse();
            string url = "https://www.googleapis.com/books/v1/volumes?q=" + bookSearchKeyword.Text;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync(); // (note: await added, method call changed to GetResponseAsync()


			// Display the status.
			//Console.WriteLine(((HttpWebResponse)response).StatusDescription);
			// Get the stream containing content returned by the server.
			Stream dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			string responseFromServer = reader.ReadToEnd();

            // Clean up the streams and the response.
            //reader.Close();
            //response.Close();
            //response.Close();
            string authorList = googleBooksJsonToText(responseFromServer);
            bookSearchResult.Text = authorList;
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
