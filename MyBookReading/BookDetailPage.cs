using System;
using System.Collections.ObjectModel;
using MyBookReading.Model;
using MyBookReading.ViewModel;
using MyBookReading.Web;
using Xamarin.Forms;

namespace MyBookReading
{
    public class BookDetailPage : ContentPage
    {
        SearchResultVM CalilSearch;
        CheckTargetLibrarys Librarys = new CheckTargetLibrarys();

        Label LabelNoReading;
        Label LabelReading;
        Switch SwitchReading;
        Entry EntryNote;
        Book Book;
        Label LabelLibraryStatus;

        Thickness Margin = new Thickness(0);



		public BookDetailPage(BookShelf bookShelf, Book book, bool isRegist)
        {
            this.Book = book;
			CalilSearch = new SearchResultVM { BookResultList = GetSearchResultBookList(this.Book) };

			if(!isRegist)
            {
				ToolbarItems.Add(new ToolbarItem
				{
					Text = "[本の登録]",
					Command = new Command(() =>
					{
						string readingStatus = SwitchReading.IsToggled ? "既読" : "未読";
						bookShelf.SaveBook(book, isRegist, readingStatus, EntryNote.Text);
					})
				});
            }
            else
            {
				ToolbarItems.Add(new ToolbarItem
				{
					Text = "[本の保存]",
					Command = new Command(() =>
					{
						string readingStatus = SwitchReading.IsToggled ? "既読" : "未読";
						bookShelf.SaveBook(book, isRegist, readingStatus, EntryNote.Text);
					})
				});
				ToolbarItems.Add(new ToolbarItem
				{
					Text = "[本の削除]",
					Command = new Command(async () =>
					{
						bool ret = await DisplayAlert("本の削除", "本を削除します", "OK", "キャンセル");
                        if (!ret)
						{
						    return;
						}

						bookShelf.DeleteBook(book);
                        await Navigation.PopAsync(true);
					})
				});
			}

            Label labelFooter = new Label
            {
                Style = Application.Current.Resources["FooterLabelStyle"] as Style,
                BindingContext = CalilSearch,
            };
            labelFooter.SetBinding(Label.TextProperty, "Status");

            ScrollView scrollView = new ScrollView
            {
				Margin = new Thickness(10, 0),
			    VerticalOptions = LayoutOptions.FillAndExpand,
				Content = GetBookDetailContent(),
			};


            Content = new StackLayout{
                Children = {
                    scrollView,
                    labelFooter,
                },
            };

			this.BindingContext = CalilSearch;
            CheckLibrary();
		}

        async private void CheckLibrary()
        {
            Collection<Book> books = new Collection<Book>
            {
                this.Book
            };
            CalilSearch.CheckBooks(books);
        }

        ObservableCollection<ViewModel.SearchResultBook> GetSearchResultBookList( Book book )
        {
			ObservableCollection<ViewModel.SearchResultBook> resultList = new ObservableCollection<ViewModel.SearchResultBook>();
			{
				var bookResult = new ViewModel.SearchResultBook(book);
				resultList.Add(bookResult);
			}
            return resultList;
		}


        StackLayout GetBookDetailContent()
        {
            var labelStyleDetailContent = new Style(typeof(Label))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty, Value = Color.Yellow },
                }
            };


            var amazonButton = new Button
            {
                Text = "Amazon",
            };
            var CalilButton = new Button
            {
                Text = "Calil",
            };

            amazonButton.Clicked += (sender, e) =>
            {
                if (Book.AmazonDetailPageURL != null)
                {
                    DependencyService.Get<IWebBrowserService>().Open(new Uri(Book.AmazonDetailPageURL));
                }
            };
            CalilButton.Clicked += (sender, e) =>
            {
                if (Book.CalilUrl != null)
                {
                    DependencyService.Get<IWebBrowserService>().Open(new Uri(Book.CalilUrl));
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
                IsToggled = Book.ReadingStatus != null ? Book.ReadingStatus.Equals("既読") : false,
                HorizontalOptions = LayoutOptions.End,
            };
            UpdateReadingToggle(SwitchReading.IsToggled);

            SwitchReading.Toggled += (sender, e) =>
            {
                UpdateReadingToggle(e.Value);
            };

            EntryNote = new Entry
            {
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                Text = Book.Note == null ? "" : Book.Note,
            };
            StackLayout bookDetailContent = new StackLayout
            {
                Spacing = 10,
                Children =

                    {
                        new Label
                        {
                            Text = Book.Title,
                            FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                            Margin = this.Margin,
                        },

                        new Label
                        {
                            Text = "書籍詳細",
                            Style = labelStyleDetailContent,
                            Margin = this.Margin,
                        },
                       new StackLayout
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children =
                            {
                                new Image
                                {
                                    Source = new UriImageSource{
                                        Uri = new Uri(Book.ImageUrl),
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
                                            Text = "著者",
                                        },
                                        new Label
                                        {
                                            Text = Book.Author,
                                        },
                                        new Label
                                        {
                                            Text = "出版社",
                                        },
                                        new Label
                                        {
                                            Text = Book.Publisher,
                                        },
                                        new Label
                                        {
                                            Text = "発売日",
                                        },
                                        new Label
                                        {
                                            Text = Book.PublishedDate,
                                        },
                                    },
                                },
                            },
                        },
                    }
            };

            bookDetailContent.Children.Add(
                new Label
                {
                    Text = "図書館 - 蔵書状況",
                    Style = labelStyleDetailContent,
                    Margin = this.Margin,
                });

            SearchResultBook resultBook = new SearchResultBook(this.Book);
            foreach(var bookResult in CalilSearch.BookResultList )
            {
                resultBook = bookResult;
                break;
            }
			//登録図書館は１件前提
			foreach( var library in Librarys.Librarys)
            {
				Label LabelLibrary = new Label
				{
					Text = library.systemname,
					FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
					Margin = this.Margin,
				};

                LabelLibraryStatus = new Label
                {
					FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
					Margin = this.Margin,
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                    BindingContext = resultBook,
				};
                LabelLibraryStatus.SetBinding(Label.TextProperty, "CalilStatus");

				StackLayout libraryStatus = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        LabelLibrary,
                        LabelLibraryStatus,
                    }
                };

				var tgr = new TapGestureRecognizer();
				tgr.Tapped += (sender, e) => 
                {
					foreach(var bookResult in CalilSearch.BookResultList )
					{
					    if(bookResult.ReserveUrl != null)
					    {
							DependencyService.Get<IWebBrowserService>().Open(new Uri(bookResult.ReserveUrl));
						}
                        break;
					}

				};
				libraryStatus.GestureRecognizers.Add(tgr);

				bookDetailContent.Children.Add(libraryStatus);
                break;
			}

            bookDetailContent.Children.Add(
                new Label
                {
                    Text = "詳細ページ",
					Style = labelStyleDetailContent,
					Margin = this.Margin,
				});
            bookDetailContent.Children.Add(
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        amazonButton,
                        CalilButton,
                    },
                });
			bookDetailContent.Children.Add(
				new Label
				{
					Text = "読了状況",
					Style = labelStyleDetailContent,
					Margin = this.Margin,
				});
			bookDetailContent.Children.Add(
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
					Margin = this.Margin,
				});
            bookDetailContent.Children.Add(
                new Label
                {
                    Text = "読書メモ",
					Style = labelStyleDetailContent,
					Margin = this.Margin,
				});
			bookDetailContent.Children.Add(
				EntryNote);

            return bookDetailContent;
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

