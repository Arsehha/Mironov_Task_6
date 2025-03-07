using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mironov_Task_5.Forms
{
    public partial class TestForm : Form
    {
        private DataGridView dgv;
        public TestForm(int[,] mas, string text)
        {
            InitializeComponent();

            this.Text = $"Проверка {text}";
            this.Size = new Size(460, 550);

            CreateControls();

            FillDataGridView(mas);
        }

        private void CreateControls()
        {
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

        private void FillDataGridView(int[,] mas)
        {
            for (int rows = 0; rows < 9; rows++)
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

        private void TestForm_Load(object sender, EventArgs e)
        {

        }
    }
}
