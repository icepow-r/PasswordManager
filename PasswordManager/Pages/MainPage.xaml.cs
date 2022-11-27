using PasswordManager.Context;
using PasswordManager.Model;
using PasswordManager.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace PasswordManager.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private List<Card> _cards;
        public MainPage()
        {
            InitializeComponent();

            using (var context = new PasswordsContext(Manager.GetConnectionString()))
            {
                _cards = context.Cards.ToList();
            }
        }

        private void ChangePassword_OnClick(object sender, RoutedEventArgs e)
        {
            var dialogWindow = new ChangePasswordDialog();
            dialogWindow.ShowDialog();
        }

        private void OpenDatabase_OnClick(object sender, RoutedEventArgs e)
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

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void AddEntry_OnClick(object sender, RoutedEventArgs e)
        {
            using (var context = new PasswordsContext(Manager.GetConnectionString()))
            {
                var window = new EntryDetails(null);

                var result = window.ShowDialog();

                if (result == true)
                {
                    context.Cards.Add(window.Card);
                    context.SaveChanges();
                    UpdateListView(context);
                }
            }
        }

        private void EditEntry_OnClick(object sender, RoutedEventArgs e)
        {
            var card = MainListView.SelectedItem as Card;

            using (var context = new PasswordsContext(Manager.GetConnectionString()))
            {
                var window = new EntryDetails(card);
                if (window.ShowDialog() == true)
                {
                    context.Cards.Update(card!);
                    context.SaveChanges();
                    UpdateListView(context);
                }
            }
        }

        private void DeleteEntry_OnClick(object sender, RoutedEventArgs e)
        {
            var card = MainListView.SelectedItem as Card;
            var result = MessageBox.Show($"Вы действительно хотите удалить учётную запись {card!.Title}?", "Менеджер паролей", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

            if (result == MessageBoxResult.Yes)
            {
                using (var context = new PasswordsContext(Manager.GetConnectionString()))
                {
                    context.Cards.Remove(card);
                    context.SaveChanges();
                    UpdateListView(context);
                }
            }
        }

        private void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            MainListView.ItemsSource = _cards;
        }

        private void MainListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainListView.SelectedItem != null)
            {
                EditEntry.IsEnabled = true;
                DeleteEntry.IsEnabled = true;
            }
            else
            {
                EditEntry.IsEnabled = false;
                DeleteEntry.IsEnabled = false;
            }
        }

        private void MainListView_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            EditEntry_OnClick(sender, e);
        }

        private void CopyPassword_OnClick(object sender, RoutedEventArgs e)
        {
            var password = (MainListView.SelectedItem as Card)?.Password;

            if (password != null)
            {
                Clipboard.SetText(password);
            }
        }

        private void UpdateListView(PasswordsContext context)
        {
            _cards = context.Cards.ToList();
            MainListView.ItemsSource = _cards;
        }

        private void FilterTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var collectionView = CollectionViewSource.GetDefaultView(MainListView.ItemsSource);
            collectionView.Filter = Filter;
            collectionView.Refresh();
        }

        private bool Filter(object arg)
        {
            var card = arg as Card;
            return card!.Title.ToLower().Contains(FilterTextBox.Text.ToLower());
        }
    }
}
