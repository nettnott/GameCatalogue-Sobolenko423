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
                var user = Lists.usersDBL
                                        .FirstOrDefault(u => u.Login == TBLogin.Text && u.Password == hashedPass);

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
        public static string GetHash(String password)
        {
            using (var hash = SHA1.Create())
            {
                return
                string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x =>
                x.ToString("X2")));
            }
        }

        private bool CheckPSW(String password1, String password2)
        {
            // 1. Проверяем длину (минимум 6 символов)
            if (password1.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return false;
            }

            bool hasDigit = false;
            bool onlyEnglish = true;

            // 2. Проверяем состав пароля за один цикл
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

            // 3. Проверяем совпадение паролей
            if (password1 != password2)
            {
                MessageBox.Show("Пароли не совпадают!");
                return false;
            }

            return true;
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void BTNChangePassword_Click(object sender, RoutedEventArgs e)
        {
            CheckRightness();
        }
    }
}
