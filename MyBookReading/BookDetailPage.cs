using System;
using MyBookReading.Web;
using Realms;
using Xamarin.Forms;

namespace MyBookReading
{
    public class BookDetailPage : ContentPage
    {
        Label LabelNoReading;
        Label LabelReading;
        Switch SwitchReading;
        Editor EditorNote;
        Book book;

		public BookDetailPage(BookShelf bookShelf, Book book, bool isRegist)
        {
            this.Padding = new Thickness(10);
            this.book = book;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "[本の登録]",
				Command = new Command(() =>
                {
                    string readingStatus = SwitchReading.IsToggled ? "既読" : "未読";
                    bookShelf.SaveBook(book, isRegist, readingStatus, EditorNote.Text);
                })
			});

			var amazonButton = new Button
            {
                Text = "Amazon",
            };
			var libraryButton = new Button
			{
				Text = "図書館予約",
			};
			var CalilButton = new Button
			{
				Text = "Calil",
			};

			amazonButton.Clicked += (sender, e) =>
            {
                if(book.AmazonDetailPageURL != null)
                {
					DependencyService.Get<IWebBrowserService>().Open(new Uri(book.AmazonDetailPageURL));
				}
            };
			libraryButton.Clicked += (sender, e) =>
			{
                if (book.ReserveUrl != null)
                {
                    DependencyService.Get<IWebBrowserService>().Open(new Uri(book.ReserveUrl));
                }
			};
			CalilButton.Clicked += (sender, e) =>
			{
                if(book.CalilUrl != null)
                {
					DependencyService.Get<IWebBrowserService>().Open(new Uri(book.CalilUrl));
				}
			};

            LabelNoReading = new Label
            {
                Text = "未読",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            LabelReading = new Label
            {
                Text = "既読",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            SwitchReading = new Switch
            {
                IsToggled = book.ReadingStatus != null ? book.ReadingStatus.Equals("既読") : false,
                HorizontalOptions = LayoutOptions.End,
            };
            UpdateReadingToggle(SwitchReading.IsToggled);

			SwitchReading.Toggled += (sender, e) =>
			{
                UpdateReadingToggle(e.Value);
			};

            EditorNote = new Editor
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Text = book.Note == null ? "" : book.Note,
            };


            ScrollView scrollView = new ScrollView
            {
				VerticalOptions = LayoutOptions.FillAndExpand,
				Content = new StackLayout
				{
					Spacing = 10,
					Children =
    				{
    					new Label
    					{
    						Text = book.Title,
    						FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
    					},
    					new Label
    					{
    						Text = book.Author,
    						FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
    					},

    					new StackLayout
    					{
    						Orientation = StackOrientation.Horizontal,
    						Children =
    						{
    							new Image
    							{
    								Source = new UriImageSource{
    									Uri = new Uri(book.ImageUrl),
    									CachingEnabled = true
    								},
    								WidthRequest = 100,
    								HeightRequest = 160,
    								HorizontalOptions = LayoutOptions.Start,
    								VerticalOptions = LayoutOptions.Start,
    								Aspect = Aspect.AspectFit,
    							},

    							new StackLayout
    							{
    								Spacing = 10,
    								Children =
    								{
    									new Label
    									{
    										Text = "出版社",
    									},
    									new Label
    									{
    										Text = book.Publisher,
    									},
    									new Label
    									{
    										Text = "発売日",
    									},
    									new Label
    									{
    										Text = book.PublishedDate,
    									},
    								},
    							},
    						},
    					},
                        new Label
                        {
                            Text = "詳細ページ",
                        },
						new StackLayout
                        {
							Orientation = StackOrientation.Horizontal,
                            Children = 
                            {
        						amazonButton,
        						libraryButton,
        						CalilButton,
							},
						},
						new StackLayout
    					{
    						Orientation = StackOrientation.Horizontal,
    						Children =
    						{
    							LabelNoReading,
    							new Label
    							{
    								Text = " / ",
    								FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
    							},
    							LabelReading,
    							SwitchReading,
    						},
    					},
    					new Label
    					{
    						Text = "読書メモ",
    						FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
    					},
                        EditorNote,
    				}
				},
			};
            Content = scrollView;


        }
        void UpdateReadingToggle(bool bReading)
        {
			if (bReading)
			{
				LabelNoReading.TextColor = Color.Gray;
				LabelReading.TextColor = Color.Blue;
			}
			else
			{
				LabelNoReading.TextColor = Color.Blue;
				LabelReading.TextColor = Color.Gray;
			}
		}
    }
}

