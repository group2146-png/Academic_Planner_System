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
            this.WindowState = FormWindowState.Maximized;
            LoadActivities();
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
        private void MyActivities_Load(object sender, EventArgs e)
        {
            LoadActivities();
        }

        private void LoadActivities()
        {
            var titles = new[] { guna2HtmlLabel2, guna2HtmlLabel3, guna2HtmlLabel4, guna2HtmlLabel5 };
            var starts = new[] { guna2HtmlLabel6, guna2HtmlLabel7, guna2HtmlLabel8, guna2HtmlLabel9 };
            var diffs = new[] { guna2HtmlLabel10, guna2HtmlLabel11, guna2HtmlLabel12, guna2HtmlLabel13 };
            var ends = new[] { guna2HtmlLabel14, guna2HtmlLabel15, guna2HtmlLabel16, guna2HtmlLabel17 };
            var status = new[] { guna2HtmlLabel22, guna2HtmlLabel23, guna2HtmlLabel24, guna2HtmlLabel25 };

            for (int i = 0; i < titles.Length; i++)
            {
                if (i < ActivityStore.Activities.Count)
                {
                    var a = ActivityStore.Activities[i];
                    titles[i].Text = a.Title;
                    starts[i].Text = a.StartAt.ToString("MMM dd, yyyy hh:mm tt");
                    diffs[i].Text = a.Difficulty;
                    ends[i].Text = a.EndAt.ToString("MMM dd, yyyy hh:mm tt");
                    status[i].Text = a.Status;
                }
                else
                {
                    titles[i].Text = starts[i].Text = diffs[i].Text =
                    ends[i].Text = status[i].Text = "";
                }
            }
        }

        private void guna2Panel4_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
