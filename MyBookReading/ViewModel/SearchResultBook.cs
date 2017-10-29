﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Karamem0.LinqToCalil;
using MyBookReading.Model;
using Realms;
using Xamarin.Forms;

namespace MyBookReading.ViewModel
{


	public enum CheckStatus
	{
		None,
		OK,
		Cache,
		Running,
		Error
	}

    public class CalilStatus
    {
        public string SystemId { get; set; }            //図書館Id
        public string SystemName { get; set; }            //図書館Name
        public CheckStatus CheckStatus { get; set; }    //Calil検索状況
        public string ReserveUrl { get; set; }          //蔵書がある場合:予約URL 蔵書がない場合:null
        public Dictionary<string, string> Libkeys { set; get; } //System内の図書館個別の蔵書状況

        public string BookHoldingStatus
        {
            get
            {
                if (CheckStatus == CheckStatus.Running)
                {
                    return "図書館の蔵書検索中";
                }
                else if (CheckStatus == CheckStatus.Error)
                {
                    return "図書館の蔵書検索失敗";
                }
                else //OK or Cached
                {
                    if (ReserveUrl == null ||
                        Libkeys == null ||
                        Libkeys.Count() == 0)
                    {
                        return "図書館に蔵書なし";
                    }
                    else
                    {
                        return "図書館に蔵書あり(図書館予約ページへ)";
                    }
                }
            }
        }
    }

	public class SearchResultBook : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public SearchResultBook(Book book)
        {
			//本棚に登録済みか判定する
			BookShelf bookVM = new BookShelf();
			Book registBook = bookVM.GetRegistBook(book);
            CalilStatusList = new ObservableCollection<ViewModel.CalilStatus>();
			if (registBook != null)
			{
				this.InitBook(registBook);
				this.IsRegistBookShelf = true;
			}
			else
			{
				this.InitBook(book);
				this.IsRegistBookShelf = false;
			}
		}

        //Amazon
        public string ASIN { get; set; }
        public string AmazonDetailPageURL { get; set; }
        public string SmallImageURL { get; set; }
        public string MediumImageURL { get; set; }
        public string LargeImageURL { get; set; }

		//Calil
        public string CalilUrl { get; set; }   //個別の本のページ
        public string ReserveUrl { get; set; } //図書館の本の予約ページ

        public ObservableCollection<CalilStatus> CalilStatusList { get; set; }

		//Coomon
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
		//public string Description { get{book.;} }
		//public string Category { get{book.;} }
        public string ImageUrl { get; set; }
		public string ReadingStatus { get; set; }   //既読 未読
		public string Note { get; set; }            //自由メモ
		public string RatingStar { get; set; }      //レート星

		//for SearchResult
		public bool IsRegistBookShelf { get; set; }
        public string SearchBookStatus 
        {
            get
            {
                if(IsRegistBookShelf)
                {
                    return "本棚に登録済み";
                }
                else
                {
                    if(SearchStatus == CheckStatus.None)    //図書館が未登録の場合は出版社と出版日を表示する
                    {
                        return Publisher + " " + PublishedDate;
                    }
                    else
                    {
						return CalilStatus;
					}
                }
            }
        }
		public bool IsLibraryBook
        {
            get
            {
                return (Libkeys != null && Libkeys.Count > 0);
            }
        }


		public Color SearchBookStatusColor 
        { 
            get
            {
                if (IsRegistBookShelf)
                {
                    return Color.Red;
                }
                else
                {
                    if(SearchStatus == CheckStatus.None)
                    {
                        return Color.Navy;
                    }
                    else
                    {
                        if (IsLibraryBook)
                        {
                            return Color.Navy;
                        }
                        else
                        {
                            return Color.Gray;
                        }
					}
                }
            }
        }


		/// <summary>
		/// システムIDに紐付く図書館のキーの配列です。
		/// < 図書館館キー, 貸出状況（「貸出中」、「貸出可」など）>
		/// 蔵書がない場合は、図書館キー自体が配列に含まれない。
		/// </summary>
		/// <value>The libkeys.</value>
		public Dictionary<string, string> Libkeys { set; get; }
		public CheckStatus SearchStatus { set; get; }
		public string StatusString { private set; get; }
		public string SystemId { set; get; }
		public string CalilStatus
		{
			get
			{
				if (SearchStatus == CheckStatus.Running)
				{
					return "図書館の蔵書検索中";
				}
				else if (SearchStatus == CheckStatus.Error)
				{
					return "図書館の蔵書検索失敗";
				}
				else //OK or Cached
				{
					if (Libkeys == null || Libkeys.Count == 0)
					{
						return "図書館に蔵書なし";
					}
					else
					{
						return "図書館に蔵書あり";
					}
				}
			}
		}

		public void InitBook(Book book)
		{
            //RealmObjectはスレッド間の受け渡しができない制限があるため、値コピーする
            //this.book = book; -> NG
            this.ASIN = book.ASIN;
            this.AmazonDetailPageURL = book.AmazonDetailPageURL;
            this.SmallImageURL = book.SmallImageURL;
            this.MediumImageURL = book.MediumImageURL;
            this.LargeImageURL = book.LargeImageURL;

            this.CalilUrl = book.CalilUrl;
            this.ReserveUrl = book.ReserveUrl;

            this.ISBN = book.ISBN;
            this.Title = book.Title;
            this.Author = book.Author;
            this.Publisher = book.Publisher;
            this.PublishedDate = book.PublishedDate;
            this.ImageUrl = book.ImageUrl;
            this.ReadingStatus = book.ReadingStatus;
            this.Note = book.Note;
            this.RatingStar = book.RatingStar;

    	}

