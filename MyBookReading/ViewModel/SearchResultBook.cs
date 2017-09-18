using System;
using System.Collections.Generic;
using System.ComponentModel;
using Karamem0.LinqToCalil;
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

	public class SearchResultBook : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = delegate { };

        //Amazon
        public string ASIN { get; set; }
        public string AmazonDetailPageURL { get; set; }
        public string SmallImageURL { get; set; }
        public string MediumImageURL { get; set; }
        public string LargeImageURL { get; set; }

		//Calil
        public string CalilUrl { get; set; }   //個別の本のページ
        public string ReserveUrl { get; set; } //図書館の本の予約ページ

		//Coomon
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
		//public string Description { get{book.;} }
		//public string Category { get{book.;} }
        public string ImageUrl { get; set; }

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

        public string ReadingStatus { get; set; }

		/// <summary>
		/// システムIDに紐尽く図書館のキーの配列です。
		/// < 図書館館キー, 貸出状況（「貸出中」、「貸出可」など）>
		/// 蔵書がない場合は、図書館キー自体が配列に含まれない。
		/// </summary>
		/// <value>The libkeys.</value>
		public Dictionary<string, string> Libkeys { set; get; }
        //public Uri CalilUrl { set; get; }   //個別の本のページ
        //public Uri ReserveUrl { set; get; } //図書館の本の予約ページ
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

            //public string Description { get; set; }
            //public string Category { get; set; }

            this.ImageUrl = book.ImageUrl;

            this.ReadingStatus = book.ReadingStatus;

    	}

		public void Update(CalilCheckResult item)
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
	};
}
