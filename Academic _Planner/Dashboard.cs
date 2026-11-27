using System.Drawing;
using System.Net;

namespace Academic__Planner
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();

        }

        private void Dashboard_Load(object? sender, EventArgs e)
        {
            
        }

        

        private void GoTo(Form form)
        {
            
        }

        private void Logout(bool closeThis)
        {
            
        }

        private void guna2Button2_Click(object? sender, EventArgs e) => GoTo(new Calendar());       // Calendar
        private void guna2Button3_Click(object? sender, EventArgs e) => GoTo(new My_Activities());  // My Activities
        private void btnCompleted_Click(object? sender, EventArgs e) => GoTo(new Completed());      // Completed
        private void guna2Button5_Click(object? sender, EventArgs e) => GoTo(new Settings());       // Settings
        private void guna2Button6_Click_1(object? sender, EventArgs e) => GoTo(new About());        // About
        private void guna2Button7_Click(object? sender, EventArgs e) => Logout(true);               // Logout
        private void guna2ImageButton2_Click(object? sender, EventArgs e) => GoTo(new Profile());   // Profile
    }
}
