namespace Academic__Planner
{
    /// <summary>Ano ang pinindot ng user sa Intro.</summary>
    internal enum IntroChoice { None, Login, Signup }

    public partial class Intro : Form
    {
        /// <summary>Chine-check ito ng Program.cs pagkasara ng Intro.</summary>
        internal IntroChoice Choice { get; private set; } = IntroChoice.None;

        public Intro()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void guna2Button1_Click(object sender, EventArgs e)   // Login
        {
            Choice = IntroChoice.Login;
            Close();
        }

        private void guna2Button2_Click(object sender, EventArgs e)   // Signup
        {
            Choice = IntroChoice.Signup;
            Close();
        }
    }
}