        private void UpdateCalilStatusList(CalilStatus status)
        {
            //ステータスが確定してからリスト表示する
            if(status.CheckStatus == CheckStatus.Running ||
               status.CheckStatus == CheckStatus.None )
            {
                return;
            }

            //リストに登録済みなら除外
            foreach(var target in CalilStatusList)
            {
                if(target.SystemId == status.SystemId)
                {
                    return;
                }
            }

            //未登録ならリスト追加
            CalilStatusList.Add(status);
        }

		public void Update(CalilCheckResult item, string systemName)
		{
			if (CalilUrl != null)
			{
			    this.CalilUrl = item.CalilUrl.ToString();
			}
			Libkeys = item.Libkeys;
			if (item.ReserveUrl != null)
			{
				this.ReserveUrl = item.ReserveUrl.ToString();
			}
			SearchStatus = ConvertStatus(item.Status);
			SystemId = item.SystemId;

            //図書館ごとの蔵書状況をメンバに追加する
            CalilStatus status = new CalilStatus();
            status.SystemId = item.SystemId;
            status.SystemName = systemName;
            if( item.ReserveUrl != null)
            {
                status.ReserveUrl = item.ReserveUrl.ToString();
            }
            status.Libkeys = Libkeys;
            status.CheckStatus = ConvertStatus(item.Status);
            UpdateCalilStatusList(status);


			PropertyChanged(this, new PropertyChangedEventArgs("ReserveUrl"));
			PropertyChanged(this, new PropertyChangedEventArgs("Libkeys"));
			PropertyChanged(this, new PropertyChangedEventArgs("SearchStatus"));
			PropertyChanged(this, new PropertyChangedEventArgs("CalilStatus"));
			PropertyChanged(this, new PropertyChangedEventArgs("SearchBookStatus"));
			PropertyChanged(this, new PropertyChangedEventArgs("SearchBookStatusColor"));
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

        public Book CreateBook()
        {
            return new Book()
            {
                ASIN = this.ASIN,
                AmazonDetailPageURL = this.AmazonDetailPageURL,
                SmallImageURL = this.SmallImageURL,
                MediumImageURL = this.MediumImageURL,
                LargeImageURL = this.LargeImageURL,

                //CalilUrl = this.CalilUrl,
                //ReserveUrl = this.ReserveUrl,

                ISBN = this.ISBN,
                Title = this.Title,
                Author = this.Author,
                Publisher = this.Publisher,
                PublishedDate = this.PublishedDate,
                ImageUrl = this.ImageUrl,
                ReadingStatus = this.ReadingStatus,
                Note = this.Note,
                RatingStar = this.RatingStar
            };
            
        }
	};

	class SearchResultVM : INotifyPropertyChanged
	{
        CheckTargetLibrarys Librarys = new CheckTargetLibrarys();

		public event PropertyChangedEventHandler PropertyChanged = delegate { };

		public ObservableCollection<SearchResultBook> BookResultList { get; set; }
		public string Intro { get { return "Monkey Header"; } }
		public string Summary { get { return " There were " + BookResultList.Count + " monkeys"; } }
		public string Status { get; private set; }
		public void UpdateStatus(string value)
		{
			Status = value;
			PropertyChanged(this, new PropertyChangedEventArgs("Status"));
		}

		//蔵書検索
		public void CheckBooks(IEnumerable<Book> books)
		{
			CalilCredentials CalilKey = CalilCredentials.LoadCredentialsFile();
			string systemid = Librarys.GetSystemIDList();
			if (systemid == null) //図書館が未設定の場合は、蔵書検索を行わない
			{
				this.UpdateStatus("表示完了");
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

            System.Threading.Tasks.Task checkTask = System.Threading.Tasks.Task.Run(() =>
			{
				Check(systemid, isbnList, CalilKey.api_key);
			});
		}

		private void Check(string systemid, string isbnList, string apiKey)
		{
			try
			{
				this.UpdateStatus("図書館[" + systemid + "]の蔵書を検索中");
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
				if (actual != null)
				{
					var result = actual.ToList();
					System.Diagnostics.Debug.WriteLine("Completed:" + DateTime.Now.ToString());
					foreach (var item in result)
					{
						System.Diagnostics.Debug.WriteLine(item);
						UpdateBook(item);
					}
					this.UpdateStatus("図書館の蔵書検索が完了しました");

				}
			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception.ToString());
				this.UpdateStatus("図書館の蔵書が失敗しました");
			}
		}

		private bool UpdateBook(CalilCheckResult item)
		{
			bool bUpdate = false;
			foreach (ViewModel.SearchResultBook book in this.BookResultList)
			{
				if (book.ISBN == item.Isbn)
				{
					bUpdate = true;

                    string systemName;
                    {
                        CheckTargetLibrarys lib = new CheckTargetLibrarys();
                        systemName = lib.GetSystemName(item.SystemId);
                    }

                    book.Update(item, systemName);
					if (item.Status == CheckState.Running)
					{
						this.UpdateStatus("「" + book.Title + "」の蔵書検索中");
						System.Diagnostics.Debug.WriteLine(this.Status);
					}
					break;
				}
			}
			return bUpdate;
		}

        public SearchResultBook GetFirstResultBook()
        {
            if(BookResultList == null)
            {
                return null;
            }
            return BookResultList.First();
        }
	}

}
