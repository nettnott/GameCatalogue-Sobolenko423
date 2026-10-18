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
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            CBRole.SelectedIndex = 0;
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

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBLog.Text) ||
                string.IsNullOrWhiteSpace(TBFIO.Text) ||
                string.IsNullOrEmpty(passBxFrst.Password) ||
                string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (CheckPSW(passBxFrst.Password, passBxScnd.Password))
            {

                var userExists = Core.Context.Users
                    .AsNoTracking()
                    .Any(u => u.Login == TBLog.Text);

                if (userExists)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }

                try
                {
                    Users userObject = new Users
                    {
                        FIO = TBFIO.Text,
                        Login = TBLog.Text,
                        Password = GetHash(passBxFrst.Password),
                        Role = CBRole.Text
                    };

                    Core.Context.Users.Add(userObject);
                    Core.Context.SaveChanges();

                    MessageBox.Show("Пользователь успешно зарегистрирован!");

                    TBLog.Clear();
                    passBxFrst.Clear();
                    passBxScnd.Clear();
                    TBFIO.Clear();
                    CBRole.SelectedIndex = 0;

                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        mainWindow.UpdateMenuVisibility();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении в базу данных: {ex.Message}");
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
    }
}

