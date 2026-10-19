using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;

namespace GameCatalogue.pages
{
    /// <summary>
    /// Логика взаимодействия для DiagramsPage.xaml
    /// </summary>
    public partial class DiagramsPage : Page
    {
        public DiagramsPage()
        {
            InitializeComponent();

            ChartGames.ChartAreas.Clear();
            ChartGames.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Количество игр")
            {
                IsValueShownAsLabel = true
            };
            ChartGames.Series.Clear();
            ChartGames.Series.Add(currentSeries);

            CBuser.ItemsSource = Lists.usersDBL;
            CBdiagrammType.ItemsSource = Enum.GetValues(typeof(SeriesChartType));

            CBdiagrammType.SelectedItem = SeriesChartType.Column;
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CBuser.SelectedItem is Users currentUser &&
                CBdiagrammType.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartGames.Series.FirstOrDefault();
                if (currentSeries == null) return;

                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                if (Lists.categoriesDBL == null || Lists.gamesDBL == null) return;

                foreach (var category in Lists.categoriesDBL)
                {
                    int gamesCount = Lists.gamesDBL.Count(g => g.UserID == currentUser.ID && g.CategoryID == category.ID);

                    if (gamesCount > 0)
                    {
                        currentSeries.Points.AddXY(category.Name, gamesCount);
                    }
                }
            }
        }

