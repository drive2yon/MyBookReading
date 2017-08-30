using System;
using MyBookReading.Web;
using Realms;
using Xamarin.Forms;

namespace MyBookReading
{
    public class BookDetailPage : ContentPage
    {
        public BookDetailPage(SearchResultBook book)
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
							realm.Add(book.book);
						});
					}
                })
			});

			var amazonButton = new Button
            {
                Text = "Goto Amazon",
            };
            amazonButton.Clicked += (sender, e) =>
            {
                DependencyService.Get<IWebBrowserService>().Open(new Uri(book.book.AmazonDetailPageURL));                
            };

            Content = new StackLayout
            {
                Spacing = 10,
                Children =
                {
                    new Label
                    {
                        Text = book.book.Title,
                        FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                    },
                    new Label
                    {
                        Text = book.book.Author,
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
                                    Uri = new Uri(book.book.ImageUrl),
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
    									Text = book.book.Publisher,
    								},
    								new Label
    								{
    									Text = "発売日",
    								},
    								new Label
    								{
    									Text = book.book.PublishedDate,
    								},
								},
							},
						},
                    },
					amazonButton,
				}
            };
        }
    }
}

