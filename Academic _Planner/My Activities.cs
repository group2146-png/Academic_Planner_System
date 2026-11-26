using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Academic__Planner
{
    public partial class My_Activities : Form
    {
        const int PageSize = 4;
        int page = 0;

        // Bawat hanay ay may 4 na rows (taas pababa)
        Guna2HtmlLabel[] titles, subjects, priorities, deadlines, statuses;
        Guna2Button[] doneBtns, editBtns, deleteBtns, pageBtns;

        public My_Activities()
        {
            InitializeComponent();

            titles     = new[] { guna2HtmlLabel2,  guna2HtmlLabel3,  guna2HtmlLabel4,  guna2HtmlLabel5 };
            subjects   = new[] { guna2HtmlLabel6,  guna2HtmlLabel7,  guna2HtmlLabel8,  guna2HtmlLabel9 };
            priorities = new[] { guna2HtmlLabel10, guna2HtmlLabel11, guna2HtmlLabel12, guna2HtmlLabel13 };
            deadlines  = new[] { guna2HtmlLabel14, guna2HtmlLabel15, guna2HtmlLabel16, guna2HtmlLabel17 };
            statuses   = new[] { guna2HtmlLabel22, guna2HtmlLabel23, guna2HtmlLabel24, guna2HtmlLabel25 };

            doneBtns   = new[] { guna2Button6,  guna2Button7,  guna2Button8,  guna2Button9 };
            editBtns   = new[] { guna2Button10, guna2Button11, guna2Button12, guna2Button13 };
            deleteBtns = new[] { guna2Button14, guna2Button15, guna2Button16, guna2Button17 };
            pageBtns   = new[] { btnPage1, btnPage2, btnPage3 };

            for (int i = 0; i < PageSize; i++)
            {
                int row = i;
                doneBtns[i].Click   += (s, e) => DoneRow(row);
                editBtns[i].Click   += (s, e) => EditRow(row);
                deleteBtns[i].Click += (s, e) => DeleteRow(row);
                editBtns[i].Text = "Edit";
                deleteBtns[i].Text = "Delete";
            }

            for (int i = 0; i < pageBtns.Length; i++)
            {
                int slot = i;
                pageBtns[i].Click += (s, e) => { page = (page / 3) * 3 + slot; RenderPage(); };
            }

            btnPrev.Click += (s, e) => { if (page > 0) { page--; RenderPage(); } };
            btnNextPage.Click += (s, e) => { if (page < TotalPages() - 1) { page++; RenderPage(); } };
            btnAdd.Click += (s, e) => AddActivity();

            Load += (s, e) =>
            {
                try { Db.LoadActivitiesForUser(Session.UserId); }
                catch (Exception ex) { MessageBox.Show("Error loading activities: " + ex.Message); }
                RenderPage();
            };
        }

        private int TotalPages() => Math.Max(1, (int)Math.Ceiling(ActivityStore.Pending().Count / (double)PageSize));

        private Activity? ActivityAt(int row)
        {
            var list = ActivityStore.Pending();
            int index = page * PageSize + row;
            return index < list.Count ? list[index] : null;
        }

        // Pinapa-refresh ang Dashboard (kahit nakatago) pagkatapos ng bawat pagbabago
        private void RefreshDashboard()
        {
            Application.OpenForms.OfType<Dashboard>().FirstOrDefault()?.RefreshStats();
        }

        private void AfterChange()
        {
            RenderPage();
            RefreshDashboard();
        }

        // ---------- ADD (overlay) ----------
        private void AddActivity()
        {
            using (var form = new Form1())
            {
                if (Overlay.ShowOver(this, form) == DialogResult.OK)
                {
                    // pumunta sa page kung nasaan ang bagong activity
                    var list = ActivityStore.Pending();
                    var newest = ActivityStore.Items.OrderByDescending(a => a.Id).FirstOrDefault();
                    int idx = newest == null ? -1 : list.FindIndex(a => a.Id == newest.Id);
                    page = idx >= 0 ? idx / PageSize : 0;
                    AfterChange();
                }
            }
        }

        // ---------- EDIT (overlay) ----------
        private void EditRow(int row)
        {
            var a = ActivityAt(row);
            if (a == null) return;

            using (var form = new Form1(a))
            {
                if (Overlay.ShowOver(this, form) == DialogResult.OK)
                    AfterChange();
            }
        }

        // ---------- DONE ----------
        private void DoneRow(int row)
        {
            var a = ActivityAt(row);
            if (a == null) return;

            try
            {
                Db.SetDone(a.Id, true);
                if (page >= TotalPages()) page = TotalPages() - 1;
                AfterChange();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // ---------- DELETE ----------
        private void DeleteRow(int row)
        {
            var a = ActivityAt(row);
            if (a == null) return;

            if (MessageBox.Show($"Delete \"{a.Title}\"?", "Delete activity",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                Db.DeleteActivity(a.Id);
                if (page >= TotalPages()) page = TotalPages() - 1;
                AfterChange();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        // ---------- RENDER ----------
        private void RenderPage()
        {
            int pages = TotalPages();
            if (page >= pages) page = pages - 1;
            if (page < 0) page = 0;

            var list = ActivityStore.Pending();

            for (int i = 0; i < PageSize; i++)
            {
                int index = page * PageSize + i;
                bool exists = index < list.Count;

                titles[i].Visible = subjects[i].Visible = priorities[i].Visible =
                    deadlines[i].Visible = statuses[i].Visible = exists;
                doneBtns[i].Visible = editBtns[i].Visible = deleteBtns[i].Visible = exists;

                if (!exists) continue;

                var a = list[index];
                titles[i].Text = a.Title;
                subjects[i].Text = a.Subject == "" ? "—" : a.Subject;
                priorities[i].Text = a.Difficulty;
                deadlines[i].Text = a.End.ToString("MMM dd, yyyy");

                bool overdue = a.End.Date < DateTime.Today;
                statuses[i].Text = overdue ? "Overdue" : "Pending";
                statuses[i].ForeColor = overdue ? Color.Crimson : Color.DarkOrange;
            }

            // page buttons: 1 2 3 / 4 5 6 / ...
            int first = (page / 3) * 3;
            for (int i = 0; i < pageBtns.Length; i++)
            {
                int number = first + i;
                pageBtns[i].Visible = number < pages;
                pageBtns[i].Text = (number + 1).ToString();
                pageBtns[i].FillColor = number == page ? Color.FromArgb(20, 90, 60) : Color.FromArgb(46, 125, 82);
            }
            btnPrev.Enabled = page > 0;
            btnNextPage.Enabled = page < pages - 1;
        }

        // ---------- BACK ----------
        private void button1_Click(object? sender, EventArgs e)
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault() ?? new Dashboard();
            dashboard.Show();
            this.Close();
        }
    }
}
