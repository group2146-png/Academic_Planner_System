using System;
using System.Collections.Generic;
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
        List<Activity> items = new();

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
            hiddenBtns = new[] { guna2Button15, guna2Button16, guna2Button17, guna2Button18 };   // sobra, itinatago
            pageBtns   = new[] { guna2Button3, guna2Button4, guna2Button5 };

            for (int i = 0; i < PageSize; i++)
            {
                int row = i;
                reopenBtns[i].Text = "Reopen";
                deleteBtns[i].Text = "Delete";
                deleteBtns[i].FillColor = ActivityUi.DeleteColor;
                hiddenBtns[i].Visible = false;

                reopenBtns[i].Click += (s, e) => Reopen(row);
                deleteBtns[i].Click += (s, e) => DeleteActivity(row);
            }

            foreach (var b in pageBtns)
                b.Click += (s, e) =>
                {
                    if (((Guna2Button)s!).Tag is int p) { page = p; ShowPage(); }
                };

            guna2Button1.Click += (s, e) => { if (page > 0) { page--; ShowPage(); } };               // Previous
            guna2Button2.Click += (s, e) => { if (page < TotalPages - 1) { page++; ShowPage(); } };  // Next

            Load += (s, e) => LoadActivities();
        }

        int TotalPages => Math.Max(1, (int)Math.Ceiling(items.Count / (double)PageSize));

        private void LoadActivities()
        {
            try
            {
                items = ActivityRepo.GetByStatus(ActivityRepo.Completed);
            }
            catch (Exception ex)
            {
                items = new List<Activity>();
                ActivityUi.ShowError(ex);
            }
            ShowPage();
        }

        private void ShowPage()
        {
            if (page > TotalPages - 1) page = TotalPages - 1;

            for (int i = 0; i < PageSize; i++)
            {
                int index = page * PageSize + i;
                bool has = index < items.Count;

                titles[i].Visible = subjects[i].Visible = priorities[i].Visible =
                    deadlines[i].Visible = statuses[i].Visible = has;
                reopenBtns[i].Visible = deleteBtns[i].Visible = has;

                if (has)
                    ActivityUi.ShowRow(items[index], titles[i], subjects[i],
                                       priorities[i], deadlines[i], statuses[i]);
            }

            ActivityUi.UpdatePager(page, TotalPages, guna2Button1, guna2Button2, pageBtns);
        }

        private Activity? RowItem(int row)
        {
            int index = page * PageSize + row;
            return index < items.Count ? items[index] : null;
        }

        // ---------- REOPEN (ibalik sa My Activities bilang Pending) ----------
        private void Reopen(int row)
        {
            var item = RowItem(row);
            if (item == null) return;

            try
            {
                ActivityRepo.SetStatus(item.Id, ActivityRepo.Pending);
                LoadActivities();
            }
            catch (Exception ex) { ActivityUi.ShowError(ex); }
        }

        // ---------- DELETE ----------
        private void DeleteActivity(int row)
        {
            var item = RowItem(row);
            if (item == null) return;

            var answer = MessageBox.Show($"Burahin ang \"{item.Title}\"?", "Delete activity",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            try
            {
                ActivityRepo.Delete(item.Id);
                LoadActivities();
            }
            catch (Exception ex) { ActivityUi.ShowError(ex); }
        }
    }
}
