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
    /// Логика взаимодействия для AddCategoryPage.xaml
    /// </summary>
    public partial class AddCategoryPage : Page
    {
        /// <summary>
        /// Создается приватное поле и в него записывается пустая, новая категория
        /// На страницу передается или выбранная категория (если мы вошли в режиме редактирования) или null (если вошли в режиме добавления)
        /// Если переданный объект не пустой (мы вошли не в режиме редактирования) то мы перезаписываем текущую категорию
        /// Переменная текущей категории назначается главным контекстом данных страницы
        /// </summary>
        private Category _currentCategory = new Category();
        public AddCategoryPage(Category selectedCategory)
        {
            InitializeComponent();
            if (selectedCategory != null)
                _currentCategory = selectedCategory;
            DataContext = _currentCategory;

        }
        /// <summary>
        /// Обрабатывает нажатие на кнопку сохранения категории.
        /// Проверяет заполнены ли все поля, если нет - выдает сообщение.
        /// Если ID категори равен 0 - она новая, поэтому добавляется в базу данных.
        /// Через try-catch происходит попытка сохранения в бд и возвращение на предыдущую страницу.
        /// Если чет все плохо все полетело то выводится ошибка
        /// </summary>
        private void ButtonSaveCategory_Click(object sender, RoutedEventArgs
        e)
        {
            if (string.IsNullOrWhiteSpace(_currentCategory.Name))
            {
                MessageBox.Show("Введите название категории!");
                return;
            }
            
            if (_currentCategory.ID == 0)
                Core.Context.Category.Add(_currentCategory);

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
        /// Очищает поле названия категории
        /// </summary>
        private void ButtonClean_Click(object sender, RoutedEventArgs e)
        {
            TBCategoryName.Text = "";
        }
        /// <summary>
        /// Возвращение назад
        /// </summary>
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
