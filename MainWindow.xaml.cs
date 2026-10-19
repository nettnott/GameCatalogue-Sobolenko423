using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
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
using GameCatalogue.pages;

namespace GameCatalogue
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new CataloguePage());
        }
        /// <summary>
        /// Обрабатывает загрузку окна.
        /// При загрузке создает переменную таймера, передает ее в TextBlock TBDateTime для отображения. 
        /// Вызывает функцию обновления видимости меню.
        /// </summary>

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => {
                TBDateTime.Text = DateTime.Now.ToString();
            };
            timer.Start();

            UpdateMenuVisibility();
        }
        /// <summary>
        /// Обновляет видимость меню в зависимости в аккаунте человек/не в аккаунте и если в аккаунте, то в зависимости от его роли.
        /// </summary>
        public void UpdateMenuVisibility()
        {
            if (Lists.currentUser != null)
            {
                // Если человек вошел в аккаунт идет проверка на роль и меняется видимость элементов
                BTNadmin.Visibility = Lists.currentUser.Role?.ToUpper() == "ADMIN" ? Visibility.Visible : Visibility.Collapsed;
                BTNLogout.Visibility = Visibility.Visible;
            }
            else
            {
                // Если не вошел - все ненужное скрывается
                BTNadmin.Visibility = Visibility.Collapsed;
                BTNLogout.Visibility = Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Обрабатывает закрытие окна.
        /// Выводит сообщение для подтверждения закрытия.
        /// </summary>
        void Window_Closing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите закрыть приложение?", "Message", MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.No)
                e.Cancel = true;
            else
                e.Cancel = false;
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку каталога.
        /// Выполняет перенаправление MainFrame на страницу с каталогом.
        /// </summary>
        private void BTNcatalogue_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CataloguePage());
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку аккаунта.
        /// Выполняет проверку, вошел ли юзер в аккаунт. Если да - переход на страницу самого аккаунта, если нет - на страницу авторизации.
        /// </summary>
        private void BTNaccount_Click(object sender, RoutedEventArgs e)
        {
            if (Lists.currentUser == null)
            {
                MainFrame.Navigate (new LogInPage());
            }
            else 
            { 
                MainFrame.Navigate(new AccountPage());
            }
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку админа.
        /// Выполняет переход на страницу для администрирования.
        /// </summary>
        private void BTNadmin_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AdminPage());
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку выхода из аккаунта.
        /// Выводит сообщение с подтверждением.
        /// Если действительно пользователь хочет выйти - обнуляет глобальню переменную текущего пользователя, вызывает функцию обновления видимости меню и перекидывает на страницу каталога.
        /// </summary>
        private void BTNLogout_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти из аккаунта?", "Message", MessageBoxButton.YesNo) == System.Windows.MessageBoxResult.Yes)
            {
                Lists.currentUser = null;
                UpdateMenuVisibility();
                MainFrame.Navigate(new CataloguePage());
            }
        }
        /// <summary>
        /// Обрабатывает нажатие CheckBox для изменения темы.
        /// Если CheckBox нажат, то:
        /// 1. Создается переменная uri с относительной ссылкой на словарь ресурсов (для темной темы)
        /// 2. Загружается файл по ссылке из ресурсов проекта и сохраняется как словарь ресурсов (стилей)
        /// 3. Очищается текущий словарь ресурсов чтобы они не спорили
        /// 4. В коллекцию ресурсов добавляется нужный словарь стилей
        /// </summary>
        private void ChbTheme_Checked(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("CyberpunkDictionary.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
        /// <summary>
        /// Обрабатывает нажатие CheckBox для изменения темы.
        /// Если CheckBox нажат, то:
        /// 1. Создается переменная uri с относительной ссылкой на словарь ресурсов (для светлой темы)
        /// 2. Загружается файл по ссылке из ресурсов проекта и сохраняется как словарь ресурсов (стилей)
        /// 3. Очищается текущий словарь ресурсов чтобы они не спорили
        /// 4. В коллекцию ресурсов добавляется нужный словарь стилей
        /// </summary>
        private void ChbTheme_Unchecked(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("LightDictionary.xaml", UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;

            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }
    }
}
