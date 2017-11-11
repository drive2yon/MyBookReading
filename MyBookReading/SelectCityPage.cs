using System;

using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Reflection;

namespace MyBookReading
{
    public class SelectCityPage : ContentPage
    {
        private class CityListSet
		{
			public List<string> あ { get; set; }
			public List<string> か { get; set; }
			public List<string> さ { get; set; }
			public List<string> た { get; set; }
			public List<string> な { get; set; }
			public List<string> は { get; set; }
			public List<string> ま { get; set; }
			public List<string> や { get; set; }
			public List<string> ら { get; set; }
			public List<string> わ { get; set; }
			public List<string> 広域 { get; set; }
		}

        public SelectCityPage( string prefName, string cityJsonResponse )
        {
            CityListSet dataSet;
            const string startSrt = "loadlibs(";
			const string endStr = ");";
            if(cityJsonResponse.StartsWith(startSrt, StringComparison.OrdinalIgnoreCase) && cityJsonResponse.EndsWith(endStr, StringComparison.OrdinalIgnoreCase))
            {
                cityJsonResponse = cityJsonResponse.Substring(startSrt.Length, cityJsonResponse.Length - startSrt.Length - endStr.Length);                
            }
			System.Diagnostics.Debug.WriteLine(cityJsonResponse.ToString());
            try
            {
                dataSet = JsonConvert.DeserializeObject<CityListSet>(cityJsonResponse);
                List<String> cityList = new List<string>();
                if (dataSet.あ != null){ cityList.AddRange(dataSet.あ); }
                if (dataSet.か != null){ cityList.AddRange(dataSet.か); }
                if (dataSet.さ != null){ cityList.AddRange(dataSet.さ); }
                if (dataSet.た != null){ cityList.AddRange(dataSet.た); }
                if (dataSet.な != null){ cityList.AddRange(dataSet.な); }
                if (dataSet.は != null){ cityList.AddRange(dataSet.は); }
                if (dataSet.ま != null){ cityList.AddRange(dataSet.ま); }
                if (dataSet.や != null){ cityList.AddRange(dataSet.や); }
                if (dataSet.ら != null){ cityList.AddRange(dataSet.ら); }
                if (dataSet.わ != null){ cityList.AddRange(dataSet.わ); }
                //if (dataSet.あ != null) { cityList.AddRange(dataSet.広域);}

				ListView listView = new ListView
				{
                    ItemsSource = cityList,
				};

				// Define a selected handler for the ListView.
				listView.ItemSelected += (sender, args) =>
				{
					if (args.SelectedItem == null)
					{
						return;
					}
                    ((ListView)sender).SelectedItem = null;

					GoLibraryListPageAsync(prefName, (String)args.SelectedItem);
				};
				this.Content = listView;

			}
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        async void GoLibraryListPageAsync(String prefecture, String city)
		{
			try
			{
                CalilCredentials calilKey =  LoadCredentialsFile();

                string url = "http://api.calil.jp/library?appkey=" + calilKey.api_key
                                                     + "&pref=" + System.Net.WebUtility.UrlEncode(prefecture)
                                                     + "&city=" + System.Net.WebUtility.UrlEncode(city)
                                                     + "&format=json&callback= ";

				WebRequest request = WebRequest.Create(url);
				WebResponse response = await request.GetResponseAsync();

                Stream st = response.GetResponseStream();
                StreamReader sr = new StreamReader(st);

				string responseFromServer = sr.ReadToEnd();
                System.Diagnostics.Debug.WriteLine(responseFromServer.ToString());

				await Navigation.PushAsync(new SelectLibraryPage(calilKey.api_key, prefecture, city, responseFromServer));

			}
			catch (Exception exception)
			{
				System.Diagnostics.Debug.WriteLine(exception.ToString());
			}
		}

		private CalilCredentials LoadCredentialsFile()
		{
			//CalilCredentials.json sample
			//{"api_key":"XXXXX"}

			var assembly = typeof(SelectCityPage).GetTypeInfo().Assembly;
            Stream stream = assembly.GetManifestResourceStream("MyBookReading.Assets.CalilCredentials.json");
			string text = "";
			using (var reader = new System.IO.StreamReader(stream))
			{
				text = reader.ReadToEnd();
			}
			CalilCredentials key = JsonConvert.DeserializeObject<CalilCredentials>(text);
			return key;
		}

	}
}

