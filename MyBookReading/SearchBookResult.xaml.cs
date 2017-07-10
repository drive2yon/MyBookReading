using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace MyBookReading
{
    public partial class SearchBookResult : ContentPage
    {
        public SearchBookResult( string keyword, AmazonCresidentials amazonKey )
        {
            InitializeComponent();

            AmazonBookSearch search = new AmazonBookSearch(amazonKey);
            search.Search(keyword);


			const string url = "https://www.xamarin.com/content/images/pages/branding/assets/xamagon.png";
			var books = new List<Book>
			{
				new Book { Title = "夏への扉", Author = "ロバート・A・ハインライン", ImageUrl = url, Reviews=100, Rating=8},
				new Book { Title = "星を継ぐもの", Author = "ジェイムズ・P・ホーガン", ImageUrl = url, Reviews=180, Rating=9},
				new Book { Title = "1984", Author = "ジョージ・オーウェル", ImageUrl = url, Reviews=50, Rating=7},
			};

			this.BindingContext = books;

		}
    }
}
