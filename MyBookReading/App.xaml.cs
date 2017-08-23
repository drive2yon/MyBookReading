using MyBookReading.Model;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var bookListPage = new MyBookListPage()
            {
                CheckTargetLibraryVM = new CheckTargetLibrarysViewModel(),
            };

            MainPage = new NavigationPage( new MyBookListPage() );
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
