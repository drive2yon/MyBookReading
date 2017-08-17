using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using MyBookReading.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using PCLStorage;
using System.Threading.Tasks;
using Realms;
using System.Linq;

namespace MyBookReading
{
    public class SelectLibraryPage : ContentPage
    {
		//図書館群をsystem名単位のグループにまとめる
		//< system名, 図書館リスト >
		Dictionary<string, List<CalilLibrary>> systemIdLibraryTable;

        public class LibraryGroup
        {
            public LibraryGroup(string name, List<string>list, bool isRegist)
            {
                SystemName = name;
                ShortNameList = list;
                IsRegist = isRegist;
                var str = new StringBuilder();
                foreach(string libName in ShortNameList)
                {
                    str.Append(libName)
                       .Append("/");
                }
                ShortNameLabel = str.ToString();
            }
            public string SystemName {private set; get; }
            public List<string> ShortNameList { private set; get; }
            public string ShortNameLabel { private set; get; }
            public bool IsRegist{ set; get; }
        };

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

            	//systemname
                var systemName = new Label { FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                                             FontAttributes = FontAttributes.Bold,
                                             HorizontalOptions = LayoutOptions.FillAndExpand};
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

        public SelectLibraryPage( string apiKey, string pref, string city, string jsonResponseLibrary )
        {
			try
			{
				List<CalilLibrary> libraryResponse = JsonConvert.DeserializeObject< List<CalilLibrary>>(jsonResponseLibrary);
                systemIdLibraryTable = new Dictionary<string, List<CalilLibrary>>();
				foreach (CalilLibrary library in libraryResponse)
				{
                    string key = library.systemname;
                    if(systemIdLibraryTable.ContainsKey(key))
                    {
                        systemIdLibraryTable[key].Add(library);
                    }
                    else
                    {
                        var value = new List<CalilLibrary>();
                        value.Add(library);
                        systemIdLibraryTable.Add(key, value);
					}
				}

				List<LibraryGroup> libraryGropuList = new List<LibraryGroup>();

				//登録済み図書館
				using (var realm = Realm.GetInstance())
				{
                    foreach (KeyValuePair<string, List<CalilLibrary>> item in systemIdLibraryTable)
        			{
                        var librarys = realm.All<CalilLibrary>().Where(x => x.systemname == item.Key);
                        bool isRegist = librarys.Count() > 0;

						List<string> shortNameList = new List<string>();
                        foreach( var name in item.Value )
                        {
                            shortNameList.Add(name.Short);
                        }
                        libraryGropuList.Add(new LibraryGroup(item.Key, shortNameList, isRegist));
        			}
				}

				ListView listView = new ListView
                {
                    ItemTemplate = new DataTemplate(typeof(LibraryCell)),//セルの指定
					ItemsSource = libraryGropuList,
                    HasUnevenRows = true,
				};

				// Define a selected handler for the ListView.
				listView.ItemSelected += async (sender, args) =>
				{
                    var item = args.SelectedItem as LibraryGroup;
					bool ret = await DisplayAlert("図書館の登録", "検索対象に登録しますか？", "OK","キャンセル");
                    if(ret)
                    {
                        item.IsRegist = true;
                        //図書館をDBに登録する
                        foreach (var keyValuePair in systemIdLibraryTable)
                        {
                            if(keyValuePair.Key == item.SystemName)
                            {
                                using (var realm = Realm.GetInstance())
                                {
                                    realm.Write(() =>
                                    {
                                        foreach(var library in keyValuePair.Value)
                                        {
											realm.Add(library);
										}
                                    });
                                }

                                break;
                            }
                        }
                    }
                    ((ListView)sender).SelectedItem = null;
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

