using System;
using System.Collections.Generic;
using System.Text;
using MyBookReading.Model;
using Newtonsoft.Json;
using Xamarin.Forms;
using Realms;
using System.Linq;
using System.ComponentModel;

namespace MyBookReading
{
    public class SelectLibraryPage : ContentPage
    {
		//図書館群をsystem名単位のグループにまとめる
		//< system名, 図書館リスト >
		Dictionary<string, List<CalilLibrary>> systemIdLibraryTable;

        public class LibraryGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            public LibraryGroup(string name, List<string>list, bool isRegist)
            {
                SystemName = name;
                ShortNameList = list;
                IsRegist = isRegist;
                CanRegist = !IsRegist;
                GroupTitle = SystemName;
                if(IsRegist)
                {
                    GroupTitle += "（登録済）";
                }
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
            public bool IsRegist{ private set; get; }
			public bool CanRegist { private set; get; }
			public string GroupTitle { private set; get; }
            public void UpdateStatus(bool isRegist)
            {
                IsRegist = isRegist;
				CanRegist = !IsRegist;
				if (IsRegist)
				{
					GroupTitle = SystemName + "（登録済）";
				}
                else
                {
					GroupTitle = SystemName;
				}

                PropertyChanged(this, new PropertyChangedEventArgs("IsRegist"));
				PropertyChanged(this, new PropertyChangedEventArgs("CanRegist"));
				PropertyChanged(this, new PropertyChangedEventArgs("GroupTitle"));
			}
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

        /// <summary>
        /// 図書館検索のレスポンスJSONを図書館クラスのリストにする。
        /// そのリストをsystemid単位で分けたディクショナリーにして返す
        /// </summary>
        /// <returns>The systemIDLibrary table.</returns>
        /// <param name="jsonResponseLibrary">Json response library.</param>
        private Dictionary<string, List<CalilLibrary>> getSystemIDLibraryTable(string jsonResponseLibrary)
        {
			Dictionary<string, List<CalilLibrary>> table = new Dictionary<string, List<CalilLibrary>>();
			try
            {
                List<CalilLibrary> libraryResponse = JsonConvert.DeserializeObject<List<CalilLibrary>>(jsonResponseLibrary);
                foreach (CalilLibrary library in libraryResponse)
                {
                    string key = library.systemname;
                    if (table.ContainsKey(key))
                    {
                        table[key].Add(library);
                    }
                    else
                    {
                        var value = new List<CalilLibrary>();
                        value.Add(library);
                        table.Add(key, value);
                    }
                }
            }
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());
			}
			return table;
		}

        /// <summary>
        /// ViewCellに表示するための図書館情報リストを取得する
        /// </summary>
        /// <returns>The library group list.</returns>
        private List<LibraryGroup> getLibraryGroupList()
        {
			List<LibraryGroup> libraryGropuList = new List<LibraryGroup>();

			//登録済み図書館
			using (var realm = Realm.GetInstance())
			{
				foreach (KeyValuePair<string, List<CalilLibrary>> item in systemIdLibraryTable)
				{
					var librarys = realm.All<CheckTargetLibrary>().Where(x => x.systemname == item.Key);
					bool isRegist = librarys.Count() > 0;

					List<string> shortNameList = new List<string>();
					foreach (var name in item.Value)
					{
						shortNameList.Add(name.Short);
					}
					libraryGropuList.Add(new LibraryGroup(item.Key, shortNameList, isRegist));
				}
			}
            return libraryGropuList;
		}

        public SelectLibraryPage( string apiKey, string pref, string city, string jsonResponseLibrary )
        {
			try
			{
                systemIdLibraryTable = getSystemIDLibraryTable(jsonResponseLibrary);
                List<LibraryGroup> libraryGropuList = getLibraryGroupList();


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

                    if(item.IsRegist)
                    {
						item.UpdateStatus(false);

						//DBから図書館を削除
						using (var realm = Realm.GetInstance())
						{
							realm.Write(() =>
							{
                                var librarys = realm.All<CheckTargetLibrary>().Where(x => x.systemname == item.SystemName);;
                                if(librarys != null && librarys.Count()>0)
                                {
                                    var library = librarys.First();
									realm.Remove(library);
								}
							});
						}
					}
                    else
                    {
						using (var realm = Realm.GetInstance())
						{
                            var librarys = realm.All<CheckTargetLibrary>();
                            if(librarys != null)
                            {
                                const int MAX_LIBRARY_NUM = 5;
                                if(librarys.Count() >= MAX_LIBRARY_NUM)
                                {
									await DisplayAlert("図書館の登録", "6件以上は登録できません", "OK");
									return;
                                }
                            }
                            
							//bool ret = await DisplayAlert("図書館の登録", "検索対象に登録しますか？", "OK", "キャンセル");
							//if (!ret)
							//{
							//    return;
							//}
							item.UpdateStatus(true);

    						//図書館をDBに登録する
    						foreach (var keyValuePair in systemIdLibraryTable)
    						{
    							if (keyValuePair.Key == item.SystemName)
    							{
    									realm.Write(() =>
    									{
    										var library = keyValuePair.Value.First();
    										CheckTargetLibrary target = new CheckTargetLibrary()
    										{
    											systemid = library.systemid,
    											systemname = library.systemname,
    										};
    										realm.Add(target);
    									});

    								break;
    							}
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

