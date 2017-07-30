using System;
using Xamarin.Forms;

namespace MyBookReading
{
    public class Book
    {
        //Amazon
        public string ASIN { get; set; }
        public string AmazonDetailPageURL { get; set; }
        public string SmallImageURL { get; set; }
        public string MediumImageURL { get; set; }
        public string LargeImageURL { get; set; }

        //Coomon
        public string ISBN { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Publisher { get; set; }
        public string PublishedDate { get; set; }
        //public string Description { get; set; }
        //public string Category { get; set; }

        private string _imageUrl { get; set; }
        public string ImageUrl
        {
            get
            {
                return this._imageUrl;
            }
            set
            {
                makeImageSource(value);
                this._imageUrl = value;
            }
        }
        //public string ImageUrl { get; set; }

        public int Rating { get; set; }
        public int Reviews { get; set; }


		public UriImageSource ImageSource { get; set; }  //for image cache
        private void makeImageSource( string url )
        {
            ImageSource = new UriImageSource
            {
				Uri = new Uri(url),
				CachingEnabled = true,
				CacheValidity = new TimeSpan(1, 0, 0, 0)
			};
		}
    }
}
