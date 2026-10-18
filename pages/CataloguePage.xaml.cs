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
        public CataloguePage()
        {
            InitializeComponent();

            // ИСПРАВЛЕНО: Убрана лишняя строка привязки, которая ломала "Все жанры"
            LoadCategories();

            CBSort.SelectedIndex = 0;

            UpdateSearch();
        }

        private void LoadCategories()
        {
            if (Lists.categoriesDBL == null) return;

            // 1. Создаем новый временный список, куда первым элементом пишем заглушку
            var displayCategories = new List<Category>();
            displayCategories.Add(new Category { ID = 0, Name = "Все жанры" });

            // 2. Добавляем в этот список все реальные жанры из вашей БД
            displayCategories.AddRange(Lists.categoriesDBL);

            // 3. Привязываем получившийся список к ComboBox
            CBFilter.ItemsSource = displayCategories;
            CBFilter.DisplayMemberPath = "Name";
            CBFilter.SelectedValuePath = "ID";   // ИСПРАВЛЕНО: Регистр ID должен совпадать с тем, что в базе

            // 4. Автоматически выбираем самый первый элемент — "Все жанры"
            CBFilter.SelectedIndex = 0;
        }

        private void UpdateSearch()
        {
            if (Lists.gamesDBL == null) return;

            // ИСПРАВЛЕНО: Делаем переменную типом IEnumerable, чтобы LINQ-методы могли спокойно перезаписывать данные
            IEnumerable<Games> filteredGames = Lists.gamesDBL;

            if (!string.IsNullOrWhiteSpace(TBSearch.Text))
            {
                string searchText = TBSearch.Text.ToLower().Trim();
                filteredGames = filteredGames.Where(g => g.Name != null && g.Name.ToLower().Contains(searchText));
            }

            // 3. ПРИМЕНЯЕМ ФИЛЬТР ПО ЖАНРУ
            if (CBFilter.SelectedItem is Category selectedCategory)
            {
                if (selectedCategory.Name != "Все жанры")
                {
                    filteredGames = filteredGames.Where(g => g.CategoryID == selectedCategory.ID);
                }
            }

            // 4. ПРИМЕНЯЕМ СОРТИРОВКУ ПО АЛФАВИТУ
            if (CBSort.SelectedIndex == 1) // А — Я
            {
                filteredGames = filteredGames.OrderBy(g => g.Name);
            }
            else if (CBSort.SelectedIndex == 2) // Я — А
            {
                filteredGames = filteredGames.OrderByDescending(g => g.Name);
            }

            // 5. Отдаем итоговый отфильтрованный список в ListBox карточек
            LBCatalogue.ItemsSource = filteredGames.ToList();
        }

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
