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
