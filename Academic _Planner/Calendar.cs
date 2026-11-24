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
    public partial class Calendar : Form
    {
        public Calendar()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void guna2ShadowPanel5_Paint(object sender, PaintEventArgs e)
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
        // In Calendar.cs
        private void Calendar_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        private void Calendar_Load(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text) || cmbDifficulty.SelectedItem == null)
            {
                MessageBox.Show("Please fill in the title and difficulty.");
                return;
            }

            if (dtpEnd.Value < dtpStart.Value)
            {
                MessageBox.Show("End date can't be before the start date.");
                return;
            }

            ActivityStore.Activities.Add(new ActivityItem
            {
                Title = txtTitle.Text,
                StartAt = dtpStart.Value,
                Difficulty = cmbDifficulty.SelectedItem.ToString(),
                EndAt = dtpEnd.Value
            });

            MessageBox.Show("Activity saved!");
            txtTitle.Clear();
            cmbDifficulty.SelectedIndex = -1;
            dtpStart.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now;
            MessageBox.Show("Total saved: " + ActivityStore.Activities.Count);
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
    }
}
