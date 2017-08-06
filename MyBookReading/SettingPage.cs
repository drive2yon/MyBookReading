using System;

using Xamarin.Forms;

namespace MyBookReading
{
    public class SettingPage : ContentPage
    {
        public SettingPage()
        {
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
                    new TableSection("図書館の設定   登録数：1 件  (最大 5 件まで)")
                    {
                        new ViewCell
                        {
                            View = labelRegistLibraly
                        },
                    }
				}
			};



			this.Content = tableView;
        }

        void OnLabelRegistLibralyClicked()
        {
			Navigation.PushAsync(new SelectPrefecturePage());
		}
    }
}

