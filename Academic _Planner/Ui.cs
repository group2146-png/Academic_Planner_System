using Guna.UI2.WinForms;

namespace Academic__Planner
{
    // Maliliit na gamit para gumawa ng UI sa code (Profile, Settings, Calendar)
    public static class Ui
    {
        public static readonly Color Green = Color.FromArgb(46, 125, 82);
        public static readonly Color Soft = Color.FromArgb(232, 242, 235);
        public static readonly Color Ink = Color.FromArgb(40, 60, 50);
        public static readonly Color Gray = Color.FromArgb(110, 125, 116);
        public static readonly Color Red = Color.FromArgb(200, 55, 70);

        public static void BackToDashboard(Form f)
        {
            Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault() ?? new Dashboard();
            dashboard.Show();
            f.Close();
        }

        // berdeng bar sa taas na may Back button at pamagat
        public static void Header(Form f, string title, Action? onBack = null)
        {
            var bar = new Guna2Panel
            {
                FillColor = Green,
                Bounds = new Rectangle(0, 0, f.ClientSize.Width, 72),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Tag = "notheme"
            };

            var back = new Guna2Button
            {
                Text = "Back",
                BorderRadius = 8,
                FillColor = Color.White,
                ForeColor = Green,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Bounds = new Rectangle(30, 19, 90, 34),
                Tag = "notheme"
            };
            back.Click += (s, e) => { if (onBack != null) onBack(); else BackToDashboard(f); };

            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(140, 16),
                Tag = "notheme"
            };

            bar.Controls.Add(back);
            bar.Controls.Add(lbl);
            f.Controls.Add(bar);
        }

        public static Guna2Panel Card(Control parent, Rectangle bounds)
        {
            var card = new Guna2Panel { FillColor = Color.White, BorderRadius = 14, Bounds = bounds };
            parent.Controls.Add(card);
            return card;
        }

        public static Label Title(Control parent, string text, int x, int y, float size = 14F)
        {
            var l = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, FontStyle.Bold),
                ForeColor = Ink,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(x, y)
            };
            parent.Controls.Add(l);
            return l;
        }

        public static Label Text(Control parent, string text, int x, int y, float size = 10F, bool muted = false, bool bold = false)
        {
            var l = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", size, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = muted ? Gray : Ink,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(x, y)
            };
            parent.Controls.Add(l);
            return l;
        }

        public static Guna2Button Button(Control parent, string text, Rectangle bounds, bool primary = true, bool danger = false)
        {
            var b = new Guna2Button
            {
                Text = text,
                BorderRadius = 10,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FillColor = danger ? Red : primary ? Green : Soft,
                ForeColor = (danger || primary) ? Color.White : Ink,
                Bounds = bounds
            };
            parent.Controls.Add(b);
            return b;
        }

        public static Guna2TextBox Box(Control parent, string placeholder, Rectangle bounds, bool password = false)
        {
            var t = new Guna2TextBox
            {
                PlaceholderText = placeholder,
                BorderRadius = 8,
                Font = new Font("Segoe UI", 10.5F),
                Bounds = bounds
            };
            if (password) t.UseSystemPasswordChar = true;
            parent.Controls.Add(t);
            return t;
        }
    }
}
