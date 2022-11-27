using System.IO;
using Microsoft.Data.Sqlite;
using System.Windows;

namespace PasswordManager.Windows
{
    /// <summary>
    /// Interaction logic for ChangePasswordDialog.xaml
    /// </summary>
    public partial class ChangePasswordDialog : Window
    {
        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (OldPassword.Password != Manager.SqLiteConnectionString.Password)
            {
                MessageBox.Show("Старый пароль введён неверно", "Менеджер паролей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (NewPassword.Password != RepeatPassword.Password)
            {
                MessageBox.Show("Пароли не совпадают", "Менеджер паролей", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var connection = new SqliteConnection(Manager.GetConnectionString()))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT quote($newPassword);";
                command.Parameters.AddWithValue("$newPassword", NewPassword.Password);
                var quotedNewPassword = command.ExecuteScalar() as string;

                command.CommandText = "PRAGMA rekey = " + quotedNewPassword;
                command.Parameters.Clear();
                command.ExecuteNonQuery();
            }

            Manager.SqLiteConnectionString.Password = NewPassword.Password;
            MessageBox.Show("Пароль успешно изменён", "Менеджер паролей", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void NewPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            SaveButton.IsEnabled = NewPassword.Password != string.Empty;
        }
    }
}
