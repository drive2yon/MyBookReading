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
		private class LibraryCell : ViewCell
		{
			public LibraryCell()
			{

				Switch librarySelSwitch = new Switch
				{
					HorizontalOptions = LayoutOptions.End,
					VerticalOptions = LayoutOptions.CenterAndExpand,
				};

				//systemname
				var systemName = new Label
				{
					FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
					FontAttributes = FontAttributes.Bold,
					HorizontalOptions = LayoutOptions.FillAndExpand
				};
				systemName.SetBinding(Label.TextProperty, "SystemName");

				//shortname list
				var shortName = new Label { FontSize = Device.GetNamedSize(NamedSize.Default, typeof(Label)) };
				shortName.SetBinding(Label.TextProperty, "ShortNameLabel");

				//サブレイアウト
				var layoutSub = new StackLayout
				{
					Spacing = 10,
					Orientation = StackOrientation.Horizontal,
					Children = { systemName, librarySelSwitch }
				};

				View = new StackLayout
				{
					Padding = new Thickness(5),
					Children = { layoutSub, shortName }
				};
			}
		}

        public SettingPage()
        {
            int registLibraryCount = 0;
			//登録済み図書館
			using (var realm = Realm.GetInstance())
			{
				var librarys = realm.All<CalilLibrary>();//.Where(x => x.systemid != null );
                registLibraryCount = librarys.Count();
			}

			var labelRegistLibraly = new Label
            {
                Text = "    図書館を登録する",
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
            };

			var tgr = new TapGestureRecognizer();
			tgr.Tapped += (sender, e) => OnLabelRegistLibralyClicked();
			labelRegistLibraly.GestureRecognizers.Add(tgr);


			TableView tableView = new TableView
            {
                Intent = TableIntent.Form,
                Root = new TableRoot("TableView Title")
                {
                    new TableSection(string.Format("図書館の設定   登録数：{0} 件  (最大 5 件まで)", registLibraryCount))
                    {
                        new ViewCell
                        {
                            View = labelRegistLibraly
                        },
                    }
				}
			};


			//ListView libraryListView = new ListView
			//{
			//	ItemTemplate = new DataTemplate(typeof(LibraryCell)),//セルの指定
			//	ItemsSource = librarys,
			//	HasUnevenRows = true,
			//};



			//var layoutSub = new StackLayout
			//{
			//	Spacing = 10,
			//	Children = { tableView, librarySelSwitch }
			//};




			this.Content = tableView;
        }

        void OnLabelRegistLibralyClicked()
        {
			Navigation.PushAsync(new SelectPrefecturePage());
		}
    }
}

