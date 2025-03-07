using Mironov_Task_5.Class;
using Mironov_Task_5.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mironov_Task_5
{
    public partial class Form1 : Form
    {
        private string fileName;
        int[,] masMain = new int[9, 9];
        int[,] masGame = new int[9, 9];
        int[,] masUser = new int[9, 9];
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonStartEz_Click(object sender, EventArgs e)
        {
            Field form = new Field(buttonStartEz.Text);
            form.Show();
        }

        private void buttonStartMed_Click(object sender, EventArgs e)
        {
            Field form = new Field(buttonStartMed.Text);
            form.Show();
        }

        private void buttonStartHard_Click(object sender, EventArgs e)
        {
            Field form = new Field(buttonStartHard.Text);
            form.Show();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Sudoku JSON (*.json)|*.json";
                openFileDialog.Title = "Выберите файл судоку";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    fileName = Path.GetFileNameWithoutExtension(filePath);
                    try
                    {
                        string json = File.ReadAllText(filePath);
                        var data = JsonSerializer.Deserialize<SudokuData>(json);

                        if (data != null)
                        {
                            masMain = ConvertTo2DArray(data.Array1);
                            masGame = ConvertTo2DArray(data.Array2);
                            masUser = ConvertTo2DArray(data.Array3);
                        }
                        Field form = new Field(fileName, masMain, masGame, masUser);
                        form.Show();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка загрузки файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // Метод конвертации ступенчатого массива в двумерный
        private int[,] ConvertTo2DArray(List<int[]> list)
        {
            int size = list.Count;
            int[,] array = new int[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    array[i, j] = list[i][j];

            return array;
        }
    }
}
