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
    /// Логика взаимодействия для UsersTabPage.xaml
    /// </summary>
    public partial class UsersTabPage : Page
    {
        /// <summary>
        /// Передается список юзеров как источник данных. Страница "подписывается" на событие Page_IsVisibleChanged
        /// </summary>
        public UsersTabPage()
        {
            InitializeComponent();
            DataGridUser.ItemsSource = Lists.usersDBL;
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }
        /// <summary>
        /// Обрабатывает изменение видимости страницы.
        /// Если страница стала видимой, то обновляем состояние подключения к БД
        /// Переподключаем источник данных
        /// </summary>
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridUser.ItemsSource = Lists.usersDBL;
            }
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку добавления.
        /// Перекидывает пользователя на страницу добавления юзера и передает туда значение "null"
        /// </summary>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddUserPage(null));
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку удаления.
        /// Выводит сообщение с количеством выбранных записей для подтверждения удаления.
        /// Если да - удаляет из бд выбранные значения, сохраняет БД и обновляет источник ресурсов.
        /// Обрабатывает ошибки через try-catch
        /// </summary>
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var usersForRemoving = DataGridUser.SelectedItems.Cast<Users>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве { usersForRemoving.Count()} элементов ? ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    Core.Context.Users.RemoveRange(usersForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");

                    DataGridUser.ItemsSource = Core.Context.Users.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку изменения.
        /// Перекидывает пользователя на страницу добавления юзера и передает туда значение выбранного юзера для изменения.
        /// </summary>
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddUserPage((sender as Button).DataContext as Users));
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
