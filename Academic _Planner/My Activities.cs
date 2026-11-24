using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;

namespace Academic__Planner
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();

        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();
            if (dashboard == null)
                dashboard = new Dashboard();

            dashboard.Show();
            this.Close();
        }
        // In My Activities.cs
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }


        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            Dashboard DashboardForm = new Dashboard();
            DashboardForm.Show();
            this.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Calendar calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Form2 activitiesForm = new Form2();
            activitiesForm.Show();
            this.Hide();
        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Stats statsForm = new Stats();
            statsForm.Show();
            this.Hide();
        }

        private void guna2HtmlLabel26_Click(object sender, EventArgs e)
        {

        }
    }
}
