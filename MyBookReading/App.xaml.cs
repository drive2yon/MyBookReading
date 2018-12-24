using Plugin.GoogleAnalytics;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

			if (Application.Current.Resources == null)
			{
				Application.Current.Resources = new ResourceDictionary();
			}

			var g_footerLabelStyle = new Style(typeof(Label))
            {
            	Setters = {
                    
            		new Setter { Property = Label.TextColorProperty, Value = Color.Silver },
            		new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Navy },
                    new Setter { Property = Label.LineBreakModeProperty, Value = LineBreakMode.MiddleTruncation }, 
            	}
            };

            GoogleAnalytics.Current.Config.TrackingId = "UA-119889066-1";
            GoogleAnalytics.Current.Config.AppId = "com.rydeenworks.mybookreading";
            GoogleAnalytics.Current.Config.AppName = "図書館ほんだな";
            GoogleAnalytics.Current.Config.AppVersion = "1.3";
            GoogleAnalytics.Current.Config.ReportUncaughtExceptions = true;
            GoogleAnalytics.Current.InitTracker();


			Application.Current.Resources.Add("FooterLabelStyle", g_footerLabelStyle);

			MainPage = new NavigationPage(new MyBookListPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
