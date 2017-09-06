using System;
using System.Collections.Generic;
using System.ComponentModel;
using Karamem0.LinqToCalil;

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

		public Book book;
		//Amazon
		public string ASIN { get { return book.ASIN; } }
		public string AmazonDetailPageURL { get { return book.AmazonDetailPageURL; } }
		public string SmallImageURL { get { return book.SmallImageURL; } }
		public string MediumImageURL { get { return book.MediumImageURL; } }
		public string LargeImageURL { get { return book.LargeImageURL; } }

		//Calil
		public string CalilUrl { get { return book.CalilUrl; } }   //個別の本のページ
		public string ReserveUrl { get { return book.ReserveUrl; } } //図書館の本の予約ページ

		//Coomon
		public string ISBN { get { return book.ISBN; } }
		public string Title { get { return book.Title; } }
		public string Author { get { return book.Author; } }
		public string Publisher { get { return book.Publisher; } }
		public string PublishedDate { get { return book.PublishedDate; } }
		//public string Description { get{book.;} }
		//public string Category { get{book.;} }
		public string ImageUrl { get { return book.ImageUrl; } }

		public byte readingStatus { set; get; }
		public string ReadingStatus
		{
			get
			{
				switch (readingStatus)
				{
					case 0:
						return "未読";
					case 1:
						return "既読";
					default:
						return "未読";
				}
			}
		}

		/// <summary>
		/// システムIDに紐尽く図書館のキーの配列です。
		/// < 図書館館キー, 貸出状況（「貸出中」、「貸出可」など）>
		/// 蔵書がない場合は、図書館キー自体が配列に含まれない。
		/// </summary>
		/// <value>The libkeys.</value>
		public Dictionary<string, string> Libkeys { set; get; }  //
																 //public Uri CalilUrl { set; get; }   //個別の本のページ
																 //public Uri ReserveUrl { set; get; } //図書館の本の予約ページ
		public CheckStatus SearchStatus { set; get; }
		public string StatusString { private set; get; }
		public string SystemId { set; get; }
		public string BookStatus
		{
			get
			{
				if (SearchStatus == CheckStatus.Running)
				{
					return "蔵書検索中";
				}
				else if (SearchStatus == CheckStatus.Error)
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
			this.book = book;
			//base.Init(book);
		}

		public void Update(CalilCheckResult item)
		{
			if (CalilUrl != null)
			{
				book.CalilUrl = item.CalilUrl.ToString();
			}
			Libkeys = item.Libkeys;
			if (item.ReserveUrl != null)
			{
				book.ReserveUrl = item.ReserveUrl.ToString();
			}
			SearchStatus = ConvertStatus(item.Status);
			SystemId = item.SystemId;
			PropertyChanged(this, new PropertyChangedEventArgs("Libkeys"));
			PropertyChanged(this, new PropertyChangedEventArgs("SearchStatus"));
			PropertyChanged(this, new PropertyChangedEventArgs("BookStatus"));
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
