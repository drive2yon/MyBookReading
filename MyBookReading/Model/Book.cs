using System.Collections.Generic;
using System.Linq;
using Realms;

namespace MyBookReading
{
    public class BookViewModel
    {
		readonly Realm _realm;

		public IEnumerable<Book> Books { get; }

		public BookViewModel()
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
        //public string Description { get; set; }
        //public string Category { get; set; }

        public string ImageUrl { get; set; }

		//既読 未読
        public string ReadingStatus { get; set; }

		public int Rating { get; set; }
        public int Reviews { get; set; }

		//private string _imageUrl { get; set; }
		//public string ImageUrl
		//{
		//    get
		//    {
		//        return this._imageUrl;
		//    }
		//    set
		//    {
		//        makeImageSource(value);
		//        this._imageUrl = value;
		//    }
		//}


		//public UriImageSource ImageSource { get; set; }  //for image cache
		//      private void makeImageSource( string url )
		//      {
		//          ImageSource = new UriImageSource
		//          {
		//		Uri = new Uri(url),
		//		CachingEnabled = true,
		//		CacheValidity = new TimeSpan(1, 0, 0, 0)
		//	};
		//}

		//   public void Init(Book book)
		//   {
		//       ASIN = book.ASIN;
		//       AmazonDetailPageURL = book.AmazonDetailPageURL;
		//       SmallImageURL = book.SmallImageURL;
		//       MediumImageURL = book.MediumImageURL;
		//       LargeImageURL = book.LargeImageURL;

		//       ISBN = book.ISBN;
		//       Title = book.Title;
		//       Author = book.Author;
		//       Publisher = book.Publisher;
		//       PublishedDate = book.PublishedDate;

		//       ImageUrl = book.ImageUrl;

		//       Rating = book.Rating;
		//       Reviews = book.Reviews;
		//}
	}
}
