using System.Drawing.Drawing2D;

namespace Academic__Planner
{
    // Bilog na progress (ginagamit ng Focus Timer sa Dashboard)
    public class RingProgress : Control
    {
        float _progress = 1f;
        string _center = "";
        string _sub = "";

        public float Progress
        {
            get => _progress;
            set { _progress = Math.Clamp(value, 0f, 1f); Invalidate(); }
        }

        public string CenterText
        {
            get => _center;
            set { _center = value; Invalidate(); }
        }

        public string SubText
        {
            get => _sub;
            set { _sub = value; Invalidate(); }
        }

        public RingProgress()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            Tag = "notheme";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.Clear(Theme.Card);
            g.SmoothingMode = SmoothingMode.AntiAlias;

            const int thick = 10;
            var rect = new Rectangle(thick, thick, Width - thick * 2 - 1, Height - thick * 2 - 1);

            using (var track = new Pen(Theme.CardAlt, thick))
                g.DrawEllipse(track, rect);

            if (_progress > 0.001f)
            {
                using (var arc = new Pen(Theme.Accent, thick) { StartCap = LineCap.Round, EndCap = LineCap.Round })
                    g.DrawArc(arc, rect, -90f, 360f * _progress);
            }

            var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding;

            using (var big = new Font("Segoe UI", 19F, FontStyle.Bold))
                TextRenderer.DrawText(g, _center, big, new Rectangle(0, Height / 2 - 26, Width, 36), Theme.Text, flags);

            using (var small = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                TextRenderer.DrawText(g, _sub, small, new Rectangle(0, Height / 2 + 12, Width, 18), Theme.Muted, flags);
        }
    }
}
