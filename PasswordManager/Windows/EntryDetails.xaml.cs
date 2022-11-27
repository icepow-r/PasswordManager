using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PasswordManager.Model;

namespace PasswordManager.Windows
{
    /// <summary>
    /// Interaction logic for EntryDetails.xaml
    /// </summary>
    public partial class EntryDetails : Window
    {
        public Card Card;
        public EntryDetails(Card? card)
        {
            InitializeComponent();

            Card = card ?? new Card();
            DataContext = Card;

            if (card != null)
            {
                HeaderText.Text = "Изменить запись";
                Title = "Изменить запись";
            }
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
