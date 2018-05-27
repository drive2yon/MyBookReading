using System.Collections.Generic;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class MyBookListPage : ContentPage
    {
        BookShelf bookShelf = new BookShelf();

		public MyBookListPage()
		{
			InitializeComponent();

			InitList();
		}

		async private void InitList()
		{
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "[本の追加]",
				Command = new Command(() => Navigation.PushAsync(new BookSearchPage(bookShelf)))
			});
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "[設定]",
				Command = new Command(() => Navigation.PushAsync(new SettingPage()))
			});

            this.BindingContext = bookShelf.Books;

			listBook.ItemSelected += async (sender, e) =>
			{
				if (e.SelectedItem == null)
				{
					return;
				}
				((ListView)sender).SelectedItem = null;
                Book item = e.SelectedItem as Book;
				if (item != null)
				{
                    
					await Navigation.PushAsync(new BookDetailPage(bookShelf, item, isRegist:true));
				}
			};

            //GoogleAnalytics
            //GoogleAnalytics.Current.Tracker.SendEvent("MyBookListPage - InitList", "");
		}
   }
}
