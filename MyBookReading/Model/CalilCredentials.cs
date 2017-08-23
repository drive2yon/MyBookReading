using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;

namespace MyBookReading
{
    public class CalilCredentials
    {
        public string api_key { set; get; }

		public static CalilCredentials LoadCredentialsFile()
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
