using System;

using Xamarin.Forms;
using Newtonsoft.Json;

namespace MyBookReading
{
    public class SelectCityPage : ContentPage
    {
        public SelectCityPage( string cityJsonResponse )
        {
            //System.Data.DataSet dataSet = JsonConvert.DeserializeObject<System.Data.DataSet>(cityJsonResponse);

			Content = new StackLayout
            {
                Children = {
                    new Label { Text = cityJsonResponse }
                }
            };
        }
    }
}

