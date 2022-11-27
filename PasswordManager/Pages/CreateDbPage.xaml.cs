using PasswordManager.Context;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PasswordManager.Pages
{
    /// <summary>
    /// Interaction logic for CreateDbPage.xaml
    /// </summary>
    public partial class CreateDbPage : Page
    {
        public CreateDbPage()
        {
            InitializeComponent();
        }

        private void ChooseExistsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                DefaultExt = ".db",
                Filter = "Password database (.db)|*.db"
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                Manager.SqLiteConnectionString.DataSource = dialog.FileName;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var connectionStrings = config.ConnectionStrings;
                connectionStrings.ConnectionStrings["defaultConnection"].ConnectionString = dialog.FileName;
                config.Save(ConfigurationSaveMode.Modified);
            }

            Manager.MainFrame.Navigate(new EnterPasswordPage());
        }

        private void OkButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Менеджер паролей", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            Manager.SqLiteConnectionString.DataSource = "Passwords.db";
            Manager.SqLiteConnectionString.Password = PasswordBox.Password;

            var suffix = 1;
            while (File.Exists(Manager.SqLiteConnectionString.DataSource))
            {
                Manager.SqLiteConnectionString.DataSource = $"Passwords_{suffix}.db";
                suffix++;
            }

            using (var context = new PasswordsContext(Manager.GetConnectionString()))
            {
                context.Database.EnsureCreated();
            }

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var connectionStrings = config.ConnectionStrings;
            connectionStrings.ConnectionStrings["defaultConnection"].ConnectionString = Manager.SqLiteConnectionString.DataSource;
            config.Save(ConfigurationSaveMode.Modified);

            MessageBox.Show("База данных успешно создана", "Менеджер паролей", MessageBoxButton.OK,
                    MessageBoxImage.Information);

            Manager.MainFrame.Navigate(new EnterPasswordPage());
        }

        private void PasswordBox_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            OkButton.IsEnabled = PasswordBox.Password != string.Empty && ConfirmPasswordBox.Password != string.Empty;
        }
    }
}
