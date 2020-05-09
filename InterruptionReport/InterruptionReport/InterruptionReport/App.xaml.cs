using InterruptionReport.DBHelper;
using InterruptionReport.View;
using System;
using System.Globalization;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace InterruptionReport
{
    public partial class App : Application
    {
        static LocalDatabase database;
        public static LocalDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new LocalDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LocalDatabase.sqlite"));
                }
                return database;
            }
        }
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new AddInterruptionPage());
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
