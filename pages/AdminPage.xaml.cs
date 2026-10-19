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
    /// Логика взаимодействия для AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        /// <summary>
        /// Обрабатывает нажатие на кнопки и перенаправляет пользователя на соответствующие страницы
        /// </summary>
        public AdminPage()
        {
            InitializeComponent();
        }

        private void BTNTabUsers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new UsersTabPage());
        }

        private void BTNTabCategory_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new CategoryTabPage());
        }

        private void BTNTabGames_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new GamesTabPage());
        }

        private void BTNDiagrams_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new DiagramsPage());
        }
    }
}
