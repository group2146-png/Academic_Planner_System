using System;
using System.Drawing;
using System.Windows.Forms;

namespace Academic__Planner
{
    // Ang maliit na "Add / Edit Activity" form
    public partial class Form1 : Form
    {
        private readonly Activity? _edit;   // null = bago, may laman = ine-edit

        public Form1() : this(null) { }   // kailangan ng Visual Studio designer

        public Form1(Activity? edit)
        {
            InitializeComponent();
            _edit = edit;

            ShowInTaskbar = false;

            cmbPriority.Items.AddRange(new object[] { "High", "Medium", "Low" });
            cmbStatus.Items.AddRange(new object[] { "Pending", "Completed" });
            cmbPriority.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;

            dtpDeadline.Format = DateTimePickerFormat.Custom;
            dtpDeadline.CustomFormat = "MMM dd, yyyy";

            if (edit == null)
            {
                lblFormTitle.Text = "Add Activity";
                dtpDeadline.Value = DateTime.Today.AddDays(1);
                cmbPriority.SelectedItem = "Medium";
                cmbStatus.SelectedItem = "Pending";
            }
            else
            {
                lblFormTitle.Text = "Edit Activity";
                txtName.Text = edit.Title;
                txtSubject.Text = edit.Subject;
                dtpDeadline.Value = edit.End;
                txtNotes.Text = edit.Notes;

                // kung may lumang value na wala sa listahan, idagdag
                if (!cmbPriority.Items.Contains(edit.Difficulty) && edit.Difficulty != "")
                    cmbPriority.Items.Add(edit.Difficulty);
                cmbPriority.SelectedItem = edit.Difficulty;
                cmbStatus.SelectedItem = edit.IsCompleted ? "Completed" : "Pending";
            }

            btnSave.Text = "Save";
            btnSave.Click += btnSave_Click;
            btnCancel.Click += (s, e) => DialogResult = DialogResult.Cancel;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            string title = txtName.Text.Trim();
            if (title.Length == 0)
            {
                MessageBox.Show("Enter a title first.");
                txtName.Focus();
                return;
            }

            if (_edit == null && dtpDeadline.Value.Date < DateTime.Today)
            {
                MessageBox.Show("The deadline can't be in the past.");
                return;
            }

            if (cmbPriority.SelectedItem == null)
            {
                MessageBox.Show("Choose a priority.");
                return;
            }

            var a = new Activity
            {
                Id = _edit?.Id ?? 0,
                Title = title,
                Subject = txtSubject.Text.Trim(),
                Difficulty = cmbPriority.SelectedItem.ToString() ?? "Medium",
                Start = _edit?.Start ?? DateTime.Today,
                End = dtpDeadline.Value.Date,
                IsCompleted = cmbStatus.Text == "Completed",
                Notes = txtNotes.Text.Trim()
            };

            try
            {
                if (_edit == null) Db.AddActivity(a);
                else Db.UpdateActivity(a);

                DialogResult = DialogResult.OK;   // isinasara ang form
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving: " + ex.Message);
            }
        }
    }
}
