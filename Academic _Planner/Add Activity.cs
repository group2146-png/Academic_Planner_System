using System;
using System.Drawing;
using System.Windows.Forms;

namespace Academic__Planner
{
    // Ang maliit na "Add / Edit Activity" form
    public partial class Form1 : Form
    {
        private readonly Activity? _edit;   // null = bago, may laman = ine-edit

        public Form1() : this(null) { }

        internal Form1(Activity? edit)
        {
            _edit = edit;
            InitializeComponent();

            // Ang designer ay 1400x900 at nasa gitna ang card. Ililipat natin sa (0,0)
            // at paliliitin ang form para ang card lang ang lumabas sa ibabaw ng dim overlay.
            Point offset = guna2ShadowPanel5.Location;
            foreach (Control c in Controls)
                c.Location = new Point(c.Left - offset.X, c.Top - offset.Y);
            ClientSize = guna2ShadowPanel5.Size;

            // Ayusin ang mga dropdown (isang string lang ang nasa designer kaya hindi gumagana)
            cmbPriority.Items.Clear();
            cmbPriority.Items.AddRange(new object[] { "High", "Medium", "Low" });
            cmbPriority.DropDownStyle = ComboBoxStyle.DropDownList;

            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(new object[] { ActivityRepo.Pending, ActivityRepo.Completed });
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;

            dtpDeadline.Format = DateTimePickerFormat.Short;

            if (_edit == null)
            {
                lblFormTitle.Text = "Add Activity";
                cmbPriority.SelectedItem = "Medium";
                cmbStatus.SelectedItem = ActivityRepo.Pending;
                dtpDeadline.Value = DateTime.Today;
            }
            else
            {
                lblFormTitle.Text = "Edit Activity";
                txtName.Text = _edit.Title;
                txtSubject.Text = _edit.Subject;
                cmbPriority.SelectedItem = _edit.Priority;
                cmbStatus.SelectedItem = _edit.Status;
                txtNotes.Text = _edit.Notes;
                dtpDeadline.Value = _edit.Deadline < dtpDeadline.MinDate ? dtpDeadline.MinDate : _edit.Deadline;
            }

            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            CancelButton = btnCancel;   // Esc = Cancel
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            string title = txtName.Text.Trim();
            string subject = txtSubject.Text.Trim();

            if (title.Length == 0)
            {
                MessageBox.Show("Pakilagay ang title ng activity.", "Activity",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }
            if (subject.Length == 0)
            {
                MessageBox.Show("Pakilagay ang subject.", "Activity",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubject.Focus();
                return;
            }

            var a = _edit ?? new Activity();
            a.Title = title;
            a.Subject = subject;
            a.Priority = cmbPriority.SelectedItem?.ToString() ?? "Medium";
            a.Status = cmbStatus.SelectedItem?.ToString() ?? ActivityRepo.Pending;
            a.Deadline = dtpDeadline.Value.Date;
            a.Notes = txtNotes.Text.Trim();

            try
            {
                if (_edit == null) ActivityRepo.Add(a);
                else ActivityRepo.Update(a);

                DialogResult = DialogResult.OK;   // isasara ang form at malalaman ng nagbukas na may nabago
            }
            catch (Exception ex)
            {
                ActivityUi.ShowError(ex);
            }
        }

        private void btnCancel_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void guna2Panel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
