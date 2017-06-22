using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class MyBookListPage : ContentPage
    {
        public MyBookListPage()
        {
			InitializeComponent();

            const string url = "http://xamarin.com/images/index/ide-xamarin-studio.png";
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
