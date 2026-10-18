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
            bool en = true;
            bool number = false;

            if (password1.Length <= 6)
            {
                for (int i = 0; i < password1.Length; i++)
                {
                    if (password1[i] >= '0' && password1[i] <= '9')
                        number = true;
                    else if (!((password1[i] >= 'A' && password1[i] <= 'Z') || (password1[i] >= 'a' && password1[i] <= 'z')))
                        en = false;
                }

                if (!en)
                {
                    MessageBox.Show("Используйте только английскую расскладку!");
                    return false;
                }
                else if (!number)
                {
                    MessageBox.Show("Добавьте хотя бы одну цифру!");
                    return false;
                }
                else if (en && number)
                {
                    if (password1 != password2)
                    {
                        MessageBox.Show("Пароли не совпадают!");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                return true;
            }
            else
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!");
                return false;
            }

        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TBLog.Text) || string.IsNullOrEmpty(TBFIO.Text) || string.IsNullOrEmpty(passBxFrst.Password) || string.IsNullOrEmpty(passBxScnd.Password))
            {
                MessageBox.Show("Заполните все поля!");
                return;
            }

            if (CheckPSW(passBxFrst.Password, passBxScnd.Password)) 
            {
                var user = Core.Context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == TBLog.Text);

                if (user != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!");
                    return;
                }
                else
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
                    CBRole.SelectedIndex = 1;
                    TBFIO.Clear();
                    return;
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

