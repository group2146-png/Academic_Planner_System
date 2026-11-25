
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Academic__Planner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Login login = Application.OpenForms.OfType<Login>().FirstOrDefault();
            if (login == null)
                login = new Login();

            login.Show();
            this.Hide();
        }
    }
}
