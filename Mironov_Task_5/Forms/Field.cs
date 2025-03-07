using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mironov_Task_5.Forms
{
    public partial class Field : Form
    {
        private bool prod = false;
        private string fileName;
        private string receivedText;
        bool load = false;
        int[,] masMain = new int[9, 9];
        int[,] masGame = new int[9, 9];
        int[,] masUser = new int[9, 9];
        private DataGridView dgv;
        private Timer autoSaveTimer;
        private Button SaveFileButton = new Button();
        private Button TestButton = new Button();
        Panel panelStart = new Panel();
        private Button AgreeButton = new Button();
        TextBox textBoxFileName = new TextBox();



        public Field(string text)
        {
            InitializeComponent();
            receivedText = text.ToLower();

            this.Text = "Судоку: " + receivedText;
            this.Size = new Size(460, 550);

            CreateControls();

            generatedMassive(masMain);
            Array.Copy(masMain, masGame, masMain.Length);
            updateMas(masGame, receivedText);
            FillDataGridView(masGame);
        }

        public Field(string text, int[,] masMain, int[,]masGame, int[,] masUser)
        {
            InitializeComponent();
            fileName = text;
            this.masMain = masMain;
            this.masGame = masGame;
            this.masUser = masUser;

            this.Text = "Загружен файл: " + fileName;
            this.Size = new Size(460, 550);

            CreateControls();
            panelStart.Visible = false;
            panelStart.Enabled = false;

            FillDataGridView(masGame);
            FillDataGridViewUser(masUser);
        }

        private void CreateControls()
        {
            //Panel
            panelStart.Dock = DockStyle.Top;
            panelStart.Height = 500; 

            this.Controls.Add(panelStart);

            //textbox
            textBoxFileName.Location = new System.Drawing.Point(135, 230); // Устанавливаем местоположение
            textBoxFileName.Width = 200; // Устанавливаем ширину

            panelStart.Controls.Add(textBoxFileName);

            // Создаем кнопку
            AgreeButton.Text = "Подтвердить";
            AgreeButton.Location = new System.Drawing.Point(190, 260);

            AgreeButton.Click += AgreeName;

            panelStart.Controls.Add(AgreeButton);

            //SaveFileButton
            SaveFileButton.Text = "Сохранить";
            SaveFileButton.Size = new Size(100, 50);
            SaveFileButton.Location = new Point(175 , 450);

            SaveFileButton.Click += (sender, e) => SaveSudoku();

            this.Controls.Add(SaveFileButton);

            //TestButton
            TestButton.Text = "Подсмотреть";
            TestButton.Size = new Size(100, 50);
            TestButton.Location = new Point(20, 450);

                TestButton.Click += (sender, e) => OpenTestForm();

            if(!prod)
            this.Controls.Add(TestButton);

            //DataGridView
            dgv = new DataGridView
            {
                ColumnCount = 9,
                Width = 408,
                Height = 408,
                Location = new Point(20, 20),
                AllowUserToAddRows = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                ReadOnly = false,
                Font = new Font("Arial", 14, FontStyle.Bold),
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            dgv.ColumnHeadersVisible = false;
            dgv.RowHeadersVisible = false;

            for (int i = 0; i < 9; i++)
            {
                dgv.Columns[i].Width = 45;
                dgv.Rows.Add();
                dgv.Rows[i].Height = 45;
            }

            dgv.CellPainting += Dgv_CellPainting;
            dgv.CellValueChanged += dgv_CellValueChanged;


            this.Controls.Add(dgv);

            // Создаем и настраиваем таймер
            autoSaveTimer = new Timer();
            autoSaveTimer.Interval = 30000; // 30 секунд
            autoSaveTimer.Tick += AutoSaveTimer_Tick;
            autoSaveTimer.Start(); // Запуск таймера
        }

        //Подтверждение названия
        private void AgreeName(object sender, EventArgs e)
        {
            fileName = textBoxFileName.Text.Trim();
            fileName += $"_{receivedText}_{DateTime.Now:MM-dd_HH-mm}";
            if (string.IsNullOrWhiteSpace(fileName))
            {
                MessageBox.Show("Введите название файла!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show($"Файл сохранится как: {fileName}.json", "Подтверждено", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SaveSudoku();
                panelStart.Visible = false;
                panelStart.Enabled = false;
            }

        }

        //Отрисовка ячеек
        private void Dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            using (Pen p = new Pen(Color.Black, 2))
            {
                if (e.ColumnIndex % 3 == 2) 
                    e.Graphics.DrawLine(p, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);

                if (e.RowIndex % 3 == 2) 
                    e.Graphics.DrawLine(p, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
            }

            e.Handled = true;
        }

        //Генерация судоку
        private void generatedMassive(int[,] mas)
        {
            Random rand = new Random();
            //Генерация строк
            for (int rows = 0; rows < 9; rows++)
            {
                bool match;
                do
                {
                    //Рандомное заполнение строк
                    int[] row = Enumerable.Range(1, 9).OrderBy(n => rand.Next()).ToArray();
                    for (int columns = 0; columns < 9; columns++)
                        masMain[rows, columns] = row[columns];
                    match = CheckRowMatch(masMain, rows);
                } while (match == false);
            }
        }

        //Проверка новой строки
        public bool CheckRowMatch(int[,] sudoku, int rowIndex)
        {
            //Проверка по совпадениям в столбцах
            for (int i = 0; i < rowIndex; i++)
            {
                bool matchFound = true;
                for (int j = 0; j < 9; j++)
                {
                    if (sudoku[rowIndex, j] == sudoku[i, j])
                    {
                        matchFound = false;
                        break;
                    }
                }

               
                if (!matchFound)
                {
                    return false;
                }
            }


            //Проверка ячеек в строке
            //Определение, с какой строки начинаеться ячейка
            int countCellRows = (rowIndex / 3) * 3;

            //Номер ячейки слева на право
            for (int cell = 0; cell < 3; cell++)
            {
                HashSet<int> unique = new HashSet<int>();
                //Проход по строкам ячейки
                for(int row = countCellRows; row <= rowIndex; row++)
                {   
                    //Проход по столбацам ячейки
                    for(int column = cell * 3; column < (cell+1)*3; column++)
                    {
                        //Проверка наличия числа в контейнере
                        if (unique.Contains(sudoku[row,column]))
                        {
                            return false;
                        }
                        else
                        {
                            unique.Add(sudoku[row, column]);
                        }
                    }
                }
            }


                return true;
        }

        //Заполнение массива для игрока
        public void updateMas(int[,] sudoku, string difficult)
        {
            int takeElement;

            if (difficult == "легкий")
                takeElement = 4;
            else if (difficult == "средний")
                takeElement = 3;
            else
                takeElement = 2;

            Random rnd = new Random();

            for (int cellRows = 0; cellRows < 3;cellRows++)
            {
                for (int cellColumns = 0; cellColumns < 3; cellColumns++)
                {
                    HashSet<int> unique = new HashSet<int>(Enumerable.Range(1,9));

                    List<int> toRemove = unique.OrderBy(x => rnd.Next()).Take(takeElement).ToList();

                    foreach (var num in toRemove)
                    {
                        unique.Remove(num);
                    }
                    for(int row = cellRows*3; row < (cellRows + 1)* 3; row++ )
                    {
                        for (int column = cellColumns * 3; column< (cellColumns + 1) * 3; column++)
                        {
                            if (unique.Contains(sudoku[row, column]))
                            {
                                sudoku[row, column] = 0;
                            }
                        }
                    }
                }
            }
        }

        //Заполнение DataGridView
        private void FillDataGridView(int[,] mas)
        {
            for(int rows = 0; rows < 9; rows++)
            {
                for (int columns = 0; columns < 9; columns++)
                {
                    if (mas[rows, columns] != 0)
                    {
                        dgv.Rows[rows].Cells[columns].Value = mas[rows, columns];
                        dgv.Rows[rows].Cells[columns].ReadOnly = true; // Запрещаем редактирование
                        dgv.Rows[rows].Cells[columns].Style.BackColor = Color.Gray; // Меняем цвет
                    }
                }
            }
            load = true;
        }

        //Заполнение DataGridView
        private void FillDataGridViewUser(int[,] mas)
        {
            for (int rows = 0; rows < 9; rows++)
            {
                for (int columns = 0; columns < 9; columns++)
                {
                    if (mas[rows, columns] != 0)
                    {
                        dgv.Rows[rows].Cells[columns].Value = mas[rows, columns];
                        ValidateCell(rows, columns, mas[rows, columns]);
                    }
                }
            }
        }

        //Сохранение
        private void SaveSudoku()
        {
            if (string.IsNullOrEmpty(fileName))
            {
                MessageBox.Show("Сначала введите название файла!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                for (int rows = 0; rows < 9; rows++)
                {
                    for (int columns = 0; columns < 9; columns++)
                    {
                        if (dgv.Rows[rows].Cells[columns].Value != null && dgv.Rows[rows].Cells[columns].ReadOnly != true)
                        {
                            if (int.TryParse(dgv.Rows[rows].Cells[columns].Value.ToString(), out int cellValue))
                            {
                                masUser[rows, columns] = cellValue;
                            }
                            else
                            {
                                masUser[rows, columns] = 0;
                            }
                        }
                    }
                }

                var data = new
                {
                    Array1 = ConvertToJaggedArray(masMain),
                    Array2 = ConvertToJaggedArray(masGame),
                    Array3 = ConvertToJaggedArray(masUser)
                };

                string json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText($"{fileName}.json", json);
            }
            catch (Exception ex)
            { }
        }

        private void AutoSaveTimer_Tick(object sender, EventArgs e)
        {
            // Проверяем, есть ли имя файла перед сохранением
            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }
            else
            {
                SaveSudoku();
            }
        }

        // Метод для конвертации двумерного массива в ступенчатый массив (List<int[]>)
        private List<int[]> ConvertToJaggedArray(int[,] array)
        {
            var list = new List<int[]>();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                int[] row = new int[array.GetLength(1)];
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    row[j] = array[i, j];
                }
                list.Add(row);
            }
            return list;
        }

        //Событие изменения значения в ячейке
        private void dgv_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!load)
                return;

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];

                if (int.TryParse(cell.Value?.ToString(), out int newValue) && newValue >= 1 && newValue <= 9)
                {
                    ValidateCell(e.RowIndex, e.ColumnIndex, newValue);
                }
                else
                {
                    cell.Value = "";
                }
            }
        }

        // Валидация ячейки
        private void ValidateCell(int row, int col, int value)
        {
            bool isValid = true;

            // Проверка строки
            for (int c = 0; c < 9; c++)
            {
                if (c != col && dgv.Rows[row].Cells[c].Value?.ToString() == value.ToString())
                {
                    isValid = false;
                    break;
                }
            }

            // Проверка столбца
            for (int r = 0; r < 9; r++)
            {
                if (r != row && dgv.Rows[r].Cells[col].Value?.ToString() == value.ToString())
                {
                    isValid = false;
                    break;
                }
            }

            // Проверка 3 на 3 блока
            int startRow = (row / 3) * 3;
            int startCol = (col / 3) * 3;
            for (int r = startRow; r < startRow + 3; r++)
            {
                for (int c = startCol; c < startCol + 3; c++)
                {
                    if ((r != row || c != col) && dgv.Rows[r].Cells[c].Value?.ToString() == value.ToString())
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            // Если невалидное значение, помечаем красным
            var cell = dgv.Rows[row].Cells[col];
            if (!isValid)
            {
                cell.Style.BackColor = Color.Red;

                // Возвращаем цвет всех ячеек в блоке к обычному
                ResetBlockColors(row, col);
            }
            else
            {
                // Если ячейка валидна, перекрашиваем в белый или в зеленый
                cell.Style.BackColor = Color.White;

                CheckBlockCompletion(row, col);

                // Если значение валидное, проверяем, нужно ли перекрасить в зеленый
                bool isSolved = true;

                // Проверка всей таблицы на корректность
                for (int rows = 0; rows < 9; rows++)
                {
                    for (int columns = 0; columns < 9; columns++)
                    {
                        var cellValue = dgv.Rows[rows].Cells[columns].Value;
                        if (cellValue == null || cellValue.ToString() != masMain[rows, columns].ToString())
                        {
                            isSolved = false;
                            break;
                        }
                    }
                }

                // Если судоку решено верно
                if (isSolved)
                {
                    MessageBox.Show("Судоку решено верно!", "Поздравляем", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void CheckBlockCompletion(int row, int col)
        {
            int startRow = (row / 3) * 3;
            int startCol = (col / 3) * 3;

            bool isBlockValid = true;

            // Проверяем все ячейки блока, существует ли строка и ячейка
            for (int r = startRow; r < startRow + 3; r++)
            {
                if (r >= dgv.RowCount) continue;  // Проверяем, что строка существует

                for (int c = startCol; c < startCol + 3; c++)
                {
                    if (c >= dgv.ColumnCount) continue;  // Проверяем, что ячейка существует

                    var cell = dgv.Rows[r].Cells[c];
                    if (cell != null && (cell.Value == null || string.IsNullOrEmpty(cell.Value.ToString()) || cell.Style.BackColor == Color.Red))
                    {
                        isBlockValid = false;
                        break;
                    }
                }

                if (!isBlockValid)
                    break;
            }

            // Если блок валиден, перекрашиваем его в зелёный
            if (isBlockValid)
            {
                for (int r = startRow; r < startRow + 3; r++)
                {
                    if (r >= dgv.RowCount) continue;  // Проверяем, что строка существует

                    for (int c = startCol; c < startCol + 3; c++)
                    {
                        if (c >= dgv.ColumnCount) continue;  // Проверяем, что ячейка существует

                        var cell = dgv.Rows[r].Cells[c];
                        if (cell != null)
                            cell.Style.BackColor = Color.Green;
                    }
                }
            }
            else
            {
                // Если блок не валиден, возвращаем ячейки к обычному цвету
                for (int r = startRow; r < startRow + 3; r++)
                {
                    for (int c = startCol; c < startCol + 3; c++)
                    {
                        var cell = dgv.Rows[r].Cells[c];
                        if (cell.Value != null)
                        {
                            // Проверка цвета ячейки перед изменением
                            if (cell.Style.BackColor != Color.Red)
                            {
                                if (cell.Value.ToString() == masGame[r, c].ToString())
                                    cell.Style.BackColor = Color.Gray;
                                else
                                    cell.Style.BackColor = Color.White;
                            }
                        }
                    }
                }
            }
        }

        // Метод для сброса цвета всех ячеек в блоке к обычному состоянию
        private void ResetBlockColors(int row, int col)
        {
            int startRow = (row / 3) * 3;
            int startCol = (col / 3) * 3;

            for (int r = startRow; r < startRow + 3; r++)
            {
                for (int c = startCol; c < startCol + 3; c++)
                {
                    var cell = dgv.Rows[r].Cells[c];

                    if (cell.Value != null)
                    {
                        // Проверка цвета ячейки перед изменением
                        if (cell.Style.BackColor != Color.Red)
                        {
                            if (cell.Value.ToString() == masGame[r, c].ToString())
                                cell.Style.BackColor = Color.Gray;
                            else
                                cell.Style.BackColor = Color.White;
                        }
                    }
                }
            }
        }

        private void OpenTestForm()
        {
            TestForm form = new TestForm(masMain, fileName);
            form.Show();
        }

        private void Field_Load(object sender, EventArgs e)
        {

        }

    }
}

