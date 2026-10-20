using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
    /// Логика взаимодействия для ChangePassPage.xaml
    /// </summary>
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Проверка правильности пароля.
        /// Проверяет, все ли поля заполнены, если да то проверяет есть ли такой юзер в бд и такой ли у него старый пароль.
        /// Если все ок, то вызывает функцию проверки сложности пароля.
        /// Если все ок, то хэширует пароль и сохраняет в бд
        /// </summary>
        private void CheckRightness()
        {
            if (string.IsNullOrEmpty(PasswordBox.Password) ||
                string.IsNullOrEmpty(PasswordBoxBRANDNEW.Password) ||
                string.IsNullOrEmpty(PasswordBoxNEWCheck.Password) ||
                string.IsNullOrEmpty(TBLogin.Text))
            {
                MessageBox.Show("Все поля обязательны к заполнению!");
                return;
            }
            else
            {
                string hashedPass = GetHash(PasswordBox.Password);
                var user = Lists.usersDBL.FirstOrDefault(u => u.Login == TBLogin.Text && u.Password == hashedPass);

                if (user == null)
                {
                    MessageBox.Show("Текущий пароль/Логин неверный!");
                    return;
                }
                else
                {
                    if (CheckPSW(PasswordBoxBRANDNEW.Password, PasswordBoxNEWCheck.Password))
                    {
                        user.Password = GetHash(PasswordBoxBRANDNEW.Password);
                        Core.Context.SaveChanges();
                        MessageBox.Show("Пароль успешно изменен!");
                        NavigationService?.Navigate(new LogInPage());
                    }
                }
            }
        }
        /// <summary>
        /// Функция для хэширования пароля
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
        /// Проверяет соответствие пароля требованиям безопасности
        /// </summary>
        private bool CheckPSW(String password1, String password2)
        {
            // Проверяет длину (минимум 6 символов)
            if (password1.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return false;
            }

            bool hasDigit = false;
            bool onlyEnglish = true;

            // Проверяет состав пароля за один цикл
            foreach (char c in password1)
            {
                if (char.IsDigit(c))
                {
                    hasDigit = true;
                }
                else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                {
                    onlyEnglish = false;
                }
            }

            if (!onlyEnglish)
            {
                MessageBox.Show("Используйте только английскую раскладку!");
                return false;
            }

            if (!hasDigit)
            {
                MessageBox.Show("Добавьте хотя бы одну цифру!");
                return false;
            }

            // Проверяет совпадение паролей
            if (password1 != password2)
            {
                MessageBox.Show("Пароли не совпадают!");
                return false;
            }

            return true;
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку назад.
        /// Перекидывает пользователя на предыдущую страницу.
        /// </summary>
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку подтверждения смены пароля.
        /// Вызывает проверку правильности пароля.
        /// </summary>
        private void BTNChangePassword_Click(object sender, RoutedEventArgs e)
        {
            CheckRightness();
        }
    }
}
