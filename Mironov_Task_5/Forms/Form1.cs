using Mironov_Task_5.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mironov_Task_5
{
    public partial class Form1 : Form
    {
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
    }
}
