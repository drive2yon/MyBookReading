using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MyBookReading
{
    public class SelectLibraryPage : ContentPage
    {
		/// <summary>
		/// https://calil.jp/doc/api_ref.html
		/// </summary>
		public class CalilLibrary
		{
			public string category { get; set; }
			public string city { get; set; }
			public string Short { get; set; }
			public string tel { get; set; }
			public string pref { get; set; }
			public string faid { get; set; }
			public string geocode { get; set; }
			public string systemid { get; set; }
			public string address { get; set; }
			public string libid { get; set; }
			public string libkey { get; set; }
			public string post { get; set; }
			public string url_pc { get; set; }
			public string systemname { get; set; }
			public string isil { get; set; }
			public string formal { get; set; }

            public override string ToString()
            {
                return formal;
            }
		}

        public class LibraryGroup
        {
            public LibraryGroup(string name, List<string>list)
            {
                SystemName = name;
                ShortNameList = list;
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
                //レスポンスから取得した図書館群
				List<CalilLibrary> libraryResponse = JsonConvert.DeserializeObject< List<CalilLibrary>>(jsonResponseLibrary);
                //図書館群をsystem名単位のグループにまとめる
                //< system名, 図書館リスト >
                Dictionary< string, List<CalilLibrary>> systemIdLibraryTable = new Dictionary<string, List<CalilLibrary>>();
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
                foreach (KeyValuePair<string, List<CalilLibrary>> item in systemIdLibraryTable)
				{
                    List<string> shortNameList = new List<string>();
                    foreach( var name in item.Value )
                    {
                        shortNameList.Add(name.Short);
                    }
                    libraryGropuList.Add(new LibraryGroup(item.Key, shortNameList));
				}


                ListView listView = new ListView
                {
                    ItemTemplate = new DataTemplate(typeof(LibraryCell)),//セルの指定
					ItemsSource = libraryGropuList,
                    HasUnevenRows = true,
				};

				// Define a selected handler for the ListView.
				listView.ItemSelected += (sender, args) =>
				{
					//GoLibraryListPageAsync(prefName, (String)args.SelectedItem);
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

