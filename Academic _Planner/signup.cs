using MySqlConnector;

namespace Academic__Planner
{
    public partial class signup : Form
    {
        public signup()
        {
            InitializeComponent();

            // guna2TextBox1 = Username, guna2TextBox2 = Password, guna2TextBox3 = Confirm Password
            guna2TextBox2.UseSystemPasswordChar = true;
            guna2TextBox3.UseSystemPasswordChar = true;
            AcceptButton = guna2Button1;   // Enter key = Sign Up
        }

        private void signup_Load(object sender, EventArgs e) { }

        private void guna2TextBox3_TextChanged(object sender, EventArgs e) { }

        // ---------- SIGN UP BUTTON ----------
        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text;
            string confirm = guna2TextBox3.Text;

            if (username.Length < 3)
            {
                MessageBox.Show("Ang username ay dapat hindi bababa sa 3 characters.", "Sign Up",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox1.Focus();
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Ang password ay dapat hindi bababa sa 6 characters.", "Sign Up",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox2.Focus();
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Hindi magkapareho ang password at confirm password.", "Sign Up",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                guna2TextBox3.Clear();
                guna2TextBox3.Focus();
                return;
            }

            guna2Button1.Enabled = false;
            try
            {
                RegisterResult result = await Db.RegisterAsync(username, password);

                if (result == RegisterResult.UsernameTaken)
                {
                    MessageBox.Show("May gumagamit na ng username na iyan. Pumili ng iba.", "Sign Up",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    guna2TextBox1.Focus();
                    return;
                }

                MessageBox.Show("Nagawa na ang account! Pwede ka nang mag-login.", "Sign Up",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();   // babalik sa Intro / Login
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Hindi makakonekta sa database.\n\n" +
                    "I-check kung naka-Start All ang Laragon (dapat tumatakbo ang MySQL).\n\n" +
                    "Detalye: " + ex.Message,
                    "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (!IsDisposed) guna2Button1.Enabled = true;
            }
        }

        // ---------- "Already have an account? Login" ----------
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}