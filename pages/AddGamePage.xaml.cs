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
    /// Логика взаимодействия для AddGamePage.xaml
    /// </summary>
    public partial class AddGamePage : Page
    {
        /// <summary>
        /// Создается локальная переменная текущей игры
        /// После инициализации компонентов привязываются ресурсы для ComboBox
        /// Если пользователь зашел в режиме редактирования (передана игра), то текущей игре присваивается выбранная
        /// </summary>
        private Games _currentGame = new Games();
        public AddGamePage(Games selectedGame)
        {
            InitializeComponent();

            CBCategory.ItemsSource = Lists.categoriesDBL;
            CBCategory.DisplayMemberPath = "Name";
            CBUser.ItemsSource = Lists.usersDBL;
            CBUser.DisplayMemberPath = "FIO";

            if (selectedGame != null)
                _currentGame = selectedGame;
            DataContext = _currentGame;
        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку сохранения.
        /// Проверяет заполненность полей, если есть как минимум одн пустое - выводит сообщение с ошибкой
        /// Проверяет тип данных поля цены, если не Int - выводит сообщение о необходимости изменить цену
        /// Данные из полей ввода и выпадающих списков переносятся в поля текущей игры
        /// Если игра новая (ID = 0), то добавляется в БД
        /// Через try-catch осуществляется попытка сохранения БД. Если все ок - выводится сообщение и пользователя перекидывает назад, если не - выводится ошибка
        /// </summary>
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TBGameName.Text) ||
                string.IsNullOrWhiteSpace(TBPrice.Text) ||
                CBUser.SelectedValue == null ||
                CBCategory.SelectedValue == null)
            {
                MessageBox.Show("Не все данные заполнены!");
                return;
            }

            if (!int.TryParse(TBPrice.Text, out int validatedPrice) || validatedPrice < 0)
            {
                MessageBox.Show("Цена может быть только положительным числом!");
                return;
            }

            _currentGame.Name = TBGameName.Text.Trim();
            _currentGame.Price = validatedPrice;

            _currentGame.UserID = (int)CBUser.SelectedValue;
            _currentGame.CategoryID = (int)CBCategory.SelectedValue;

            if (_currentGame.ID == 0)
            {
                Core.Context.Games.Add(_currentGame);
            }

            try
            {
                Core.Context.SaveChanges();
                MessageBox.Show("Данные успешно сохранены!");
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }

        /// <summary>
        /// Обрабатывает нажатие на кнопку очистки.
        /// Очищает все поля.
        /// </summary>
        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBGameName.Text = "";
            TBPrice.Text = "";
            TBDescription.Text = "";
            CBCategory.SelectedIndex = 0;
            CBUser.SelectedIndex = 0;
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
