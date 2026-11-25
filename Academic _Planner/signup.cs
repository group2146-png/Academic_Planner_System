
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
    public partial class signup : Form
    {
        public signup()
        {
            InitializeComponent();
            guna2TextBox2.UseSystemPasswordChar = true;
            guna2TextBox3.UseSystemPasswordChar = true;
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login == null)
                login = new Login();

            login.Show();
            this.Hide();
        }

        private void signup_Load(object sender, EventArgs e)
        {

        }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text;
            string confirm = guna2TextBox3.Text;

            if (username == "" || password == "" || confirm == "")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            try
            {
                using (var conn = new SqlConnection(Db.ConnectionString))
                {
                    conn.Open();

                    // Check kung existing na ang username
                    using (var check = new SqlCommand(
                        "SELECT COUNT(*) FROM Users WHERE Username = @u", conn))
                    {
                        check.Parameters.AddWithValue("@u", username);
                        if ((int)check.ExecuteScalar() > 0)
                        {
                            MessageBox.Show("Username already exists.");
                            return;
                        }
                    }

                    // I-save ang bagong user
                    using (var cmd = new SqlCommand(
                        "INSERT INTO Users (Username, PasswordHash) VALUES (@u, @p)", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", username);
                        cmd.Parameters.AddWithValue("@p", Db.HashPassword(password));
                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Account created! You can now log in.");

                new Login().Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}