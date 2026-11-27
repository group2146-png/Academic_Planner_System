using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Academic__Planner
{
    public partial class Completed : Form
    {
        const int PageSize = 4;
        int page = 0;

        Guna2HtmlLabel[] titles, subjects, priorities, deadlines, statuses;
        Guna2Button[] reopenBtns, deleteBtns, hiddenBtns, pageBtns;
    }
}
}