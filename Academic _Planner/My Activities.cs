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
    public partial class My_Activities : Form
    {
        const int PageSize = 4;
        int page = 0;

        Guna.UI2.WinForms.Guna2HtmlLabel[] titles, starts, diffs, ends, statuses;
        Guna.UI2.WinForms.Guna2Button[] completeBtns;

        public My_Activities()
        {
            InitializeComponent();

            titles = new[] { guna2HtmlLabel2, guna2HtmlLabel3, guna2HtmlLabel4, guna2HtmlLabel5 };
            starts = new[] { guna2HtmlLabel6, guna2HtmlLabel7, guna2HtmlLabel8, guna2HtmlLabel9 };
            diffs = new[] { guna2HtmlLabel10, guna2HtmlLabel11, guna2HtmlLabel12, guna2HtmlLabel13 };
            ends = new[] { guna2HtmlLabel14, guna2HtmlLabel15, guna2HtmlLabel16, guna2HtmlLabel17 };
            statuses = new[] { guna2HtmlLabel22, guna2HtmlLabel23, guna2HtmlLabel24, guna2HtmlLabel25 };

            // Find buttons by their text, so no designer names are needed
            var allButtons = GetAllButtons(this).ToList();

            completeBtns = allButtons
                .Where(b => b.Text == "Complete")
                .OrderBy(b => b.PointToScreen(Point.Empty).Y)
                .Take(PageSize)
                .ToArray();

            for (int i = 0; i < completeBtns.Length; i++)
            {
                int row = i;
                completeBtns[i].Click += (s, e) => CompleteRow(row);
            }

            var prev = allButtons.FirstOrDefault(b => b.Text == "Previous");
            var next = allButtons.FirstOrDefault(b => b.Text == "Next");
            if (prev != null) prev.Click += (s, e) => { if (page > 0) { page--; RenderPage(); } };
            if (next != null) next.Click += (s, e) =>
            {
                if ((page + 1) * PageSize < ActivityStore.Items.Count) { page++; RenderPage(); }
            };

            for (int n = 1; n <= 3; n++)
            {
                int pageNumber = n - 1;
                var btn = allButtons.FirstOrDefault(b => b.Text == n.ToString());
                if (btn != null)
                    btn.Click += (s, e) => { page = pageNumber; RenderPage(); };
            }

            this.Load += (s, e) => RenderPage();
        }

        // Collects every Guna2Button on the form, even inside panels
        private IEnumerable<Guna.UI2.WinForms.Guna2Button> GetAllButtons(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Guna.UI2.WinForms.Guna2Button b)
                    yield return b;

                foreach (var child in GetAllButtons(c))
                    yield return child;
            }
        }

        private void RenderPage()
        {
            int rows = Math.Min(PageSize, completeBtns.Length);

            for (int i = 0; i < PageSize; i++)
            {
                int index = page * PageSize + i;
                bool exists = i < rows && index < ActivityStore.Items.Count;

                titles[i].Visible = exists;
                starts[i].Visible = exists;
                diffs[i].Visible = exists;
                ends[i].Visible = exists;
                statuses[i].Visible = exists;
                if (i < completeBtns.Length) completeBtns[i].Visible = exists;

                if (!exists) continue;

                var a = ActivityStore.Items[index];
                titles[i].Text = a.Title;
                starts[i].Text = a.Start.ToString("MMM dd, yyyy");
                diffs[i].Text = a.Difficulty;
                ends[i].Text = a.End.ToString("MMM dd, yyyy");

                if (a.IsCompleted)
                {
                    statuses[i].Text = "✔ Completed";
                    statuses[i].ForeColor = Color.SeaGreen;
                    completeBtns[i].Enabled = false;
                }
                else
                {
                    statuses[i].Text = "Pending";
                    statuses[i].ForeColor = Color.DarkOrange;
                    completeBtns[i].Enabled = true;
                }
            }
        }

        private void CompleteRow(int row)
        {
            int index = page * PageSize + row;
            if (index >= ActivityStore.Items.Count) return;

            ActivityStore.Items[index].IsCompleted = true;
            RenderPage();
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
            My_Activities activitiesForm = new My_Activities();
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