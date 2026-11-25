using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Academic__Planner
{
    public partial class Profile : Form
    {
        public Profile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault();
            if (dashboard == null)
                dashboard = new Dashboard();

            dashboard.Show();
            this.Close();
        }
    }
}
