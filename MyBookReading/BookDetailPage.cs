using System;
using System.Collections.ObjectModel;
using System.Linq;
using MyBookReading.Model;
using MyBookReading.ViewModel;
using MyBookReading.Web;
using Plugin.GoogleAnalytics;
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
        Thickness Margin = new Thickness(0);

		public BookDetailPage(BookShelf bookShelf, Book book, bool isRegist)
        {
            //GA->
            //詳細ページ表示してAmazonページをみる割合を検証する()
            GoogleAnalytics.Current.Tracker.SendView("BookDetailPage");
            //GA<-

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
						bookShelf.SaveBook(book, readingStatus, EntryNote.Text);
                        //GA->
                        //膨大な蔵書を育てる人が多い事の検証。蔵書追加頻度を知りたい
                        GoogleAnalytics.Current.Tracker.SendEvent("BookDetailPage", "AddBook", book.Title);
                        //GA<-
					})
				});
            }
            else
            {
				ToolbarItems.Add(new ToolbarItem
				{
					Text = "[本の登録]",
					Command = new Command(() =>
					{
						string readingStatus = SwitchReading.IsToggled ? "既読" : "未読";
						bookShelf.SaveBook(book, readingStatus, EntryNote.Text);
                        //GA->
                        //膨大な蔵書を育てる人が多い事の検証。蔵書の更新頻度を知りたい
                        GoogleAnalytics.Current.Tracker.SendEvent("BookDetailPage", "UpdateBook", book.Title);
                        //GA<-
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
                Text = "Amazon商品ページ",
            };

            amazonButton.Clicked += (sender, e) =>
            {
                if (Book.AmazonDetailPageURL != null)
                {
                    //GA->
                    //詳細ページ表示してAmazonページをみる割合を検証する(図書館がない場合高い)
                    bool isLibraryBook = false; //図書館に蔵書がある場合true
                    var result = CalilSearch.GetFirstResultBook();
                    if(result != null)
                    {
                        isLibraryBook = result.IsLibraryBook;
                    }
                    GoogleAnalytics.Current.Tracker.SendEvent("BookDetailPage", "OpenUrl - Amazon", isLibraryBook?"図書館蔵書あり":"図書館蔵書なし");
                    //GA<-
                        
                    DependencyService.Get<IWebBrowserService>().Open(new Uri(Book.AmazonDetailPageURL));
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
                VerticalOptions = LayoutOptions.FillAndExpand,
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
                                    Children =
                                    {
                                        new Label
                                        {
                                            Text = "著者 : " + Book.Author,
                                        },
                                        new Label
                                        {
                                            Text = "出版社 : " + Book.Publisher,
                                        },
                                        new Label
                                        {
                                            Text = "発売日 : " + Book.PublishedDate,
                                        },
                                        new Label
                                        {
                                            Text = "本の詳細ページ : ",
                                        },
                                        amazonButton,
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

            var cell = new DataTemplate(typeof(TextCell));
            cell.SetBinding(TextCell.TextProperty, "SystemName");
            cell.SetBinding(TextCell.DetailProperty, "BookHoldingStatus");
            ListView bookSearchlistView = new ListView
            {
                ItemsSource = CalilSearch.GetFirstResultBook().CalilStatusList,
                ItemTemplate = cell,
                HeightRequest = (Librarys.Librarys.Count() )* (Cell.DefaultCellHeight+10),
            };

            bookSearchlistView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
                var item = e.SelectedItem as CalilStatus;
                if (item != null && item.ReserveUrl != null)
                {
                    //GA->
                    //詳細ページ表示して図書館予約ページをみる割合を検証する()
                    GoogleAnalytics.Current.Tracker.SendEvent("BookDetailPage", "OpenUrl - LibraryReserve" );
                    //GA<-

                    DependencyService.Get<IWebBrowserService>().Open(new Uri(item.ReserveUrl));
                }
            };

            bookDetailContent.Children.Add(bookSearchlistView);

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

