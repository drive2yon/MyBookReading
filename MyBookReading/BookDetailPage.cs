using System;
using MyBookReading.Web;
using Xamarin.Forms;

namespace MyBookReading
{
    public class BookDetailPage : ContentPage
    {
        public BookDetailPage(SearchResultBook book)
        {
            this.Padding = new Thickness(10);
            var amazonButton = new Button
            {
                Text = "Goto Amazon",
            };
            amazonButton.Clicked += (sender, e) =>
            {
                DependencyService.Get<IWebBrowserService>().Open(new Uri(book.AmazonDetailPageURL));                
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
				}
            };
        }
    }
}

