namespace Mironov_Task_5
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonStartHard = new System.Windows.Forms.Button();
            this.buttonStartMed = new System.Windows.Forms.Button();
            this.buttonStartEz = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStartHard
            // 
            this.buttonStartHard.Location = new System.Drawing.Point(113, 138);
            this.buttonStartHard.Name = "buttonStartHard";
            this.buttonStartHard.Size = new System.Drawing.Size(100, 23);
            this.buttonStartHard.TabIndex = 0;
            this.buttonStartHard.Text = "Трудный";
            this.buttonStartHard.UseVisualStyleBackColor = true;
            this.buttonStartHard.Click += new System.EventHandler(this.buttonStartHard_Click);
            // 
            // buttonStartMed
            // 
            this.buttonStartMed.Location = new System.Drawing.Point(113, 94);
            this.buttonStartMed.Name = "buttonStartMed";
            this.buttonStartMed.Size = new System.Drawing.Size(100, 23);
            this.buttonStartMed.TabIndex = 1;
            this.buttonStartMed.Text = "Средний";
            this.buttonStartMed.UseVisualStyleBackColor = true;
            this.buttonStartMed.Click += new System.EventHandler(this.buttonStartMed_Click);
            // 
            // buttonStartEz
            // 
            this.buttonStartEz.Location = new System.Drawing.Point(113, 49);
            this.buttonStartEz.Name = "buttonStartEz";
            this.buttonStartEz.Size = new System.Drawing.Size(100, 23);
            this.buttonStartEz.TabIndex = 2;
            this.buttonStartEz.Text = "Легкий";
            this.buttonStartEz.UseVisualStyleBackColor = true;
            this.buttonStartEz.Click += new System.EventHandler(this.buttonStartEz_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 224);
            this.Controls.Add(this.buttonStartEz);
            this.Controls.Add(this.buttonStartMed);
            this.Controls.Add(this.buttonStartHard);
            this.Name = "Form1";
            this.Text = "Судоку";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStartHard;
        private System.Windows.Forms.Button buttonStartMed;
        private System.Windows.Forms.Button buttonStartEz;
    }
}

