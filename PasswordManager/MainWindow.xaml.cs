using Microsoft.Data.Sqlite;
using PasswordManager.Context;
using PasswordManager.Pages;
using System.Configuration;
using System.Windows;

namespace PasswordManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame = MainFrame;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStrings = config.ConnectionStrings;
            var connectionString = connectionStrings.ConnectionStrings["defaultConnection"].ConnectionString;

            Manager.SqLiteConnectionString = new SqliteConnectionStringBuilder
            {
                Mode = SqliteOpenMode.ReadWriteCreate,
                DataSource = connectionString
            };


            using (var context = new PasswordsContext(Manager.GetConnectionString()))
            {
                if (context.Database.CanConnect())
                {
                    MainFrame.Navigate(new EnterPasswordPage());
                }
                else
                {
                    MainFrame.Navigate(new CreateDbPage());
                }
            }
        }
    }
}
