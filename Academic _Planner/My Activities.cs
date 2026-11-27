using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Academic__Planner
{
    public partial class My_Activities : Form
    {
        public My_Activities()
        {
            InitializeComponent();
            
        }

        

        private void button1_Click(object? sender, EventArgs e)
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault() ?? new Dashboard();
            dashboard.Show();
            this.Close();
        }
    }
}
