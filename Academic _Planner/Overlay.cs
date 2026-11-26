using System.Drawing;
using System.Windows.Forms;

namespace Academic__Planner
{
    public static class Overlay
    {
        // Dim-an ang buong "area" form, tapos ilagay ang maliit na form sa gitna nito
        public static DialogResult ShowOver(Form area, Form dialog)
        {
            using (var dim = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                BackColor = Color.Black,
                Opacity = 0.5,
                ShowInTaskbar = false,
                Bounds = area.RectangleToScreen(area.ClientRectangle)
            })
            {
                dim.Show(area);

                dialog.StartPosition = FormStartPosition.Manual;
                dialog.Location = new Point(
                    dim.Left + (dim.Width - dialog.Width) / 2,
                    dim.Top + (dim.Height - dialog.Height) / 2);

                return dialog.ShowDialog(dim);
            }
        }
    }
}
