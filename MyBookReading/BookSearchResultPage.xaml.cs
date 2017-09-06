using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Karamem0.LinqToCalil;
using MyBookReading.Model;
using Realms;
using Xamarin.Forms;
using System.Threading.Tasks;
using MyBookReading.ViewModel;

namespace MyBookReading
{
    public partial class BookSearchResultPage : ContentPage
    {
        ObservableCollection<SearchResultBook> BookResultList = new ObservableCollection<SearchResultBook>();
        public BookSearchResultPage( IEnumerable<Book> books, bool IsBookSearch )
        {
            InitializeComponent();

            InitList( books, IsBookSearch );
		}

        async private void InitList(IEnumerable<Book> books, bool IsBookSearch)
        {
			if (!IsBookSearch)
			{
				ToolbarItems.Add(new ToolbarItem
				{
					Text = "[本の追加]",
					Command = new Command(() => Navigation.PushAsync(new BookSearchPage()))
				});
				ToolbarItems.Add(new ToolbarItem
				{
					Text = "[設定]",
					Command = new Command(() => Navigation.PushAsync(new SettingPage()))
				});
			}

			foreach (var book in books)
			{
				var bookResult = new SearchResultBook();
				bookResult.InitBook(book);
				BookResultList.Add(bookResult);
			}

			this.BindingContext = BookResultList;

			listBook.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					return;
				}
				((ListView)sender).SelectedItem = null;
				SearchResultBook item = e.SelectedItem as SearchResultBook;
				if (item != null)
				{
					await Navigation.PushAsync(new BookDetailPage(item.book));
				}
			};

            if(IsBookSearch)
            {
				CheckBooks(books);
                //labelPublishedDate.IsVisible = true;
                //labelSearchStatus.IsVisible = true;
                //labelBookStatus.IsVisible = true;
			}
            else
            {
                //labelReadingStatus.IsVisible = true;
            }
		}

        //蔵書検索
        private void CheckBooks(IEnumerable<Book> books)
        {
			CalilCredentials CalilKey = CalilCredentials.LoadCredentialsFile();
			StringBuilder systemidList = new StringBuilder();
            using (var realm = Realm.GetInstance() )
            {
                var librarys = realm.All<CheckTargetLibrary>();
                if(librarys != null && librarys.Count() > 0)
                {
                    foreach(var library in librarys)
                    {
                        if(systemidList.Length == 0)
                        {
                            systemidList.Append(library.systemid);
                        }
                        else
                        {
                            systemidList.Append("," + library.systemid);
                        }
                    }
                }
            }

            StringBuilder bookList = new StringBuilder();
            foreach( Book  book in books)
            {
				if (bookList.Length == 0)
				{
					bookList.Append(book.ISBN);
				}
				else
				{
					bookList.Append("," + book.ISBN);
				}
			}

			string systemid = systemidList.ToString();  //1つのみ
			string isbnList = bookList.ToString();

            Task checkTask = Task.Run(() => {
                Check(systemid, isbnList, CalilKey.api_key); 
            });
		}

        private void Check( string systemid, string isbnList, string apiKey)
        {
			try
			{
				var target = Calil.GetCheck(apiKey);
				var actual = target
				.Where(x => x.SystemId == systemid)
				.Where(x => x.Isbn == isbnList)
					.Polling(r => {
						System.Diagnostics.Debug.WriteLine("Polling:" + DateTime.Now.ToString());
						foreach (var item in r)
						{
							System.Diagnostics.Debug.WriteLine(item);
                            UpdateBook(item);
						}
						return true;
					})
					.AsEnumerable();
				var result = actual.ToList();
				System.Diagnostics.Debug.WriteLine("Completed:" + DateTime.Now.ToString());
				foreach (var item in result)
				{
					System.Diagnostics.Debug.WriteLine(item);
                    UpdateBook(item);
				}

			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception.ToString());
			}            
        }

		private bool UpdateBook(CalilCheckResult item)
		{
			bool bUpdate = false;
			foreach (SearchResultBook book in BookResultList)
			{
				if (book.book.ISBN == item.Isbn)
				{
					bUpdate = true;
					book.Update(item);
					break;
				}
			}
			return bUpdate;
		}
	}
}
