using System.Collections.Generic;
using System.Linq;
using Realms;

namespace MyBookReading
{
    /// <summary>
    /// 自分の本棚。ここに本を登録するとDBに追加する。
    /// 本ののDB操作はこのクラスに集約する。
    /// </summary>
    public class BookShelf
    {
		readonly Realm _realm;

		public IEnumerable<Book> Books { get; }

		public BookShelf()
		{
			_realm = Realm.GetInstance();
			Books = _realm.All<Book>();
		}

        public Book GetRegistBook(Book book)
        {
            var books = _realm.All<Book>().Where(b => b.ISBN == book.ISBN);
            if(books.Count() == 0)
            {
                return null;
            }
            else
            {
                return books.First();
            }
		}

		/// <summary>
		/// 本を本棚に登録する。登録済みの場合は上書き更新する。
		/// </summary>
		/// <param name="book">Book.</param>
		/// <param name="isRegist">If set to <c>true</c> is regist.</param>
		/// <param name="ReadingStatus">Reading status.</param>
		/// <param name="Note">Note.</param>
		public void SaveBook(Book book, bool isRegist, string ReadingStatus, string Note)
		{
			if (isRegist)
			{
				//登録済みの場合はトランザクションの中でプロパティを更新する
				_realm.Write(() =>
				{
					book.ReadingStatus = ReadingStatus;
				    book.Note = Note;
				});
			}
			else
			{
				book.ReadingStatus = ReadingStatus;
				book.Note = Note;
				_realm.Write(() =>
				{
					_realm.Add(book);
				});
			}
		}
    }

    public class Book : RealmObject
    {
        //Amazon
        public string ASIN { get; set; }
        public string AmazonDetailPageURL { get; set; }
        public string SmallImageURL { get; set; }
        public string MediumImageURL { get; set; }
        public string LargeImageURL { get; set; }

		//Calil
		public string CalilUrl { set; get; }   //個別の本のページ
		public string ReserveUrl { set; get; } //図書館の本の予約ページ

		//Coomon
		public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        public string ImageUrl { get; set; }
        public string ReadingStatus { get; set; }   //既読 未読
        public string Note { get; set; }            //自由メモ
        public string RatingStar { get; set; }      //レート星(未使用)
	}
}
