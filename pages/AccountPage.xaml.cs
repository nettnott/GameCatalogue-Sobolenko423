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
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
            // Присваивание элементам TextBlock данных текущего пользователя через глобальную переменную currentUser
            TBFIO.Text = Lists.currentUser.FIO;
            TBLogin.Text = Lists.currentUser.Login;
            TBRole.Text = Lists.currentUser.Role;
        }
        /// <summary>
        /// Переход на страницу смены пароля.
        /// </summary>
        private void BTNChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new ChangePassPage());
        }
    }
}
