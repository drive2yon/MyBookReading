using System;
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
		//public string ISBN { get; set; }
		public string Title { get; set; }
		public string Author { get; set; }
		public string Publisher { get; set; }
		//public string PublishedDate { get; set; }
		//public string Description { get; set; }
		//public string Category { get; set; }
		public string ImageUrl { get; set; }
        public int Rating { get; set; }
        public int Reviews { get; set; }
    }
}
