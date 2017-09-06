using System;
using MyBookReading.Web;
using Realms;
using Xamarin.Forms;

namespace MyBookReading
{
    public class BookDetailPage : ContentPage
    {
        public BookDetailPage(Book book)
        {
            this.Padding = new Thickness(10);


			ToolbarItems.Add(new ToolbarItem
			{
				Text = "[本の登録]",
				Command = new Command(() =>
                {
					using (var realm = Realm.GetInstance())
					{
						realm.Write(() =>
						{
							realm.Add(book);
						});
					}
                })
			});

			var amazonButton = new Button
            {
                Text = "Goto Amazon",
            };
			var libraryButton = new Button
			{
				Text = "Goto 図書館予約ページ",
			};
			var CalilButton = new Button
			{
				Text = "Goto Calilページ",
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
					amazonButton,
                    libraryButton,
                    CalilButton,
				}
            };
        }
    }
}

