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
    /// Логика взаимодействия для LogInPage.xaml
    /// </summary>
    public partial class LogInPage : Page
    {
        private int failedAttempts = 0;

        public LogInPage()
        {
            InitializeComponent();
        }

        private void BTNEnter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TBLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин или пароль");
                return;
            }

            string hashedPassword = GetHash(PasswordBox.Password);

            var db = Core.Context;
            var user = db.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Login == TBLogin.Text &&
                u.Password == hashedPassword);

            if (user == null)
            {
                MessageBox.Show("Пользователь с такими данными не найден!");
                failedAttempts++;
                if (failedAttempts >= 3)
                {
                    if (captcha.Visibility != Visibility.Visible)
                    {
                        CaptchaSwitch();
                    }
                    CaptchaChange();
                }
                return;
            }
            else if (Lists.currentUser != null)
            {
                MessageBox.Show("Выйдите из аккаунта, чтобы зайти в новый");
                return;
            }
            else
            {
                Lists.currentUser = user;
                MessageBox.Show("Пользователь успешно найден!");

                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.UpdateMenuVisibility();
                }

                switch (user.Role)
                {
                    case "USER":
                        NavigationService?.Navigate(new AccountPage());
                        break;
                    case "ADMIN":
                        NavigationService?.Navigate(new AdminPage());
                        break;

                }
            }
        }

        private void BTNReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void BTNChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
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

        public void CaptchaSwitch()
        {
            switch (captcha.Visibility)
            {
                case Visibility.Visible:
                    TBLogin.Clear();
                    PasswordBox.Clear();

                    captcha.Visibility = Visibility.Collapsed;
                    captchaInput.Visibility = Visibility.Collapsed;
                    labelCaptcha.Visibility = Visibility.Collapsed;
                    submitCaptcha.Visibility = Visibility.Collapsed;

                    labelLogin.Visibility = Visibility.Visible;
                    labelPass.Visibility = Visibility.Visible;
                    TBLogin.Visibility = Visibility.Visible;
                    PasswordBox.Visibility = Visibility.Visible;

                    BTNChangePassword.Visibility = Visibility.Visible;
                    BTNEnter.Visibility = Visibility.Visible;
                    BTNReg.Visibility = Visibility.Visible;
                    return;
                case Visibility.Collapsed:
                    captcha.Visibility = Visibility.Visible;
                    captchaInput.Visibility = Visibility.Visible;
                    labelCaptcha.Visibility = Visibility.Visible;
                    submitCaptcha.Visibility = Visibility.Visible;

                    labelLogin.Visibility = Visibility.Collapsed;
                    labelPass.Visibility = Visibility.Collapsed;
                    TBLogin.Visibility = Visibility.Collapsed;
                    PasswordBox.Visibility = Visibility.Collapsed;

                    BTNChangePassword.Visibility = Visibility.Collapsed;
                    BTNEnter.Visibility = Visibility.Collapsed;
                    BTNReg.Visibility = Visibility.Collapsed;
                    return;
            }
        }

        public void CaptchaChange() {

            String allowchar = " ";
            allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,y,z";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            String[] ar = allowchar.Split(a);
            String pwd = "";
            string temp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            captcha.Text = pwd;
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча", "Ошибка");
                CaptchaChange();

            }
            else
            {
                MessageBox.Show("Капча введена успешно, можете продолжить авторизацию", "Успех");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
            e.Command == ApplicationCommands.Cut ||
            e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}
