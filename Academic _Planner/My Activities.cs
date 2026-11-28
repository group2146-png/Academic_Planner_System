using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Academic__Planner
{
    public partial class My_Activities : Form
    {
        const int PageSize = 4;     // 4 rows kada page (4 ang row sa design)
        int _page = 0;
        List<Activity> _items = new();

        // Bawat array = 1 column, 4 rows
        Guna2HtmlLabel[] _titles, _subjects, _priorities, _deadlines, _statuses;
        Guna2Button[] _doneBtns, _editBtns, _deleteBtns, _pageBtns;

        public My_Activities()
        {
            InitializeComponent();

            _titles     = new[] { guna2HtmlLabel2,  guna2HtmlLabel3,  guna2HtmlLabel4,  guna2HtmlLabel5 };
            _subjects   = new[] { guna2HtmlLabel6,  guna2HtmlLabel7,  guna2HtmlLabel8,  guna2HtmlLabel9 };
            _priorities = new[] { guna2HtmlLabel10, guna2HtmlLabel11, guna2HtmlLabel12, guna2HtmlLabel13 };
            _deadlines  = new[] { guna2HtmlLabel14, guna2HtmlLabel15, guna2HtmlLabel16, guna2HtmlLabel17 };
            _statuses   = new[] { guna2HtmlLabel22, guna2HtmlLabel23, guna2HtmlLabel24, guna2HtmlLabel25 };
            _doneBtns   = new[] { guna2Button6,  guna2Button7,  guna2Button8,  guna2Button9 };
            _editBtns   = new[] { guna2Button10, guna2Button11, guna2Button12, guna2Button13 };
            _deleteBtns = new[] { guna2Button14, guna2Button15, guna2Button16, guna2Button17 };
            _pageBtns   = new[] { btnPage1, btnPage2, btnPage3 };

            for (int i = 0; i < PageSize; i++)
            {
                int row = i;   // kailangan ang kopya para tama ang row sa bawat click
                _doneBtns[i].Text = "Done";
                _editBtns[i].Text = "Edit";
                _deleteBtns[i].Text = "Delete";
                _deleteBtns[i].FillColor = ActivityUi.DeleteColor;

                _doneBtns[i].Click   += (s, e) => MarkDone(row);
                _editBtns[i].Click   += (s, e) => EditActivity(row);
                _deleteBtns[i].Click += (s, e) => DeleteActivity(row);
            }

            foreach (var b in _pageBtns)
                b.Click += (s, e) =>
                {
                    if (((Guna2Button)s!).Tag is int p) { _page = p; ShowPage(); }
                };

            btnPrev.Click     += (s, e) => { if (_page > 0) { _page--; ShowPage(); } };
            btnNextPage.Click += (s, e) => { if (_page < TotalPages - 1) { _page++; ShowPage(); } };
            btnAdd.Click      += btnAdd_Click;

            Load += (s, e) => LoadActivities();
        }

        int TotalPages => Math.Max(1, (int)Math.Ceiling(_items.Count / (double)PageSize));

        // ---------- KUMUHA NG DATA ----------
        private void LoadActivities()
        {
            try
            {
                _items = ActivityRepo.GetByStatus(ActivityRepo.Pending);
            }
            catch (Exception ex)
            {
                _items = new List<Activity>();
                ActivityUi.ShowError(ex);
            }
            ShowPage();
        }

        // ---------- IPAKITA ANG CURRENT PAGE ----------
        private void ShowPage()
        {
            if (_page > TotalPages - 1) _page = TotalPages - 1;   // kung nabawasan ang pages (hal. nag-delete)

            for (int i = 0; i < PageSize; i++)
            {
                int index = _page * PageSize + i;
                bool has = index < _items.Count;

                _titles[i].Visible = _subjects[i].Visible = _priorities[i].Visible =
                    _deadlines[i].Visible = _statuses[i].Visible = has;
                _doneBtns[i].Visible = _editBtns[i].Visible = _deleteBtns[i].Visible = has;

                if (has)
                    ActivityUi.ShowRow(_items[index], _titles[i], _subjects[i],
                                       _priorities[i], _deadlines[i], _statuses[i]);
            }

            ActivityUi.UpdatePager(_page, TotalPages, btnPrev, btnNextPage, _pageBtns);
        }

        private Activity? RowItem(int row)
        {
            int index = _page * PageSize + row;
            return index < _items.Count ? _items[index] : null;
        }

        // ---------- ADD ----------
        private void btnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new Form1();
            if (Overlay.ShowOver(this, dlg) == DialogResult.OK)
                LoadActivities();
        }

        // ---------- EDIT ----------
        private void EditActivity(int row)
        {
            var item = RowItem(row);
            if (item == null) return;

            using var dlg = new Form1(item);
            if (Overlay.ShowOver(this, dlg) == DialogResult.OK)
                LoadActivities();
        }

        // ---------- DONE (ilipat sa Completed) ----------
        private void MarkDone(int row)
        {
            var item = RowItem(row);
            if (item == null) return;

            try
            {
                ActivityRepo.SetStatus(item.Id, ActivityRepo.Completed);
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

        // ---------- BACK ----------
        private void button1_Click(object? sender, EventArgs e)
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault() ?? new Dashboard();
            dashboard.Show();
            this.Close();
        }
    }
}
