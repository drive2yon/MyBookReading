using System.Collections.Generic;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class MyBookListPage : ContentPage
    {
		BookViewModel bookVM = new BookViewModel();

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
				Command = new Command(() => Navigation.PushAsync(new BookSearchPage()))
			});
			ToolbarItems.Add(new ToolbarItem
			{
				Text = "[設定]",
				Command = new Command(() => Navigation.PushAsync(new SettingPage()))
			});

            this.BindingContext = bookVM.Books;

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
                    
					await Navigation.PushAsync(new BookDetailPage(item));
				}
			};
		}
   }
}
