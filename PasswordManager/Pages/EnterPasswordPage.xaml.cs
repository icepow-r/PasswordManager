using Microsoft.Data.Sqlite;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;

namespace PasswordManager.Pages
{
    /// <summary>
    /// Interaction logic for EnterPasswordPage.xaml
    /// </summary>
    public partial class EnterPasswordPage : Page
    {
        public EnterPasswordPage()
        {
            InitializeComponent();
        }

        private void ShowPasswordCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (ShowPasswordCheckBox.IsChecked == true)
            {
                PasswordText.Text = PasswordBox.Password;
                PasswordText.Visibility = Visibility.Visible;
                PasswordBox.Visibility = Visibility.Hidden;
            }
            else
            {
                PasswordBox.Visibility = Visibility.Visible;
                PasswordText.Visibility = Visibility.Hidden;
            }
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.SqLiteConnectionString.Password = PasswordBox.Password;

            using (var connection = new SqliteConnection(Manager.GetConnectionString()))
            {
                try
                {
                    connection.Open();
                    using SqliteCommand command = new SqliteCommand("PRAGMA schema_version;", connection);
                    command.ExecuteScalar();
                }
                catch (SqliteException)
                {
                    MessageBox.Show("Неверный пароль");
                    return;
                }
                finally
                {
                    connection.Close();
                }
            }

            Manager.MainFrame.Navigate(new MainPage());
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".db",
                Filter = "Password database (.db)|*.db"
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                // Open document
                Manager.SqLiteConnectionString.DataSource = dialog.FileName;

                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStrings = config.ConnectionStrings;
                connectionStrings.ConnectionStrings["defaultConnection"].ConnectionString = dialog.FileName;
                config.Save(ConfigurationSaveMode.Modified);
            }
        }

        private void PasswordText_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            PasswordBox.Password = PasswordText.Text;
        }

        private void CreateButton_OnClick(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new CreateDbPage());
        }
    }
}
