using System.Runtime.CompilerServices;
using Guna.UI2.WinForms;

namespace Academic__Planner
{
    // Dark / Light mode para sa buong app.
    // Gamit: Theme.Hook(this) sa constructor ng bawat form (pagkatapos ng InitializeComponent).
    // Para i-skip ang isang control: control.Tag = "notheme";
    public static class Theme
    {
        public static bool Dark { get; private set; }
        public static event Action? Changed;

        // ---------- MGA KULAY (gamitin kung gagawa ka ng sariling control sa code) ----------
        public static Color Bg      => Dark ? Color.FromArgb(20, 26, 23)   : Color.FromArgb(241, 248, 242);
        public static Color Card    => Dark ? Color.FromArgb(32, 40, 36)   : Color.White;
        public static Color CardAlt => Dark ? Color.FromArgb(42, 52, 47)   : Color.FromArgb(232, 242, 235);
        public static Color Text    => Dark ? Color.FromArgb(232, 238, 234) : Color.FromArgb(40, 60, 50);
        public static Color Muted   => Dark ? Color.FromArgb(158, 172, 164) : Color.FromArgb(110, 125, 116);
        public static Color Line    => Dark ? Color.FromArgb(66, 80, 73)   : Color.FromArgb(214, 226, 218);
        public static Color Accent  => Color.FromArgb(46, 125, 82);

        public static void Init() => Dark = AppPrefs.LoadDark();

        public static void SetDark(bool dark)
        {
            if (Dark == dark) return;
            Dark = dark;
            AppPrefs.SaveDark(dark);
            Changed?.Invoke();
        }

        // ---------- I-CONNECT ANG FORM ----------
        public static void Hook(Form f, bool exitWhenLast = true)
        {
            Action apply = () => { if (!f.IsDisposed) Apply(f); };

            f.Load += (s, e) => apply();
            f.VisibleChanged += (s, e) => { if (f.Visible) apply(); };
            Changed += apply;

            f.FormClosed += (s, e) =>
            {
                Changed -= apply;

                // Kapag sinara ang huling nakikitang form (X button), isara na ang buong app
                // para hindi maiwan na tumatakbo sa background.
                if (exitWhenLast && e.CloseReason == CloseReason.UserClosing &&
                    !Application.OpenForms.Cast<Form>().Any(x => x.Visible))
                {
                    Application.Exit();
                }
            };
        }

        // ---------- PAG-AAPPLY ----------
        sealed class Snap
        {
            public Color Back, AppBack, Fore, AppFore, Fill, AppFill, Border, AppBorder;
            public Image? Image;
            public bool UseVisual;
        }

        static readonly ConditionalWeakTable<Control, Snap> Snaps = new();

        public static void Apply(Control root)
        {
            Capture(root);   // 1st pass: kunin muna ang orihinal na kulay bago galawin
            Style(root);     // 2nd pass: i-apply
        }

        static void Capture(Control c)
        {
            Snaps.GetValue(c, k =>
            {
                var s = new Snap
                {
                    Back = k.BackColor,
                    Fore = k.ForeColor,
                    Image = (k as Form)?.BackgroundImage,
                    UseVisual = (k as Button)?.UseVisualStyleBackColor ?? false
                };
                s.AppBack = s.Back;
                s.AppFore = s.Fore;
                switch (k)
                {
                    case Guna2Panel p: s.Fill = p.FillColor; break;
                    case Guna2ShadowPanel sp: s.Fill = sp.FillColor; break;
                    case Guna2Button b: s.Fill = b.FillColor; break;
                    case Guna2TextBox t: s.Fill = t.FillColor; s.Border = t.BorderColor; break;
                    case Guna2ComboBox cb: s.Fill = cb.FillColor; s.Border = cb.BorderColor; break;
                }
                s.AppFill = s.Fill;
                s.AppBorder = s.Border;
                return s;
            });

            foreach (Control child in c.Controls) Capture(child);
        }

        static void Style(Control c)
        {
            if (!(c.Tag is string tag && tag == "notheme"))
                StyleOne(c);

            foreach (Control child in c.Controls) Style(child);
        }

