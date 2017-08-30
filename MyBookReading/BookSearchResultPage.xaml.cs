using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Karamem0.LinqToCalil;
using MyBookReading.Model;
using Realms;
using Xamarin.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyBookReading
{
	public enum CheckStatus
	{
        None,
		OK,
		Cache,
		Running,
		Error
	}    
    public class SearchResultBook : Book, INotifyPropertyChanged
    {
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		/// <summary>
		/// システムIDに紐尽く図書館のキーの配列です。
		/// < 図書館館キー, 貸出状況（「貸出中」、「貸出可」など）>
		/// 蔵書がない場合は、図書館キー自体が配列に含まれない。
		/// </summary>
		/// <value>The libkeys.</value>
		public Dictionary<string, string> Libkeys { set; get; }  //
		public Uri CalilUrl { set; get; }   //個別の本のページ
        public Uri ReserveUrl { set; get; } //本の予約ページ
        public CheckStatus SearchStatus { set; get; }
        public string StatusString { private set; get; }
        public string SystemId { set; get; }
        public string BookStatus
        {
            get
            {
                if(SearchStatus == CheckStatus.Running)
                {
                    return "蔵書検索中";
                }
                else if(SearchStatus == CheckStatus.Error)
                {
					return "蔵書検索失敗";
				}
                else
                {
					if (Libkeys == null || Libkeys.Count == 0)
					{
						return "蔵書なし";
					}
					else
					{
						return "蔵書あり";
					}
				}
            }
        }

        public void InitBook(Book book)
        {
            base.Init(book);
        }

		public void Update(CalilCheckResult item)
		{
			CalilUrl = item.CalilUrl;
			Libkeys = item.Libkeys;
			ReserveUrl = item.ReserveUrl;
			SearchStatus = ConvertStatus(item.Status);
			SystemId = item.SystemId;
			PropertyChanged(this, new PropertyChangedEventArgs("Libkeys"));
			PropertyChanged(this, new PropertyChangedEventArgs("SearchStatus"));
			PropertyChanged(this, new PropertyChangedEventArgs("BookStatus"));
            System.Diagnostics.Debug.WriteLine("Update Book by Result. " + Title + "SearchStatus=" + SearchStatus.ToString());

		}

		private CheckStatus ConvertStatus(CheckState state)
		{
			switch (state)
			{
				case CheckState.OK: return CheckStatus.OK;
				case CheckState.Cache: return CheckStatus.Cache;
				case CheckState.Running: return CheckStatus.Running;
				case CheckState.Error: return CheckStatus.Error;
				default: return CheckStatus.None;
			}
		}

	};

    public partial class BookSearchResultPage : ContentPage
    {
        CalilCredentials CalilKey;
        ObservableCollection<SearchResultBook> BookResultList = new ObservableCollection<SearchResultBook>();
        public BookSearchResultPage( string keyword, AmazonCredentials amazonKey, ObservableCollection<Book> books )
        {
            InitializeComponent();

            foreach(var book in books)
            {
                var bookResult = new SearchResultBook();
                bookResult.InitBook(book);
                BookResultList.Add(bookResult);
            }

			this.BindingContext = BookResultList;

            CalilKey = CalilCredentials.LoadCredentialsFile();

            list.ItemSelected += async (sender, e) =>
            {
				if (e.SelectedItem == null)
				{
					return;
				}
				((ListView)sender).SelectedItem = null;

                await Navigation.PushAsync(new BookDetailPage((SearchResultBook)(e.SelectedItem)));				
			};

    		CheckBooks(books);
		}

        //蔵書検索
        private void CheckBooks(ObservableCollection<Book> books)
        {
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
				if (book.ISBN == item.Isbn)
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
