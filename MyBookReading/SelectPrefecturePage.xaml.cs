using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using Org.BouncyCastle.Utilities.Collections;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class SelectPrefecturePage : ContentPage
    {
        public SelectPrefecturePage()
        {
            InitializeComponent();

            string[] prefectureList = new string[47]
            {
                "北海道",
                "青森県",
                "岩手県",
                "宮城県",
                "秋田県",
                "山形県",
                "福島県",
                "茨城県",
                "栃木県",
                "群馬県",
                "埼玉県",
                "千葉県",
                "東京都",
                "神奈川県",
                "新潟県",
                "富山県",
                "石川県",
                "福井県",
                "山梨県",
                "長野県",
                "岐阜県",
                "静岡県",
                "愛知県",
                "三重県",
                "滋賀県",
                "京都府",
                "大阪府",
                "兵庫県",
                "奈良県",
                "和歌山県",
                "鳥取県",
                "島根県",
                "岡山県",
                "広島県",
                "山口県",
                "徳島県",
                "香川県",
                "愛媛県",
                "高知県",
                "福岡県",
                "佐賀県",
                "長崎県",
                "熊本県",
                "大分県",
                "宮崎県",
                "鹿児島県",
                "沖縄県",
            };

			ListView listView = new ListView
			{
                ItemsSource = prefectureList,
			};

			// Define a selected handler for the ListView.
			listView.ItemSelected += (sender, args) =>
			{
				if (args.SelectedItem == null)
				{
					return;
				}
				((ListView)sender).SelectedItem = null;

				GoCityListPageAsync((String)args.SelectedItem);
			};
            this.Content = listView;
        }

        async void GoCityListPageAsync(String prefecture)
        {
            try
            {
                string url = "http://api.calil.jp/citylist?pref=" + System.Net.WebUtility.UrlEncode(prefecture);
                WebRequest request = WebRequest.Create(url);
                WebResponse response = await request.GetResponseAsync();

                Stream st = response.GetResponseStream();
                StreamReader sr = new StreamReader(st);

                string responseFromServer = sr.ReadToEnd();

				await Navigation.PushAsync(new SelectCityPage(prefecture, responseFromServer));

			}
            catch (Exception exception)
            {
				System.Diagnostics.Debug.WriteLine(exception.ToString());
			}
		}
    }
}
