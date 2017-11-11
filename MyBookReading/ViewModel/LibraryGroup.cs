using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using MyBookReading.Model;
using Newtonsoft.Json;
using System.Linq;

namespace MyBookReading.ViewModel
{
    //View表示用の図書館情報
    public class LibraryGroup : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public LibraryGroup(string name, List<string> list, bool isRegist)
        {
            SystemName = name;
            ShortNameList = list;
            IsRegist = isRegist;
            CanRegist = !IsRegist;
            GroupTitle = SystemName;
            if (IsRegist)
            {
                GroupTitle += "（登録済）";
            }
            var str = new StringBuilder();
            foreach (string libName in ShortNameList)
            {
                str.Append(libName)
                   .Append("/");
            }
            ShortNameLabel = str.ToString();
        }
        public string SystemName { private set; get; }
        public List<string> ShortNameList { private set; get; }
        public string ShortNameLabel { private set; get; }
        public bool IsRegist { private set; get; }
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

        /// <summary>
        /// 図書館検索のレスポンスJSONを図書館クラスのリストにする。
        /// そのリストをsystemid単位で分けたディクショナリーにして返す
        /// </summary>
        /// <returns>The systemIDLibrary table.</returns>
        /// <param name="jsonResponseLibrary">Json response library.</param>
        public static Dictionary<string, List<CalilLibrary>> getSystemIDLibraryTable(string jsonResponseLibrary)
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
        /// <param name="systemIdLibraryTable">図書館群をsystem名単位のグループ< system名, 図書館リスト ></param>
        public static List<LibraryGroup> GetLibraryGroupList(Dictionary<string, List<CalilLibrary>> systemIdLibraryTable)
        {
            List<LibraryGroup> libraryGropuList = new List<LibraryGroup>();
            CheckTargetLibrarys targetLibrary = new CheckTargetLibrarys();

            //登録済み図書館
            foreach (KeyValuePair<string, List<CalilLibrary>> item in systemIdLibraryTable)
            {
                bool isRegist = targetLibrary.IsRegist(item.Key);

                List<string> shortNameList = new List<string>();
                foreach (var name in item.Value)
                {
                    shortNameList.Add(name.Short);
                }
                libraryGropuList.Add(new LibraryGroup(item.Key, shortNameList, isRegist));
            }

            return libraryGropuList;
        }

    };
}
