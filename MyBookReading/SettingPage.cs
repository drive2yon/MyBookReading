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
				};
				systemName.SetBinding(Label.TextProperty, "systemname");

				View = new StackLayout
				{
					Spacing = 10,
					Orientation = StackOrientation.Horizontal,
                    Children = { systemName, deleteLabel },
                    VerticalOptions = LayoutOptions.Start,

				};
			}
		}

        Label LabelLibrary = new Label();
        private void UpdateLibraryCount()
        {
			//LabelLibrary.Text = string.Format("図書館の設定   登録数：{0} 件  (最大 5 件まで)", ((CheckTargetLibrarysViewModel)BindingContext).Librarys.Count());
			LabelLibrary.Text = string.Format("図書館の設定   (最大 5 件まで登録可能)");
		}

        public SettingPage()
        {
            BindingContext = new CheckTargetLibrarysViewModel();

            this.Title = "設定";

			var labelRegistLibraly = new Label
            {
                Text = "    図書館を登録する",
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            };

			var tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) => OnLabelRegistLibralyClicked();
			labelRegistLibraly.GestureRecognizers.Add(tgr);

			ListView libraryListView = new ListView
			{
				ItemTemplate = new DataTemplate(typeof(LibraryCell)),//セルの指定
				ItemsSource = ((CheckTargetLibrarysViewModel)BindingContext).Librarys,
                VerticalOptions = LayoutOptions.Start,
			};

			var layout = new StackLayout
			{
				Children = 
                { 
                    LabelLibrary,
                    labelRegistLibraly,
                    libraryListView,
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
            UpdateLibraryCount();
		}
    }
}

