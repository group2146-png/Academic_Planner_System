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

            cmbMonth.AutoSize = false;
            cmbMonth.Height = 50;
            cmbMonth.Width = 300;
            cmbMonth.Font = new Font("Segoe UI", 14F);
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
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(guna2TextBox1.Text) || guna2ComboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please enter a title and choose a difficulty.");
                return;
            }

            if (guna2DateTimePicker2.Value < guna2DateTimePicker1.Value)
            {
                MessageBox.Show("End date can't be before the start date.");
                return;
            }

            if (Session.UserId == 0)
            {
                MessageBox.Show("No user is logged in. Please log in again.");
                return;
            }

            string title = guna2TextBox1.Text.Trim();
            string difficulty = guna2ComboBox1.SelectedItem.ToString();
            DateTime start = guna2DateTimePicker1.Value;
            DateTime end = guna2DateTimePicker2.Value;

            try
            {
                // 1. I-save sa database
                using (var conn = new SqlConnection(Db.ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(
                        @"INSERT INTO Activities (UserId, Title, Difficulty, StartDate, EndDate)
                          VALUES (@uid, @title, @diff, @start, @end)", conn))
                    {
                        cmd.Parameters.AddWithValue("@uid", Session.UserId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@diff", difficulty);
                        cmd.Parameters.AddWithValue("@start", start);
                        cmd.Parameters.AddWithValue("@end", end);
                        cmd.ExecuteNonQuery();
                    }
                }

                // 2. I-add din sa ActivityStore para makita agad sa ibang forms
                ActivityStore.Items.Add(new Activity
                {
                    Title = title,
                    Difficulty = difficulty,
                    Start = start,
                    End = end
                });

                guna2TextBox1.Clear();
                guna2ComboBox1.SelectedIndex = -1;
                MessageBox.Show("Activity saved!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

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
    }
}