        private void BTNExcel_Click(object sender, RoutedEventArgs e)
        {
            if (Lists.usersDBL == null || Lists.categoriesDBL == null || Lists.gamesDBL == null)
            {
                MessageBox.Show("Данные базы данных не загружены!");
                return;
            }

            var allUsers = Lists.usersDBL;
            var allCategories = Lists.categoriesDBL;

            // 1. Создаем объект приложения Excel и новую книгу
            var excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Add();

            try
            {
                // Перебираем пользователей и для каждого генерируем данные
                for (int uIndex = 0; uIndex < allUsers.Count; uIndex++)
                {
                    var user = allUsers[uIndex];
                    Excel.Worksheet worksheet;

                    // В новой книге Excel по умолчанию уже есть один лист. 
                    // Используем его для первого юзера, а для остальных — создаем новые.
                    if (uIndex == 0)
                    {
                        worksheet = (Excel.Worksheet)workbook.Sheets[1];
                    }
                    else
                    {
                        worksheet = (Excel.Worksheet)workbook.Sheets.Add(Type.Missing, workbook.Sheets[workbook.Sheets.Count]);
                    }

                    // Именуем лист по ФИО пользователя (Excel не любит имена длиннее 31 символа, обрезаем на всякий случай)
                    string sheetName = !string.IsNullOrWhiteSpace(user.FIO) ? user.FIO : $"Пользователь {user.ID}";
                    if (sheetName.Length > 30) sheetName = sheetName.Substring(0, 30);
                    worksheet.Name = sheetName;

                    // 2. Создаем шапку и название таблицы
                    worksheet.Cells[1, 1] = $"Статистика добавленных игр: {user.FIO}";
                    worksheet.Cells[3, 1] = "Категория (Жанр)";
                    worksheet.Cells[3, 2] = "Количество игр";

                    // 3. Заполняем строки данными по категориям
                    int currentRow = 4;
                    for (int i = 0; i < allCategories.Count; i++)
                    {
                        var currentCategory = allCategories[i];

                        // Считаем игры из глобального списка для текущего юзера и текущей категории
                        int gamesCount = Lists.gamesDBL.Count(g => g.UserID == user.ID && g.CategoryID == currentCategory.ID);

                        worksheet.Cells[currentRow, 1] = currentCategory.Name;
                        worksheet.Cells[currentRow, 2] = gamesCount;
                        currentRow++;
                    }

                    // 4. Дополнительная статистика (Самая дорогая игра)
                    var userGames = Lists.gamesDBL.Where(g => g.UserID == user.ID).ToList();
                    var maxPriceGame = userGames.OrderByDescending(g => g.Price).FirstOrDefault();

                    if (maxPriceGame != null)
                    {
                        currentRow++; // Пропускаем пустую строку
                        worksheet.Cells[currentRow, 1] = "Самая дорогая игра:";
                        worksheet.Cells[currentRow, 2] = maxPriceGame.Name;

                        currentRow++;
                        worksheet.Cells[currentRow, 1] = "Стоимость:";

                        // ИСПРАВЛЕНО: Форматируем цену в C# и передаем в ячейку как готовую текстовую строку
                        string formattedPrice = maxPriceGame.Price != null
                            ? ((decimal)maxPriceGame.Price).ToString("N2")
                            : "0.00";

                        worksheet.Cells[currentRow, 2] = $"{formattedPrice} руб.";
                    }

                    // 5. КРАСИВОЕ ОФОРМЛЕНИЕ И СТИЛИЗАЦИЯ (Согласно методичке)
                    // Шрифт для всего листа
                    Excel.Range allRange = worksheet.Range["A1", $"B{currentRow}"];
                    allRange.Font.Name = "Times New Roman";

                    // Оформление главного заголовка страницы
                    Excel.Range titleRange = worksheet.Range["A1", "B1"];
                    titleRange.Merge(); // Объединяем ячейки А1 и В1
                    titleRange.Font.Size = 16;
                    titleRange.Font.Bold = true;
                    titleRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // Оформление шапки таблицы (Строка 3)
                    Excel.Range headerRange = worksheet.Range["A3", "B3"];
                    headerRange.Font.Size = 12;
                    headerRange.Font.Bold = true;
                    headerRange.Interior.Color = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.LightGray);
                    headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                    // Включаем границы (сетку) для таблицы данных
                    Excel.Range tableDataRange = worksheet.Range["A3", $"B{3 + allCategories.Count}"];
                    tableDataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    tableDataRange.Borders.Weight = Excel.XlBorderWeight.xlThin;

                    // Автоподбор ширины столбцов под размер текста
                    worksheet.Columns[1].AutoFit();
                    worksheet.Columns[2].AutoFit();
                }

                // 6. Безопасное сохранение результирующих файлов в папку "Документы"
                string excelPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Games_Report.xlsx");

                // Отключаем системные предупреждения Excel перед сохранением
                excelApp.DisplayAlerts = false;
                workbook.SaveAs(excelPath);

                MessageBox.Show($"Отчет Excel успешно сгенерирован и сохранен в Документы!\nПуть: {excelPath}", "Успех");

                // Делаем Excel видимым на экране
                excelApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при работе с Excel: {ex.Message}");
                // Закрываем книгу без сохранения, если произошел сбой, чтобы очистить память
                workbook?.Close(false);
                excelApp?.Quit();
            }
        }


        private void BTNWord_Click(object sender, RoutedEventArgs e)
        {
            if (Lists.usersDBL == null || Lists.categoriesDBL == null || Lists.gamesDBL == null)
            {
                MessageBox.Show("Данные базы данных не загружены!");
                return;
            }

            var allUsers = Lists.usersDBL;
            var allCategories = Lists.categoriesDBL;

            // Создаем приложение Word и новый документ
            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            try
            {
                foreach (var user in allUsers)
                {
                    // 1. Имя пользователя (Заголовок)
                    Word.Paragraph userParagraph = document.Paragraphs.Add();
                    Word.Range userRange = userParagraph.Range;
                    userRange.Text = user.FIO ?? "Пользователь без ФИО";
                    userParagraph.set_Style("Заголовок 1");
                    userRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    userRange.InsertParagraphAfter();

                    // 2. Создание таблицы категорий игр для текущего юзера
                    Word.Paragraph tableParagraph = document.Paragraphs.Add();
                    Word.Range tableRange = tableParagraph.Range;

                    Word.Table gamesTable = document.Tables.Add(tableRange, allCategories.Count + 1, 2);

                    gamesTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    gamesTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    gamesTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    // Заполняем шапку таблицы
                    Word.Range cellRange;
                    cellRange = gamesTable.Cell(1, 1).Range;
                    cellRange.Text = "Категория (Жанр)";
                    cellRange = gamesTable.Cell(1, 2).Range;
                    cellRange.Text = "Количество добавленных игр";

                    gamesTable.Rows[1].Range.Font.Name = "Times New Roman";
                    gamesTable.Rows[1].Range.Font.Size = 14;
                    gamesTable.Rows[1].Range.Bold = 1;
                    gamesTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    // Заполняем строки таблицы данными
                    for (int i = 0; i < allCategories.Count; i++)
                    {
                        var currentCategory = allCategories[i];

                        cellRange = gamesTable.Cell(i + 2, 1).Range;
                        cellRange.Text = currentCategory.Name;
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;

                        cellRange = gamesTable.Cell(i + 2, 2).Range;
                        int userGamesInCatCount = Lists.gamesDBL.Count(g => g.UserID == user.ID && g.CategoryID == currentCategory.ID);

                        cellRange.Text = userGamesInCatCount.ToString();
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;
                        cellRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    }

                    document.Paragraphs.Add();

                    // 3. Дополнительная статистика
                    var userGames = Lists.gamesDBL.Where(g => g.UserID == user.ID).ToList();
                    var maxPriceGame = userGames.OrderByDescending(g => g.Price).FirstOrDefault();

                    if (maxPriceGame != null)
                    {
                        Word.Paragraph statParagraph = document.Paragraphs.Add();
                        Word.Range statRange = statParagraph.Range;

                        string formattedPrice = maxPriceGame.Price != null ? ((decimal)maxPriceGame.Price).ToString("N2") : "0.00";

                        statRange.Text = $"Самая дорогостоящая разработанная/добавленная игра: «{maxPriceGame.Name}» стоимостью {formattedPrice} руб.";
                        statParagraph.set_Style("Обычный");
                        statRange.Font.Name = "Times New Roman";
                        statRange.Font.Size = 12;
                        statRange.Font.Bold = 1;
                        statRange.Font.Color = Word.WdColor.wdColorDarkRed;
                        statRange.InsertParagraphAfter();
                    }

                    if (user != allUsers.LastOrDefault())
                    {
                        Word.Paragraph breakParagraph = document.Paragraphs.Add();
                        breakParagraph.Range.InsertBreak(Word.WdBreakType.wdPageBreak);
                    }
                }

                // ИСПРАВЛЕНО: Безопасное сохранение файлов
                // Чтобы не было проблем с правами диска С/D, сохраняем в "Мои Документы" текущего юзера Windows
                string docPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Games.docx");
                string pdfPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Games.pdf");

                // Сначала сохраняем DOCX
                document.SaveAs2(docPath);

                // Для PDF используем специализированный метод экспорта, чтобы не вешать ворд
                document.ExportAsFixedFormat(pdfPath, Word.WdExportFormat.wdExportFormatPDF);

                MessageBox.Show($"Отчеты успешно сохранены в папку 'Документы'!\nDOCX: {docPath}\nPDF: {pdfPath}", "Успех");

                // Делаем Word видимым для пользователя
                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при работе с Word: {ex.Message}");

                // В случае падения закрываем процесс, чтобы не забивать оперативку
                document?.Close(Word.WdSaveOptions.wdDoNotSaveChanges);
                application?.Quit();
            }
        }


        private void BTNback_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}
