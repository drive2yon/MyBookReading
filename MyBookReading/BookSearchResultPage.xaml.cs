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
using System.ComponentModel;

namespace MyBookReading
{
    class SearchResultVM : INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged = delegate { };
	
        public ObservableCollection<ViewModel.SearchResultBook> BookResultList { get; set; }
        public string Intro { get { return "Monkey Header"; } }
        public string Summary { get { return " There were " + BookResultList.Count + " monkeys"; } }
        public string Status { get; private set; }
        public void UpdateStatus(string value)
        {
            Status = value;
			PropertyChanged(this, new PropertyChangedEventArgs("Status"));
		}
    }

    public partial class BookSearchResultPage : ContentPage
    {
        SearchResultVM SearchResultVM;
        public BookSearchResultPage(AmazonBookSearch search, ObservableCollection<Book> books)
        {
            InitializeComponent();

            InitList(books);
        }

        async private void InitList(ObservableCollection<Book> books)
        {
            ObservableCollection<ViewModel.SearchResultBook> resultList = new ObservableCollection<ViewModel.SearchResultBook>();
            {

                //本棚に登録済みか判定する
                BookViewModel bookVM = new BookViewModel();
                foreach (var book in books)
                {
                    var bookResult = new ViewModel.SearchResultBook();


                    Book registBook = bookVM.GetRegistBook(book);
                    if (registBook != null)
                    {
                        bookResult.InitBook(registBook);
                        bookResult.IsRegistBookShelf = true;
                    }
                    else
                    {
                        bookResult.InitBook(book);
                        bookResult.IsRegistBookShelf = false;
                    }
                    resultList.Add(bookResult);
                }
            }

            SearchResultVM = new SearchResultVM { BookResultList = resultList };
            this.BindingContext = SearchResultVM;

            listBook.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
                ViewModel.SearchResultBook item = e.SelectedItem as ViewModel.SearchResultBook;
                if (item != null)
                {
                    //詳細ページを開くためのBookインスタンスの撮り方は
                    //await Navigation.PushAsync(new BookDetailPage(item.book));
                }
            };

            CheckBooks(books);
        }

        //Calil検索用に設定済み図書館のIDを文字列で取得する
        private string GetSystemID()
        {
            StringBuilder systemidList = new StringBuilder();
            using (var realm = Realm.GetInstance())
            {
                var librarys = realm.All<CheckTargetLibrary>();
                if (librarys == null || librarys.Count() == 0)
                {
                    return null;
                }
                foreach (var library in librarys)
                {
                    if (systemidList.Length == 0)
                    {
                        systemidList.Append(library.systemid);
                    }
                    else
                    {
                        systemidList.Append("," + library.systemid);
                    }
                }
            }
            return systemidList.ToString();
        }

        //蔵書検索
        private void CheckBooks(IEnumerable<Book> books)
        {
            CalilCredentials CalilKey = CalilCredentials.LoadCredentialsFile();
            string systemid = GetSystemID();
            if (systemid == null) //図書館が未設定の場合は、蔵書検索を行わない
            {
                SearchResultVM.UpdateStatus("表示完了");
                return;
            }

            StringBuilder bookList = new StringBuilder();
            foreach (Book book in books)
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

            string isbnList = bookList.ToString();

            Task checkTask = Task.Run(() =>
            {
                Check(systemid, isbnList, CalilKey.api_key);
            });
        }

        private void Check(string systemid, string isbnList, string apiKey)
        {
            try
            {
                SearchResultVM.UpdateStatus("図書館[" + systemid + "]の蔵書を検索中");
                var target = Calil.GetCheck(apiKey);
                var actual = target
                .Where(x => x.SystemId == systemid)
                .Where(x => x.Isbn == isbnList)
                    .Polling(r =>
                    {
                        System.Diagnostics.Debug.WriteLine("Polling:" + DateTime.Now.ToString());
                        foreach (var item in r)
                        {
                            UpdateBook(item);
                        }
                        return true;
                    })
                    .AsEnumerable();
                if(actual != null)
                {
					var result = actual.ToList();
					System.Diagnostics.Debug.WriteLine("Completed:" + DateTime.Now.ToString());
					foreach (var item in result)
					{
						System.Diagnostics.Debug.WriteLine(item);
						UpdateBook(item);
					}
                    SearchResultVM.UpdateStatus("図書館の蔵書検索が完了しました");

				}
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine(exception.ToString());
                SearchResultVM.UpdateStatus("図書館の蔵書が失敗しました");
            }
        }

        private bool UpdateBook(CalilCheckResult item)
        {
            bool bUpdate = false;
            foreach (ViewModel.SearchResultBook book in SearchResultVM.BookResultList)
            {
                if (book.ISBN == item.Isbn)
                {
                    bUpdate = true;
                    book.Update(item);
                    if( item.Status == CheckState.Running )
                    {
						SearchResultVM.UpdateStatus("「" + book.Title + "」の蔵書検索中");
						System.Diagnostics.Debug.WriteLine(SearchResultVM.Status);
					}
                    break;
                }
            }
            return bUpdate;
        }
    }
}
