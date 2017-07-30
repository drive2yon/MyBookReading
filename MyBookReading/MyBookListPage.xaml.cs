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

            this.Title = "本の一覧";
            //const string url = "https://www.xamarin.com/content/images/pages/branding/assets/xamagon.png";
            var books = new List<Book>
            {
                //new Book { Title = "夏への扉", Author = "ロバート・A・ハインライン", ImageUrl = url, Reviews=100, Rating=8},
                //new Book { Title = "星を継ぐもの", Author = "ジェイムズ・P・ホーガン", ImageUrl = url, Reviews=180, Rating=9},
                //new Book { Title = "1984", Author = "ジョージ・オーウェル", ImageUrl = url, Reviews=50, Rating=7},
            };

            this.BindingContext = books;

			ToolbarItems.Add(new ToolbarItem
			{
				Text = "[本の追加]",
				Command = new Command(() => Navigation.PushAsync(new BookSearchPage()))
   			});
			ToolbarItems.Add(new ToolbarItem { Text = "[設定]" });
		

            //test
            //var target = Karamem0.LinqToCalil.Calil.GetCityList();
			//IEnumerable<Karamem0.LinqToCalil.CalilCityListResult> actual = target.AsEnumerable();
			//var actual = target.AsEnumerable();
			//var prefArray = actual.Where(x => x.Pref).Distinct();
            //foreach (var item in actual) {
            //    System.Diagnostics.Debug.WriteLine(item.Pref);
            //}
        }
    }
}
