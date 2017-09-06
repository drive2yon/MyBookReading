using MyBookReading.Model;
using Xamarin.Forms;

namespace MyBookReading
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new NavigationPage( new MyBookListPage() );
            BookViewModel vm = new BookViewModel();

            MainPage = new NavigationPage(new MyBookListPage( vm.Books, false ));
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
