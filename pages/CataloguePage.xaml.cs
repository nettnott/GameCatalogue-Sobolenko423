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
    /// Логика взаимодействия для CataloguePage.xaml
    /// </summary>
    public partial class CataloguePage : Page
    {
        /// <summary>
        /// Вызывает функцию загрузки категорий, выбирает изначальную сортировку
        /// </summary>
        public CataloguePage()
        {
            InitializeComponent();
            LoadCategories();

            CBSort.SelectedIndex = 0;

            UpdateSearch();
        }
        /// <summary>
        /// Функция загрузки категорий.
        /// Создается переменная для нового списка категорий, в котором задается первый элемент - все жанры.
        /// В этот список добавляется список категорий из БД.
        /// Список назначается источником для ComboBox, определяется отображаемая величина и выбранная. Задается базовый выбранный элемент
        /// </summary>
        private void LoadCategories()
        {
            if (Lists.categoriesDBL == null) return;

            var displayCategories = new List<Category>();
            displayCategories.Add(new Category { ID = 0, Name = "Все жанры" });

            displayCategories.AddRange(Lists.categoriesDBL);

            CBFilter.ItemsSource = displayCategories;
            CBFilter.DisplayMemberPath = "Name";
            CBFilter.SelectedValuePath = "ID";

            CBFilter.SelectedIndex = 0;
        }
        /// <summary>
        /// Обновляет данные для ListBox при вводе поискового запроса/сортировки/фильтра
        /// </summary>
        private void UpdateSearch()
        {
            if (Lists.gamesDBL == null) return;

            IEnumerable<Games> filteredGames = Lists.gamesDBL;

            // Выборка игр, где имя соответствует поиску
            if (!string.IsNullOrWhiteSpace(TBSearch.Text))
            {
                string searchText = TBSearch.Text.ToLower().Trim();
                filteredGames = filteredGames.Where(g => g.Name != null && g.Name.ToLower().Contains(searchText));
            }

            // Применение фильтра по жанру
            if (CBFilter.SelectedItem is Category selectedCategory)
            {
                if (selectedCategory.Name != "Все жанры")
                {
                    filteredGames = filteredGames.Where(g => g.CategoryID == selectedCategory.ID);
                }
            }

            // Сортиковка по алфавиту
            if (CBSort.SelectedIndex == 1) // А — Я
            {
                filteredGames = filteredGames.OrderBy(g => g.Name);
            }
            else if (CBSort.SelectedIndex == 2) // Я — А
            {
                filteredGames = filteredGames.OrderByDescending(g => g.Name);
            }

            // Возвращение итогового списка
            LBCatalogue.ItemsSource = filteredGames.ToList();
        }
        /// <summary>
        /// Обрабатывает изменения текста в поисковой строке или выбора элемента ComboBox.
        /// Вызывает функцию обновления каталога.
        /// </summary>
        private void TBSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSearch();
        }

        private void CBSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSearch();
        }

        private void CBFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSearch();
        }
    }
}
