using System.Drawing;
using System.Net;

namespace Academic__Planner
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();

            // "View All ->" at ang Completed button
            guna2HtmlLabel7.Cursor = Cursors.Hand;
            guna2HtmlLabel7.Click += (s, e) => GoTo(new My_Activities());

            // pag bumalik ka sa Dashboard (galing My Activities / Completed), mag-update ang numbers
            VisibleChanged += (s, e) => { if (Visible && IsHandleCreated) RefreshStats(); };
        }

        private void Dashboard_Load(object? sender, EventArgs e)
        {
            try
            {
                if (Session.UserId != 0)
                    Db.LoadActivitiesForUser(Session.UserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading activities: " + ex.Message);
            }

            RefreshStats();
        }

        // Public para matawag ng My Activities at Completed pagkatapos ng add / done / delete
        public void RefreshStats()
        {
            var all = ActivityStore.Items;
            var pending = ActivityStore.Pending();
            DateTime today = DateTime.Today;

            int total = all.Count;
            int completed = total - pending.Count;
            int dueSoon = pending.Count(a => a.End.Date >= today && a.End.Date <= today.AddDays(3));
            int overdue = pending.Count(a => a.End.Date < today);
            int dueToday = pending.Count(a => a.End.Date == today);

            lblTotalCount.Text = total.ToString();
            lblCompletedCount.Text = completed.ToString();
            lblDueSoonCount.Text = dueSoon.ToString();
            lblOverdueCount.Text = overdue.ToString();

            guna2HtmlLabel17.Text = $"You have {dueToday} {(dueToday == 1 ? "activity" : "activities")} to complete today";

            // Up-Coming Activities (susunod na 5 na hindi pa tapos)
            if (pending.Count == 0)
            {
                guna2HtmlLabel9.Text = "No Upcoming events yet!";
                guna2HtmlLabel9.ForeColor = Color.FromArgb(158, 170, 162);
                guna2HtmlLabel9.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                guna2HtmlLabel9.Location = new Point(290, 196);
            }
            else
            {
                var lines = pending.Take(5).Select(a =>
                    $"• <b>{WebUtility.HtmlEncode(a.Title)}</b>" +
                    (a.Subject != "" ? $" &nbsp;({WebUtility.HtmlEncode(a.Subject)})" : "") +
                    $" &nbsp;-&nbsp; due {a.End:MMM dd, yyyy}" +
                    (a.End.Date < today ? " <font color='#DC143C'>(overdue)</font>" : ""));

                guna2HtmlLabel9.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
                guna2HtmlLabel9.ForeColor = Color.FromArgb(40, 60, 50);
                guna2HtmlLabel9.Location = new Point(24, 95);
                guna2HtmlLabel9.Text = string.Join("<br>", lines);
            }
        }

        private void GoTo(Form form)
        {
            form.Show();
            this.Hide();
        }

        private void Logout(bool closeThis)
        {
            var confirm = MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            Login loginForm = Application.OpenForms.OfType<Login>().FirstOrDefault() ?? new Login();
            loginForm.Show();

            if (closeThis) this.Close();
            else this.Hide();
        }

        // ---------- SIDEBAR ----------
        private void guna2Button2_Click(object? sender, EventArgs e) => GoTo(new Calendar());       // Calendar
        private void guna2Button3_Click(object? sender, EventArgs e) => GoTo(new My_Activities());  // My Activities
        private void btnCompleted_Click(object? sender, EventArgs e) => GoTo(new Completed());      // Completed
        private void guna2Button5_Click(object? sender, EventArgs e) => GoTo(new Settings());       // Settings
        private void guna2Button6_Click_1(object? sender, EventArgs e) => GoTo(new About());        // About
        private void guna2Button7_Click(object? sender, EventArgs e) => Logout(true);               // Logout
        private void guna2ImageButton2_Click(object? sender, EventArgs e) => GoTo(new Profile());   // Profile
    }
}
