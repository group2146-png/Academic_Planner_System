namespace Academic__Planner
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Login muna. Kapag sinara lang ang window nang hindi nag-login, titigil ang app.
            Application.Run(new Intro ());

            using (var login = new Login())
            {
                Application.Run(login);
                if (!login.LoginSuccess)
                    return;
            }

            Application.Run(new Dashboard ());
        }
    }
}
