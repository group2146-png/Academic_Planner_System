
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Academic__Planner
{
    public partial class Calendar : Form
    {
        public Calendar()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

        }


        // ---------- FORM EVENTS ----------
        private void Calendar_Load(object sender, EventArgs e)
        {
        }

        private void Calendar_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
        }

        // ---------- SAVE ACTIVITY ----------
        

        // ---------- NAVIGATION ----------
        private void button1_Click(object sender, EventArgs e)   // Back button
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();

            if (dashboard == null)
                dashboard = new Dashboard();

            dashboard.Show();
            this.Close();
        }

        private void guna2Button1_Click(object sender, EventArgs e)   // Dashboard
        {
            Dashboard DashboardForm = new Dashboard();
            DashboardForm.Show();
            this.Hide();
        }

        private void guna2Button2_Click(object sender, EventArgs e)   // Calendar
        {
            Calendar calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }

        private void guna2Button3_Click(object sender, EventArgs e)   // My Activities
        {
            My_Activities activitiesForm = new My_Activities();
            activitiesForm.Show();
            this.Hide();
        }

        private void guna2Button4_Click(object sender, EventArgs e)   // Stats
        {
            Stats statsForm = new Stats();
            statsForm.Show();
            this.Hide();
        }

        // ---------- EVENTS NA HINAHANAP NG DESIGNER (huwag burahin) ----------
        private void dtpStart_ValueChanged(object sender, EventArgs e)
        {
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void guna2ShadowPanel5_Paint(object sender, PaintEventArgs e)
        {
        }

        private void monthCalendar1_DateChanged(object sender, DateRangeEventArgs e)
        {
        }

        private void monthCalendar1_DateChanged_1(object sender, DateRangeEventArgs e)
        {
        }

        private void guna2HtmlLabel4_Click(object sender, EventArgs e)
        {
        }

        private void guna2ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint_1(object sender, PaintEventArgs e)
        {
            tlpCalendar.Dock = DockStyle.Fill;
            tlpCalendar.ColumnCount = 7;
            tlpCalendar.RowCount = 7;
            tlpCalendar.ColumnStyles.Clear();
            tlpCalendar.RowStyles.Clear();

            // 7 equal columns
            for (int c = 0; c < 7; c++)
                tlpCalendar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / 7));

            // header row (day names) fixed height, 6 equal week rows
            tlpCalendar.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));
            for (int r = 0; r < 6; r++)
                tlpCalendar.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / 6));
        }
    }
}