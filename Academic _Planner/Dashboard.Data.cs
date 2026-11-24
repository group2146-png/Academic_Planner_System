using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Academic__Planner
{
    // Second half of your existing Dashboard form (partial class)
    public partial class Dashboard
    {
        const int MaxRows = 4;
        readonly List<Control> dynamicRows = new List<Control>();

        // Runs every time the dashboard is shown, so it is always up to date
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (Visible) RefreshDashboard();
        }

        void RefreshDashboard()
        {
            // 1) Upcoming schedule: not completed and not already past, soonest first
            var upcoming = ActivityStore.Items
                .Where(a => !a.IsCompleted && a.End.Date >= DateTime.Today)
                .OrderBy(a => a.Start)
                .Take(MaxRows)
                .Select(a => (a.Title, a.Start.ToString("MMM dd"), Color.FromArgb(46, 125, 83)))
                .ToList();
            ShowRows("No Upcoming", upcoming);

            // 2) Activities: most recent first, with status
            var recent = ActivityStore.Items
                .AsEnumerable().Reverse()
                .Take(MaxRows)
                .Select(a => (a.Title,
                              a.IsCompleted ? "✔ Completed" : "Pending",
                              a.IsCompleted ? Color.SeaGreen : Color.DarkOrange))
                .ToList();
            ShowRows("No Activities", recent);

            // 3) Progress circle
            int total = ActivityStore.Items.Count;
            int done = ActivityStore.Items.Count(a => a.IsCompleted);
            int percent = total == 0 ? 0 : (int)Math.Round(done * 100.0 / total);

            var bar = AllControls(this)
                .OfType<Guna.UI2.WinForms.Guna2CircleProgressBar>()
                .FirstOrDefault();
            if (bar != null) bar.Value = percent;

            var percentLabel = AllControls(this).FirstOrDefault(c =>
                !(c is Guna.UI2.WinForms.Guna2CircleProgressBar) &&
                c.Text != null && c.Text.Trim().EndsWith("%"));
            if (percentLabel != null) percentLabel.Text = percent + "%";
            else if (bar != null) bar.Text = percent + "%";
        }

        // Replaces the "No ..." placeholder with up to 4 rows inside the same card
        void ShowRows(string placeholderText, List<(string Left, string Right, Color RightColor)> rows)
        {
            var empty = AllControls(this).FirstOrDefault(c =>
                c.Text != null && c.Text.Contains(placeholderText));
            if (empty == null) return;

            Control host = empty.Parent;

            // remove rows added by the previous refresh
            foreach (var old in dynamicRows.Where(r => r.Parent == host).ToList())
            {
                host.Controls.Remove(old);
                old.Dispose();
                dynamicRows.Remove(old);
            }

            var pic = host.Controls.OfType<PictureBox>().FirstOrDefault();
            bool isEmpty = rows.Count == 0;
            empty.Visible = isEmpty;
            if (pic != null) pic.Visible = isEmpty;
            if (isEmpty) return;

            int y = pic != null ? pic.Top : Math.Max(10, empty.Top - 100);
            int left = 20;
            int width = host.Width - 40;

            foreach (var r in rows)
            {
                var title = new Label
                {
                    Text = r.Left,
                    Font = new Font("Segoe UI Semibold", 11f),
                    ForeColor = Color.FromArgb(40, 40, 40),
                    BackColor = Color.Transparent,
                    AutoEllipsis = true,
                    Left = left,
                    Top = y,
                    Width = width - 120,
                    Height = 26
                };
                var info = new Label
                {
                    Text = r.Right,
                    Font = new Font("Segoe UI", 10f),
                    ForeColor = r.RightColor,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.TopRight,
                    Left = left + width - 120,
                    Top = y,
                    Width = 120,
                    Height = 26
                };

                host.Controls.Add(title);
                host.Controls.Add(info);
                title.BringToFront();
                info.BringToFront();
                dynamicRows.Add(title);
                dynamicRows.Add(info);

                y += 30;
            }
        }

        // Every control on the form, including those inside panels
        IEnumerable<Control> AllControls(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                yield return c;
                foreach (var child in AllControls(c))
                    yield return child;
            }
        }
    }
}