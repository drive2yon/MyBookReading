using System;
using System.Collections.Generic;
using System.Linq;
using MyBookReading.Model;
using Newtonsoft.Json;
using Realms;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class SettingPage : ContentPage
    {
        public interface IBookShelf
        {
            string SaveFile(string filename, string dataText);
            string GetFilePath(string filename);
            string LoadFile(string filepath);
            bool IsExistFile(string filepath);
        }

		class LibraryCell : ViewCell
		{
            public LibraryCell()
			{
                Label deleteLabel = new Label()
                {
                    Text = "[削除]",
					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
				};

                var tgr = new TapGestureRecognizer();
                tgr.Tapped += (sender, e) =>
                {
                    CheckTargetLibrarys targetLibrarys = new CheckTargetLibrarys();
                    targetLibrarys.DelLibrary((CheckTargetLibrary)BindingContext);
                };
				deleteLabel.GestureRecognizers.Add(tgr);

				//systemname
				var systemName = new Label
				{
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.FillAndExpand,
					VerticalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.TailTruncation,
				};
				systemName.SetBinding(Label.TextProperty, "systemname");

				View = new StackLayout
				{
					Orientation = StackOrientation.Horizontal,
                    Children = { systemName, deleteLabel },
                    VerticalOptions = LayoutOptions.Start,

				};
			}
		}

        const string BOOKSHELF_FILENAME = "bookshelf.txt";
        private ListView libraryListView;
        public SettingPage()
        {
            BindingContext = new CheckTargetLibrarys();

            this.Title = "設定";

            var labelStyleDetailContent = new Style(typeof(Label))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty, Value = Color.Yellow },
                }
            };

            var registLibraryButton = new Button
            {
                Text = "図書館を登録する",
            };
            registLibraryButton.Clicked += (sender, e) =>
            {
                Navigation.PushAsync(new SelectPrefecturePage());
            };



			libraryListView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(LibraryCell)),//セルの指定
				ItemsSource = ((CheckTargetLibrarys)BindingContext).Librarys,
                VerticalOptions = LayoutOptions.Start,
                HeightRequest = (((CheckTargetLibrarys)BindingContext).Librarys.Count()) * (Cell.DefaultCellHeight + 10),
			};
            libraryListView.ItemSelected += (sender, e) =>
            {
                if (e.SelectedItem == null)
                {
                    return;
                }
                ((ListView)sender).SelectedItem = null;
            };

            var exportButton = new Button
            {
                Text = "本だなデータファイルを保存する",
            };
            exportButton.Clicked += async (sender, e) =>
            {
                BookShelf bookShelf = new BookShelf();
                string bookJsonText = JsonConvert.SerializeObject(bookShelf.Books);
                var obj = DependencyService.Get<IBookShelf>();
                string filepath = obj.SaveFile(BOOKSHELF_FILENAME, bookJsonText);
                await DisplayAlert("本だなデータを保存しました", filepath, "OK");
            };

            var importtButton = new Button
            {
                Text = "本だなデータファイルを読み込む",
            };
            importtButton.Clicked += async (sender, e) =>
            {
                bool bLoad = await DisplayAlert("本だなデータの読み込み開始", "アプリに登録済みの本だなデータは全消去しますがよろしいですか？", "OK","キャンセル");
                if (bLoad == false)
                {
                    return;
                }
                var obj = DependencyService.Get<IBookShelf>();
                string filepath = obj.GetFilePath(BOOKSHELF_FILENAME);
                await DisplayAlert("本だなデータファイルを以下の場所に配置したらOKしてください", filepath, "OK");

                bool isExist = obj.IsExistFile(filepath);
                if (isExist == false)
                {
                    await DisplayAlert("読み込み失敗", "本だなデータファイルが見つかりませんでしたので終了します", "OK");
                    return;
                }

                string bookJsonText = obj.LoadFile(filepath);

                IEnumerable<Book> books = JsonConvert.DeserializeObject<IEnumerable<Book>>(bookJsonText);
                BookShelf bookShelf = new BookShelf();
                bookShelf.UpdateAll(books);

                await DisplayAlert("読み込み完了", "本だなデータの読み込みに成功しました", "OK");
            };


			var layout = new StackLayout
			{
				Children = 
                {
                    new Label
                    {
                        Text = "図書館の設定   (最大 5 件まで登録可能)",
                        Style = labelStyleDetailContent,
                    },
                    registLibraryButton,
                    libraryListView,
                    new Label
                    {
                        Text = "本だなデータの移行/取込み",
                        Style = labelStyleDetailContent,
                    },
                    exportButton,
                    importtButton,
                },
			};

            this.Content = layout;
        }

        void OnLabelRegistLibralyClicked()
        {
			Navigation.PushAsync(new SelectPrefecturePage());
		}

        protected override void OnAppearing()
        {
            base.OnAppearing();
            libraryListView.HeightRequest = (((CheckTargetLibrarys)BindingContext).Librarys.Count()) * (Cell.DefaultCellHeight + 10);
        }
    }
}

