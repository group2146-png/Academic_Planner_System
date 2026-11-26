
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;



namespace Academic__Planner
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            CenterLoginPanel();
            guna2TextBox2.UseSystemPasswordChar = true;
        }

        private void CenterLoginPanel()
        {
            guna2ShadowPanel1.Left =
                (this.ClientSize.Width - guna2ShadowPanel1.Width) / 2;

            guna2ShadowPanel1.Top =
                (this.ClientSize.Height - guna2ShadowPanel1.Height) / 2;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            CenterLoginPanel();
        }

        private void Login_Resize(object sender, EventArgs e)
        {
            CenterLoginPanel();
        }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text;

            if (username == "" || password == "")
            {
                MessageBox.Show("Please enter your username and password.");
                return;
            }

            try
            {
                using (var conn = new SqlConnection(Db.ConnectionString))
                {
                    conn.Open();

                    using (var cmd = new SqlCommand(
                        "SELECT UserId FROM Users WHERE Username = @u AND PasswordHash = @p", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", Db.HashPassword(password));

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            Session.UserId = (int)result;
                            Session.Username = username;
                            Db.LoadActivitiesForUser(Session.UserId);

                            new Dashboard().Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Invalid username or password.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            signup signUp = Application.OpenForms.OfType<signup>().FirstOrDefault();
            if (signUp == null)
                signUp = new signup();

            signUp.Show();
            this.Hide();

        }
    }
}