        static void StyleOne(Control c)
        {
            if (!Snaps.TryGetValue(c, out Snap? s)) return;

            // --- Form: background + picture ---
            if (c is Form f)
            {
                Sync(() => f.BackColor, v => f.BackColor = v, ref s.Back, ref s.AppBack, _ => Bg);
                f.BackgroundImage = Dark ? null : s.Image;
                Sync(() => f.ForeColor, v => f.ForeColor = v, ref s.Fore, ref s.AppFore, MapFore);
                return;
            }

            // --- core Button na walang kulay (default na gray) ---
            if (c is Button btn && c is not Guna2Button && s.UseVisual)
            {
                if (Dark)
                {
                    btn.UseVisualStyleBackColor = false;
                    btn.BackColor = CardAlt;
                    btn.ForeColor = Text;
                }
                else
                {
                    btn.UseVisualStyleBackColor = true;
                }
                return;
            }

            Sync(() => c.BackColor, v => c.BackColor = v, ref s.Back, ref s.AppBack, MapFill);
            Sync(() => c.ForeColor, v => c.ForeColor = v, ref s.Fore, ref s.AppFore, MapFore);

            switch (c)
            {
                case Guna2Panel p:
                    Sync(() => p.FillColor, v => p.FillColor = v, ref s.Fill, ref s.AppFill, MapFill);
                    break;
                case Guna2ShadowPanel sp:
                    Sync(() => sp.FillColor, v => sp.FillColor = v, ref s.Fill, ref s.AppFill, MapFill);
                    break;
                case Guna2Button b:
                    Sync(() => b.FillColor, v => b.FillColor = v, ref s.Fill, ref s.AppFill, MapFill);
                    break;
                case Guna2TextBox t:
                    Sync(() => t.FillColor, v => t.FillColor = v, ref s.Fill, ref s.AppFill, MapInput);
                    Sync(() => t.BorderColor, v => t.BorderColor = v, ref s.Border, ref s.AppBorder, MapBorder);
                    break;
                case Guna2ComboBox cb:
                    Sync(() => cb.FillColor, v => cb.FillColor = v, ref s.Fill, ref s.AppFill, MapInput);
                    Sync(() => cb.BorderColor, v => cb.BorderColor = v, ref s.Border, ref s.AppBorder, MapBorder);
                    break;
                case DataGridView g:
                    StyleGrid(g);
                    break;
            }
        }

        // Kung may nagbago ng kulay galing sa code (hal. RenderPage), tinatanggap iyon
        // bilang bagong "orihinal" para hindi mabura.
        static void Sync(Func<Color> get, Action<Color> set,
                         ref Color orig, ref Color applied, Func<Color, Color> map)
        {
            Color cur = get();
            if (cur.ToArgb() != applied.ToArgb()) orig = cur;

            Color target = Dark ? map(orig) : orig;
            if (get().ToArgb() != target.ToArgb()) set(target);
            applied = target;
        }

        // ---------- MAPPING NG KULAY (light -> dark) ----------
        static Color MapFill(Color c)
        {
            if (c.A == 0) return c;
            if (c.GetBrightness() <= 0.8f) return c;
            return (c.R >= 250 && c.G >= 250 && c.B >= 250) ? Card : CardAlt;
        }

        static Color MapInput(Color c)
        {
            if (c.A == 0) return c;
            return c.GetBrightness() > 0.7f ? CardAlt : c;
        }

        static Color MapBorder(Color c)
        {
            if (c.A == 0) return c;
            return c.GetBrightness() > 0.6f ? Line : c;
        }

        static Color MapFore(Color c)
        {
            if (c.A == 0) return c;
            float br = c.GetBrightness();
            float sat = c.GetSaturation();
            float hue = c.GetHue();

            if (br > 0.65f) return c;                                   // puti / maliwanag na, ok na
            if (sat > 0.25f)
                return (hue >= 80 && hue <= 170)
                    ? Color.FromArgb(120, 205, 155)                     // berde -> mas maliwanag na berde
                    : c;                                                // pula / orange, iwan
            return br > 0.45f ? Muted : Text;                           // gray -> muted, itim -> maliwanag
        }

        static void StyleGrid(DataGridView g)
        {
            g.BackgroundColor = Card;
            g.GridColor = Line;
            g.DefaultCellStyle.BackColor = Card;
            g.DefaultCellStyle.ForeColor = Text;
            g.ColumnHeadersDefaultCellStyle.BackColor = Dark ? CardAlt : Accent;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Dark ? Text : Color.White;
            g.EnableHeadersVisualStyles = false;
        }
    }
}
