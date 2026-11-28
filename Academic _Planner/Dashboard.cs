using System.Drawing;
using System.Net;

namespace Academic__Planner
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            btnDashboard.Click += (s, e) => { };   // nasa Dashboard na, walang gagawin

        }

        private void Dashboard_Load(object? sender, EventArgs e)
        {

        }



        private void GoTo(Form form)
        {
            form.FormClosed += (s, e) =>
            {
                if (!IsDisposed) Show();
            };
            Hide();
            form.Show();
        }

        private void Logout(bool closeThis)
        {
            var answer = MessageBox.Show("Sigurado ka bang gusto mong mag-logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            Session.CurrentUser = null;
            Application.Restart();   // babalik sa Login screen
        }

        private void guna2Button2_Click(object? sender, EventArgs e) => GoTo(new CalendarForm());       // Calendar
        private void guna2Button3_Click(object? sender, EventArgs e) => GoTo(new My_Activities());  // My Activities
        private void btnCompleted_Click(object? sender, EventArgs e) => GoTo(new Completed());      // Completed
        private void guna2Button5_Click(object? sender, EventArgs e) => GoTo(new Settings());       // Settings
        private void guna2Button6_Click_1(object? sender, EventArgs e) => GoTo(new About());        // About
        private void guna2Button7_Click(object? sender, EventArgs e) => Logout(true);               // Logout
        private void guna2ImageButton2_Click(object? sender, EventArgs e) => GoTo(new Profile());   // Profile

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            CalendarForm calendar = new CalendarForm();
            calendar.ShowDialog();
        }
    }
}
