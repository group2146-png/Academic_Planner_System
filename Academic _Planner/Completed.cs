using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Academic__Planner
{
    public partial class Completed : Form
    {
        const int PageSize = 4;
        int page = 0;

        Guna2HtmlLabel[] titles, subjects, priorities, deadlines, statuses;
        Guna2Button[] reopenBtns, deleteBtns, hiddenBtns, pageBtns;

        public Completed()
        {
            InitializeComponent();

            titles     = new[] { guna2HtmlLabel2,  guna2HtmlLabel3,  guna2HtmlLabel4,  guna2HtmlLabel5 };
            subjects   = new[] { guna2HtmlLabel6,  guna2HtmlLabel7,  guna2HtmlLabel8,  guna2HtmlLabel9 };
            priorities = new[] { guna2HtmlLabel10, guna2HtmlLabel11, guna2HtmlLabel12, guna2HtmlLabel13 };
            deadlines  = new[] { guna2HtmlLabel14, guna2HtmlLabel15, guna2HtmlLabel16, guna2HtmlLabel17 };
            statuses   = new[] { guna2HtmlLabel22, guna2HtmlLabel23, guna2HtmlLabel24, guna2HtmlLabel25 };

            reopenBtns = new[] { guna2Button6,  guna2Button7,  guna2Button8,  guna2Button9 };
            deleteBtns = new[] { guna2Button11, guna2Button12, guna2Button13, guna2Button14 };
            hiddenBtns = new[] { guna2Button15, guna2Button16, guna2Button17, guna2Button18 };   // sobrang hanay, itatago
            pageBtns   = new[] { guna2Button3,  guna2Button4,  guna2Button5 };

            for (int i = 0; i < PageSize; i++)
            {
                int row = i;
                reopenBtns[i].Text = "Reopen";
                deleteBtns[i].Text = "Delete";
                hiddenBtns[i].Visible = false;
                reopenBtns[i].Click += (s, e) => ReopenRow(row);
                deleteBtns[i].Click += (s, e) => DeleteRow(row);
            }

            for (int i = 0; i < pageBtns.Length; i++)
            {
                int slot = i;
                pageBtns[i].Click += (s, e) => { page = (page / 3) * 3 + slot; RenderPage(); };
            }

            guna2Button1.Click += (s, e) => { if (page > 0) { page--; RenderPage(); } };                    // Previous
            guna2Button2.Click += (s, e) => { if (page < TotalPages() - 1) { page++; RenderPage(); } };    // Next

            AddBackButton();

            Load += (s, e) =>
            {
                try { Db.LoadActivitiesForUser(Session.UserId); }
                catch (Exception ex) { MessageBox.Show("Error loading activities: " + ex.Message); }
                RenderPage();
            };

            // kapag bumalik sa form na ito, i-update ang listahan
            VisibleChanged += (s, e) => { if (Visible && IsHandleCreated) RenderPage(); };
        }

        private void AddBackButton()
        {
            var back = new Guna2Button
            {
                Text = "Back",
                Size = new Size(90, 32),
                Location = new Point(51, 4),
                BorderRadius = 8,
                FillColor = Color.FromArgb(46, 125, 82),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9.75F, FontStyle.Bold)
            };
            back.Click += (s, e) =>
            {
                Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault() ?? new Dashboard();
                dashboard.Show();
                Close();
            };
            Controls.Add(back);
            back.BringToFront();
        }

        private int TotalPages() => Math.Max(1, (int)Math.Ceiling(ActivityStore.Done().Count / (double)PageSize));

        private Activity? ActivityAt(int row)
        {
            var list = ActivityStore.Done();
            int index = page * PageSize + row;
            return index < list.Count ? list[index] : null;
        }

        private void RefreshDashboard()
        {
            Application.OpenForms.OfType<Dashboard>().FirstOrDefault()?.RefreshStats();
        }

        private void ReopenRow(int row)
        {
            var a = ActivityAt(row);
            if (a == null) return;

            try
            {
                Db.SetDone(a.Id, false);
                RenderPage();
                RefreshDashboard();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void DeleteRow(int row)
        {
            var a = ActivityAt(row);
            if (a == null) return;

            if (MessageBox.Show($"Delete \"{a.Title}\"?", "Delete activity",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                Db.DeleteActivity(a.Id);
                RenderPage();
                RefreshDashboard();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void RenderPage()
        {
            int pages = TotalPages();
            if (page >= pages) page = pages - 1;
            if (page < 0) page = 0;

            var list = ActivityStore.Done();

            for (int i = 0; i < PageSize; i++)
            {
                int index = page * PageSize + i;
                bool exists = index < list.Count;

                titles[i].Visible = subjects[i].Visible = priorities[i].Visible =
                    deadlines[i].Visible = statuses[i].Visible = exists;
                reopenBtns[i].Visible = deleteBtns[i].Visible = exists;

                if (!exists) continue;

                var a = list[index];
                titles[i].Text = a.Title;
                subjects[i].Text = a.Subject == "" ? "—" : a.Subject;
                priorities[i].Text = a.Difficulty;
                deadlines[i].Text = a.End.ToString("MMM dd, yyyy");
                statuses[i].Text = "✔ Completed";
                statuses[i].ForeColor = Color.SeaGreen;
            }

            int first = (page / 3) * 3;
            for (int i = 0; i < pageBtns.Length; i++)
            {
                int number = first + i;
                pageBtns[i].Visible = number < pages;
                pageBtns[i].Text = (number + 1).ToString();
                pageBtns[i].FillColor = number == page ? Color.FromArgb(20, 90, 60) : Color.FromArgb(46, 125, 82);
            }
            guna2Button1.Enabled = page > 0;
            guna2Button2.Enabled = page < pages - 1;
        }
    }
}
