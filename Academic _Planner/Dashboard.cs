
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Academic__Planner
{
    public partial class Dashboard : Form
    {
        private Form? activeForm = null;

        public Dashboard()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;

        }

        // Opens another form inside the dashboard content panel
        private void OpenChildForm(Form childForm)
        {
            // Close the previous form if there is one
            if (activeForm != null)
            {
                activeForm.Close();
                activeForm.Dispose();
            }

            activeForm = childForm;

            // Prepare the child form
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;

            // Display it inside the content panel
            guna2ShadowPanel5.Controls.Clear();
            guna2ShadowPanel5.Controls.Add(childForm);

            childForm.Show();
            childForm.BringToFront();
        }



        // MY ACTIVITIES BUTTON
        private void btnMyActivities_ButtonClick(object sender, EventArgs e)
        {
            OpenChildForm(new Form2());
        }

        private void guna2TextBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void guna2PictureBox2_Click(object sender, EventArgs e)
        {
        }

        private void guna2PictureBox6_Click(object sender, EventArgs e)
        {
        }

        private void guna2ShadowPanel4_Paint(object sender, PaintEventArgs e)
        {
        }

        private void guna2PictureBox2_Click_1(object sender, EventArgs e)
        {
        }

        private void guna2Button6_Click(object sender, EventArgs e)
        {
        }

        private void guna2ShadowPanel3_Paint(object sender, PaintEventArgs e)
        {
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
        }

        private void guna2PictureBox3_Click(object sender, EventArgs e)
        {
        }

        private void guna2ShadowPanel5_Paint(object sender, PaintEventArgs e)
        {




        }

        private void guna2HtmlLabel8_Click(object sender, EventArgs e)
        {

        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {


        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {

        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Calendar calendarForm = new Calendar();
            calendarForm.Show();
            this.Hide();
        }

        private void guna2Button3_Click(object sender, EventArgs e)
        {
            Form2 activitiesForm = new Form2();
            activitiesForm.Show();
            this.Hide();

        }

        private void guna2Button4_Click(object sender, EventArgs e)
        {
            Stats statsForm = new Stats();
            statsForm.Show();
            this.Hide();
        }

        private void guna2ImageButton1_Click(object sender, EventArgs e)
        {

        }
    }
}
