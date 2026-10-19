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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GameCatalogue.pages
{
    /// <summary>
    /// Логика взаимодействия для AddGamePage.xaml
    /// </summary>
    public partial class AddGamePage : Page
    {
        private Games _currentGame = new Games();
        public AddGamePage(Games selectedGame)
        {
            InitializeComponent();

            CBCategory.ItemsSource = Lists.categoriesDBL;
            CBCategory.DisplayMemberPath = "Name";
            CBUser.ItemsSource = Lists.usersDBL;
            CBUser.DisplayMemberPath = "FIO";

            if (selectedGame != null)
                _currentGame = selectedGame;
            DataContext = _currentGame;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrWhiteSpace(_currentGame.Name.ToString()))
                errors.AppendLine("Укажите имя");

            if (string.IsNullOrWhiteSpace(_currentGame.Price.ToString()))
                errors.AppendLine("Укажите цену");

            if (string.IsNullOrWhiteSpace(_currentGame.UserID.ToString()))
                errors.AppendLine("Укажите клиента!");
            if
            (string.IsNullOrWhiteSpace(_currentGame.CategoryID.ToString()))
                errors.AppendLine("Укажите категорию!");
            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (_currentGame.ID == 0)
                Core.Context.Games.Add(_currentGame);
            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBGameName.Text = "";
            TBPrice.Text = "";
            TBDescription.Text = "";
            CBCategory.SelectedIndex = 0;
            CBUser.SelectedIndex = 0;
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
