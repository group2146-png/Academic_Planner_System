namespace Academic__Planner
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Intro: pipili ng Login o Sign Up. Kapag sinara lang ang window (X), titigil ang app.
            IntroChoice choice;
            using (var intro = new Intro())
            {
                Application.Run(intro);
                choice = intro.Choice;
            }
            if (choice == IntroChoice.None)
                return;

            // 2. Sign Up (kung iyon ang pinindot). Pagkatapos, dederetso sa Login.
            if (choice == IntroChoice.Signup)
            {
                using (var signUpForm = new signup())
                {
                    Application.Run(signUpForm);
                }
            }

            // 3. Login. Kapag sinara lang ang window nang hindi nag-login, titigil ang app.
            using (var login = new Login())
            {
                Application.Run(login);
                if (!login.LoginSuccess)
                    return;
            }

            // 4. Dashboard
            Application.Run(new Dashboard());
        }
    }
}