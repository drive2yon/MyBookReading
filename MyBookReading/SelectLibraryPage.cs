using System;
using System.Collections.Generic;
using MyBookReading.Model;
using Xamarin.Forms;
using System.Linq;
using MyBookReading.ViewModel;
using Plugin.GoogleAnalytics;

namespace MyBookReading
{
    public class SelectLibraryPage : ContentPage
    {
		//図書館群をsystem名単位のグループにまとめる< system名, 図書館リスト >
		Dictionary<string, List<CalilLibrary>> systemIdLibraryTable;


        private class LibraryCell : ViewCell
		{
			public LibraryCell()
			{

	            Switch librarySelSwitch = new Switch
	            {
                    HorizontalOptions = LayoutOptions.End,
                    VerticalOptions = LayoutOptions.CenterAndExpand,
	            };
                librarySelSwitch.SetBinding(Switch.IsToggledProperty, "IsRegist");
                librarySelSwitch.IsEnabled = false;

            	//systemname
                var systemName = new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                                             FontAttributes = FontAttributes.Bold,
                                             HorizontalOptions = LayoutOptions.FillAndExpand};
				systemName.SetBinding(Label.TextProperty, "GroupTitle");

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

                //this.SetBinding(IsEnabledProperty, "CanRegist" );
			}
		}

        public SelectLibraryPage( string apiKey, string pref, string city, string jsonResponseLibrary )
        {
			try
			{
                systemIdLibraryTable = LibraryGroup.getSystemIDLibraryTable(jsonResponseLibrary);
                List<LibraryGroup> libraryGropuList = LibraryGroup.GetLibraryGroupList(systemIdLibraryTable);


				ListView listView = new ListView
                {
                    ItemTemplate = new DataTemplate(typeof(LibraryCell)),//セルの指定
					ItemsSource = libraryGropuList,
                    HasUnevenRows = true,
				};

				// Define a selected handler for the ListView.
				listView.ItemSelected += async (sender, args) =>
				{
                    if(args.SelectedItem == null)
                    {
                        return;
                    }
					((ListView)sender).SelectedItem = null;

					var item = (LibraryGroup)args.SelectedItem;

                    CheckTargetLibrarys targetLibrarys = new CheckTargetLibrarys();

                    if(item.IsRegist)
                    {
						item.UpdateStatus(false);

						//DBから図書館を削除
                        targetLibrarys.DelLibrary(item.SystemName);
					}
                    else
                    {
                        const int MAX_LIBRARY_NUM = 5;
                        if (targetLibrarys.Librarys.Count() >= MAX_LIBRARY_NUM)
                        {
                            await DisplayAlert("図書館の登録", "6件以上は登録できません", "OK");
                            return;
                        }
                        //GA->
                        //図書館を仮想的に利用して膨大な蔵書を育てる人が多い事の検証。図書館追加することを検証する
                        int libraryNum = targetLibrarys.Librarys.Count() + 1;
                        GoogleAnalytics.Current.Tracker.SendEvent("SelectLibraryPage", "AddLibrary", libraryNum.ToString());
                        //GA<-

                        item.UpdateStatus(true);
                        //図書館をDBに登録する
                        foreach (var keyValuePair in systemIdLibraryTable)
                        {
                            if (keyValuePair.Key == item.SystemName)
                            {
                                var library = keyValuePair.Value.First();

                                targetLibrarys.AddLibrary(library.systemid, library.systemname);
                            }
                        }
					}

				};
				this.Content = listView;

			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
        }
    }
}

