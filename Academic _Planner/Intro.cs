using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Academic__Planner
{
    public partial class Intro : Form
    {
        public Intro()
        {
            InitializeComponent();
        }
        private void GoTo(Form form)
        {
            form.FormClosed += (s, e) =>
            {
                if (IsDisposed) return;
                if (!Application.OpenForms.Cast<Form>().Any(f => f != this && f.Visible))
                    Show();
            };
            form.Show();
            Hide();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            GoTo(new signup());

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            GoTo(new Login());

        }
    }
}
