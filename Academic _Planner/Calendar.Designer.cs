namespace Academic__Planner
{
    partial class Calendar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            guna2ShadowPanel5 = new Guna.UI2.WinForms.Guna2ShadowPanel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            button1 = new Button();
            SuspendLayout();
            // 
            // guna2ShadowPanel5
            // 
            guna2ShadowPanel5.BackColor = Color.Transparent;
            guna2ShadowPanel5.FillColor = Color.White;
            guna2ShadowPanel5.Location = new Point(45, 55);
            guna2ShadowPanel5.Name = "guna2ShadowPanel5";
            guna2ShadowPanel5.Radius = 10;
            guna2ShadowPanel5.ShadowColor = Color.Black;
            guna2ShadowPanel5.ShadowDepth = 50;
            guna2ShadowPanel5.ShadowShift = 2;
            guna2ShadowPanel5.Size = new Size(972, 512);
            guna2ShadowPanel5.TabIndex = 9;
            guna2ShadowPanel5.Paint += guna2ShadowPanel5_Paint;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.Location = new Point(45, 23);
            guna2HtmlLabel1.Margin = new Padding(2);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(82, 27);
            guna2HtmlLabel1.TabIndex = 3;
            guna2HtmlLabel1.Text = "Calendar";
            // 
            // button1
            // 
            button1.Location = new Point(1352, 23);
            button1.Name = "button1";
            button1.Size = new Size(93, 43);
            button1.TabIndex = 10;
            button1.Text = "Back";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Calendar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1473, 751);
            Controls.Add(button1);
            Controls.Add(guna2HtmlLabel1);
            Controls.Add(guna2ShadowPanel5);
            Name = "Calendar";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2ShadowPanel guna2ShadowPanel5;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Button button1;
    }
}