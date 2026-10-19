using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.RightsManagement;
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
    /// Логика взаимодействия для AddUserPage.xaml
    /// </summary>
    public partial class AddUserPage : Page
    {
        /// <summary>
        /// Создается локальная переменная текущего пользователя
        /// Если пользователь зашел в режиме редактирования (передан пользователь), то текущему пользователю присваивается выбранный
        /// </summary>
        private Users _currentUser = new Users();
        public AddUserPage(Users selectedUser)
        {
            InitializeComponent();
            if (selectedUser != null)
                _currentUser = selectedUser;
            DataContext = _currentUser;
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку сохранения.
        /// Проверяет заполненность полей, если есть как минимум одн пустое - выводит сообщение с ошибкой
        /// Данные из полей ввода и выпадающих списков переносятся в поля текущего пользователя, пароль хэшируется
        /// Если пользователь новый (ID = 0), то добавляется в БД
        /// Через try-catch осуществляется попытка сохранения БД. Если все ок - выводится сообщение и пользователя перекидывает назад, если не - выводится ошибка
        /// </summary>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBLogin.Text) ||
                string.IsNullOrWhiteSpace(TBPass.Text) || 
                cmbRole.SelectedValue == null ||
                string.IsNullOrWhiteSpace(TBFio.Text))
            {
                MessageBox.Show("Не все данные заполнены!");
                return;
            }

            _currentUser.Login = TBLogin.Text;
            _currentUser.Password = GetHash(TBPass.Text);
            _currentUser.Role = cmbRole.Text;
            _currentUser.FIO = TBFio.Text;

            if (_currentUser.ID == 0)
                Core.Context.Users.Add(_currentUser);
            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
        /// <summary>
        /// Хэширование пароля
        /// </summary>
        public static string GetHash(String password)
        {
            using (var hash = SHA1.Create())
            {
                return
                string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x =>
                x.ToString("X2")));
            }
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку очистки.
        /// Очищает все поля.
        /// </summary>
        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBLogin.Text = "";
            TBPass.Text = "";
            cmbRole.SelectedIndex = 0;
            TBFio.Text = "";

        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку назад.
        /// Перекидывает пользователя на предыдущую страницу.
        /// </summary>
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
