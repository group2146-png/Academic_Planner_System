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
    }
}