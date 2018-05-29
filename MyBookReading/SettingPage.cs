using System;
using System.Collections.Generic;
using System.Linq;
using MyBookReading.Model;
using MyBookReading.Web;
using Newtonsoft.Json;
using Plugin.GoogleAnalytics;
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
        private int ExportFuncTapCount = 0;
        Label exportLabel;
        Button exportButton;
        Button importButton;
        public SettingPage()
        {
            //GA->
            //設定ページ表示してお問い合わせする人の割合を計測する
            GoogleAnalytics.Current.Tracker.SendView("SettingPage");
            //GA<-

            BindingContext = new CheckTargetLibrarys();

            this.Title = "設定";

            var labelStyleDetailContent = new Style(typeof(Label))
            {
                Setters = {
                    new Setter { Property = BackgroundColorProperty, Value = Color.Yellow },
                }
            };

            //図書館追加とリスト表示
            var libraryCommentLabel = new Label
            {
                Text = "図書館の設定   (最大 5 件まで登録可能)",
                Style = labelStyleDetailContent,
            };

            var tgr = new TapGestureRecognizer();
            tgr.Tapped += (sender, e) => OnLabelClicked(sender, e);
            libraryCommentLabel.GestureRecognizers.Add(tgr);


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


            //お問い合わせボタン
            var requestFormLabel = new Label
            {
                Text = "質問や改善要望の受付(どんなフィードバックも歓迎)",
                Style = labelStyleDetailContent,
            };
            var requestFormButton = new Button
            {
                Text = "お問い合わせフォームへ移動",
            };
            requestFormButton.Clicked += (sender, e) =>
            {
                //GA->
                //設定ページ表示してお問い合わせする人の割合を計測する
                GoogleAnalytics.Current.Tracker.SendEvent("SettingPage", "RequestForm");
                //GA<-

                DependencyService.Get<IWebBrowserService>().Open(new Uri("https://docs.google.com/forms/d/e/1FAIpQLSfeIr1kPfVb87bbIVZX9pgOW_6ks6Wll2zOg-PjjRlAyVmByQ/viewform?usp=sf_link"));
            };

            CrateExportView(labelStyleDetailContent);




			var layout = new StackLayout
			{
				Children = 
                {
                    libraryCommentLabel,    
                    registLibraryButton,
                    libraryListView,

                    requestFormLabel,
                    requestFormButton,

                    exportLabel,
                    exportButton,
                    importButton,
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

        private void OnLabelClicked(object sender, EventArgs e)
        {
            ExportFuncTapCount++;
            if(ExportFuncTapCount == 20)
            {
                exportLabel.IsVisible = true;
                exportButton.IsVisible = true;
                importButton.IsVisible = true;
            }
        }

        private void CrateExportView( Style labelStyleDetailContent)
        {

            exportLabel =  new Label
            {
                Text = "本だなデータの移行/取込み",
                Style = labelStyleDetailContent,
                IsVisible = false
            };

            exportButton = new Button
            {
                Text = "本だなデータファイルを保存する",
                IsVisible = false
            };
            exportButton.Clicked += async (sender, e) =>
            {
                BookShelf bookShelf = new BookShelf();
                string bookJsonText = JsonConvert.SerializeObject(bookShelf.Books);
                var obj = DependencyService.Get<IBookShelf>();
                string filepath = obj.SaveFile(BOOKSHELF_FILENAME, bookJsonText);
                await DisplayAlert("本だなデータを保存しました", filepath, "OK");
            };

            importButton = new Button
            {
                Text = "本だなデータファイルを読み込む",
                IsVisible = false
            };
            importButton.Clicked += async (sender, e) =>
            {
                bool bLoad = await DisplayAlert("本だなデータの読み込み開始", "アプリに登録済みの本だなデータは全消去しますがよろしいですか？", "OK", "キャンセル");
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

        }
    }
}

