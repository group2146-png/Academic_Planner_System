using MySqlConnector;

namespace Academic__Planner
{
    public partial class Login : Form
    {
        /// <summary>Nagiging true kapag tama ang login. Chine-check ito ng Program.cs.</summary>
        public bool LoginSuccess { get; private set; }

        public Login()
        {
            InitializeComponent();

            // guna2TextBox1 = Username, guna2TextBox2 = Password
            guna2TextBox2.UseSystemPasswordChar = true;   // tinatago ang tina-type na password
            AcceptButton = guna2Button1;                  // Enter key = Login
        }

        private void Login_Load(object sender, EventArgs e) { }

        private void Login_Resize(object sender, EventArgs e) { }

        private void guna2ShadowPanel1_Paint(object sender, PaintEventArgs e) { }

        // ---------- LOGIN BUTTON ----------
        private async void guna2Button1_Click(object sender, EventArgs e)
        {
            string username = guna2TextBox1.Text.Trim();
            string password = guna2TextBox2.Text;

            if (username.Length == 0 || password.Length == 0)
            {
                MessageBox.Show("Pakilagay ang username at password.", "Login",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            guna2Button1.Enabled = false;
            try
            {
                UserInfo? user = await Db.AuthenticateAsync(username, password);

                if (user == null)
                {
                    MessageBox.Show("Mali ang username o password.", "Login failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    guna2TextBox2.Clear();
                    guna2TextBox2.Focus();
                    return;
                }

                Session.CurrentUser = user;
                LoginSuccess = true;
                Close();   // magsasara ang Login, tapos bubuksan ng Program.cs ang Dashboard
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Hindi makakonekta sa database.\n\n" +
                    "I-check kung naka-Start All ang Laragon at kung na-run na ang database.sql.\n\n" +
                    "Detalye: " + ex.Message,
                    "Database error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                guna2Button1.Enabled = true;
            }
        }

        // ---------- SIGN UP BUTTON ----------
        private void guna2Button2_Click(object sender, EventArgs e)
        {
            Hide();
            using (var form = new signup())
            {
                form.ShowDialog();
            }
            Show();
        }
    }
}
