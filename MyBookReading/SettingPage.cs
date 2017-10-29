using System;
using System.Collections.Generic;
using System.Linq;
using MyBookReading.Model;
using Realms;
using Xamarin.Forms;

namespace MyBookReading
{
    public class SettingPage : ContentPage
    {
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
					//登録済み図書館
					using (var realm = Realm.GetInstance())
					{
                        realm.Write(() => realm.Remove((CheckTargetLibrary)BindingContext));
					}
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
                Text = "データを取り出す",
            };
            registLibraryButton.Clicked += (sender, e) =>
            {
            };

            var importtButton = new Button
            {
                Text = "データを取り込む",
            };
            registLibraryButton.Clicked += (sender, e) =>
            {
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
                        Text = "データの移行/取込み",
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

