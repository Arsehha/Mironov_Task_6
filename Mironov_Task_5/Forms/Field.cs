using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mironov_Task_5.Forms
{
    public partial class Field : Form
    {
        private string receivedText;
        int[,] masMain = new int[9, 9];
        int[,] masGame = new int[9, 9];
        private Timer timer;
        private DataGridView dgv;
        Button resultButton = new Button();
        System.Windows.Forms.Label resultLabel = new System.Windows.Forms.Label();

        public Field(string text)
        {
            InitializeComponent();
            receivedText = text.ToLower();

            this.Text = "Судоку: " + receivedText;
            this.Size = new Size(460, 550);

            timer = new Timer();
            timer.Interval = 3000;
            timer.Tick += Timer_Tick;

            CreateControls();

            generatedMassive(masMain);
            Array.Copy(masMain, masGame, masMain.Length);
            updateMas(masGame, receivedText);
            FillDataGridView(masGame);
        }

        private void CreateControls()
        {
            //Label
            resultLabel.Text = "Ваш результат";
            resultLabel.Size = new Size(300, 30);
            resultLabel.Location = new Point(140, 455);
            resultLabel.Font = new Font("Arial", 12, FontStyle.Bold);
            resultLabel.ForeColor = Color.Gray;
            resultLabel.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(resultLabel);

            //Button
            resultButton.Text = "Проверить себя";
            resultButton.Size = new Size(100, 50);
            resultButton.Location = new Point(25, 450);

            resultButton.Click += resultButton_Click;

            this.Controls.Add(resultButton);

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

            this.Controls.Add(dgv);
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

        //Результирующая кнопка
        private void resultButton_Click(object sender, EventArgs e)
        {
            int rows = dgv.RowCount;
            int columns = dgv.ColumnCount;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (dgv.Rows[row].Cells[col].Value != null)
                    {
                        if (masMain[row,col] == Convert.ToInt32(dgv.Rows[row].Cells[col].Value))
                        {

                        }
                        else
                        {
                            resultLabel.Text = "Судоку решено не верно";
                            resultLabel.ForeColor = Color.Red;
                            timer.Start();
                            return;
                        }
                    }
                    else
                    {
                        resultLabel.Text = "Судоку не полностью заполнено";
                        resultLabel.ForeColor = Color.Red;
                        timer.Start();
                        return;
                    }
                }
            }
            resultLabel.Text = "Судоку решено верно";
            resultLabel.ForeColor = Color.Green;
            timer.Start();
            return;
        }

        //Таймер для лейбла
        private void Timer_Tick(object sender, EventArgs e)
        {
            resultLabel.Text = "Ваш результат";
            resultLabel.ForeColor = Color.Gray;

            timer.Stop();
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
        }

        private void Field_Load(object sender, EventArgs e)
        {

        }

    }
}

