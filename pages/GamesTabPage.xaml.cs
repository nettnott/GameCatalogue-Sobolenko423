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
    /// Логика взаимодействия для GamesTabPage.xaml
    /// </summary>
    public partial class GamesTabPage : Page
    {
        /// <summary>
        /// Передается список игр как источник данных. Страница "подписывается" на событие Page_IsVisibleChanged
        /// </summary>
        public GamesTabPage()
        {
            InitializeComponent();
            DataGridPayment.ItemsSource = Core.Context.Games.ToList();
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
                DataGridPayment.ItemsSource = Core.Context.Games.ToList();
            }
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку добавления.
        /// Перекидывает пользователя на страницу добавления игры и передает туда значение "null"
        /// </summary>
        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AddGamePage(null));
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку удаления.
        /// Выводит сообщение с количеством выбранных записей для подтверждения удаления.
        /// Если да - удаляет из бд выбранные значения, сохраняет БД и обновляет источник ресурсов.
        /// Обрабатывает ошибки через try-catch
        /// </summary>
        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var paymentForRemoving =
            DataGridPayment.SelectedItems.Cast<Games>().ToList();
            if (MessageBox.Show($"Вы точно хотите удалить записи в количестве { paymentForRemoving.Count()} элементов ? ", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
{
                try
                {
                    Core.Context.Games.RemoveRange(paymentForRemoving);
                    Core.Context.SaveChanges();
                    MessageBox.Show("Данные успешно удалены!");
                    DataGridPayment.ItemsSource = Core.Context.Games.ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку изменения.
        /// Перекидывает пользователя на страницу добавления игры и передает туда значение выбранной игры для изменения.
        /// </summary>
        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddGamePage((sender as Button).DataContext as Games));
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
