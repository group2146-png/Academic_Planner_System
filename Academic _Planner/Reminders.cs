using Guna.UI2.WinForms;

namespace Academic__Planner
{
    // Reminders page: gawa ng bago, i-edit, Done/Reopen, Snooze, Delete, may filter tabs,
    // at may alert (pop-up) pag dumating na ang oras (tingnan ang ReminderService).
    public class Reminders : Form
    {
        // ---- kaliwa: form ----
        readonly Guna2HtmlLabel lblFormTitle = new Guna2HtmlLabel();
        readonly Guna2TextBox txtTitle = new Guna2TextBox();
        readonly Guna2TextBox txtNotes = new Guna2TextBox();
        readonly DateTimePicker dtpDate = new DateTimePicker();
        readonly DateTimePicker dtpTime = new DateTimePicker();
        readonly Guna2Button btnSave = new Guna2Button();
        readonly Guna2Button btnCancelEdit = new Guna2Button();
        readonly Label lblDeadlines = new Label();

        // ---- kanan: listahan ----
        readonly Guna2Button[] tabs = new Guna2Button[4];
        readonly string[] tabNames = { "Upcoming", "Overdue", "Done", "All" };
        readonly DataGridView grid = new DataGridView();
        readonly Guna2Button btnDone = new Guna2Button();
        readonly Guna2Button btnEdit = new Guna2Button();
        readonly Guna2Button btnSnooze = new Guna2Button();
        readonly Guna2Button btnDelete = new Guna2Button();
        readonly Label lblEmpty = new Label();

        string _filter = "Upcoming";
        Reminder? _editing;
        private Guna2Button btnEnableNotif;
        private Guna2ShadowPanel cardOverdue;
        private Guna2HtmlLabel guna2HtmlLabel1;
        private Guna2ShadowPanel cardToday;
        private Guna2HtmlLabel guna2HtmlLabel2;
        private Guna2ShadowPanel cardUpcoming;
        private Guna2HtmlLabel guna2HtmlLabel3;
        private Guna2HtmlLabel lblEmptyOverdue;
        private FlowLayoutPanel flpOverdue;
        private Guna2HtmlLabel lblEmptyToday;
        private FlowLayoutPanel flpToday;
        private Guna2HtmlLabel lblEmptyUpcoming;
        private FlowLayoutPanel flpUpcoming;
        private Guna2CircleButton guna2CircleButton1;
        private Guna2CircleButton btnBell;
        private Guna2HtmlLabel lblBellBadge;
        private Guna2TextBox txtSearch;
        bool _goingBack;

        public Reminders()
        {
            Text = "Reminders";
            ClientSize = new Size(1405, 861);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(241, 248, 242);
            Font = new Font("Segoe UI", 9F);

            BuildHeader();
            BuildFormCard();
            BuildListCard();

            Theme.Hook(this);
            Action restyle = () => { StyleGrid(); UpdateTabs(); RefreshList(); };
            Theme.Changed += restyle;
            FormClosed += (s, e) => Theme.Changed -= restyle;

            Load += (s, e) =>
            {
                try { ReminderDb.Load(Session.UserId); }
                catch (Exception ex) { MessageBox.Show("Error loading reminders: " + ex.Message); }
                ResetForm();
                StyleGrid();
                RefreshList();
            };

            VisibleChanged += (s, e) => { if (Visible && IsHandleCreated) RefreshList(); };

            FormClosed += (s, e) =>
            {
                // X button lang ang magpapatay ng app (ang Back button ay hindi)
                if (!_goingBack && e.CloseReason == CloseReason.UserClosing &&
                    !Application.OpenForms.Cast<Form>().Any(x => x.Visible))
                    Application.Exit();
            };
        }

        // =====================================================================
        //  UI
        // =====================================================================
        void BuildHeader()
        {
            var bar = new Guna2Panel
            {
                FillColor = Color.FromArgb(46, 125, 82),
                Bounds = new Rectangle(0, 0, ClientSize.Width, 72),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                Tag = "notheme"
            };

            var back = new Guna2Button
            {
                Text = "Back",
                BorderRadius = 8,
                FillColor = Color.White,
                ForeColor = Color.FromArgb(46, 125, 82),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Bounds = new Rectangle(30, 19, 90, 34),
                Tag = "notheme"
            };
            back.Click += (s, e) =>
            {
                _goingBack = true;
                Dashboard dashboard = Application.OpenForms.OfType<Dashboard>().FirstOrDefault() ?? new Dashboard();
                dashboard.Show();
                Close();
            };

            var title = new Label
            {
                Text = "Reminders",
                Font = new Font("Segoe UI", 20F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = true,
                Location = new Point(140, 16),
                Tag = "notheme"
            };

            bar.Controls.Add(back);
            bar.Controls.Add(title);
            Controls.Add(bar);
        }

        void BuildFormCard()
        {
            var card = new Guna2Panel
            {
                FillColor = Color.White,
                BorderRadius = 14,
                Bounds = new Rectangle(30, 100, 400, 530)
            };

            lblFormTitle.Text = "New Reminder";
            lblFormTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblFormTitle.ForeColor = Color.FromArgb(40, 60, 50);
            lblFormTitle.BackColor = Color.Transparent;
            lblFormTitle.Location = new Point(24, 18);

            Control Caption(string text, int y) => new Guna2HtmlLabel
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(110, 125, 116),
                BackColor = Color.Transparent,
                Location = new Point(24, y)
            };

            txtTitle.PlaceholderText = "What do you need to remember?";
            txtTitle.BorderRadius = 8;
            txtTitle.Font = new Font("Segoe UI", 10.5F);
            txtTitle.Bounds = new Rectangle(24, 78, 352, 40);
            txtTitle.MaxLength = 150;

            txtNotes.PlaceholderText = "Optional notes";
            txtNotes.BorderRadius = 8;
            txtNotes.Multiline = true;
            txtNotes.Font = new Font("Segoe UI", 10F);
            txtNotes.Bounds = new Rectangle(24, 158, 352, 80);
            txtNotes.MaxLength = 500;

            dtpDate.Format = DateTimePickerFormat.Custom;
            dtpDate.CustomFormat = "MMM dd, yyyy";
            dtpDate.Font = new Font("Segoe UI", 10.5F);
            dtpDate.Bounds = new Rectangle(24, 278, 200, 29);

            dtpTime.Format = DateTimePickerFormat.Custom;
            dtpTime.CustomFormat = "hh:mm tt";
            dtpTime.ShowUpDown = true;
            dtpTime.Font = new Font("Segoe UI", 10.5F);
            dtpTime.Bounds = new Rectangle(240, 278, 136, 29);

            // mabilis na pagpili ng oras
            Guna2Button Quick(string text, int x, Func<DateTime> when)
            {
                var b = new Guna2Button
                {
                    Text = text,
                    BorderRadius = 8,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    FillColor = Color.FromArgb(232, 242, 235),
                    ForeColor = Color.FromArgb(40, 60, 50),
                    Bounds = new Rectangle(x, 326, 112, 32)
                };
                b.Click += (s, e) => SetWhen(when());
                return b;
            }

            var q1 = Quick("In 1 hour", 24, () => DateTime.Now.AddHours(1));
            var q2 = Quick("Tonight 8 PM", 144, () => DateTime.Today.AddHours(20) > DateTime.Now
                ? DateTime.Today.AddHours(20) : DateTime.Today.AddDays(1).AddHours(20));
            var q3 = Quick("Tomorrow 8 AM", 264, () => DateTime.Today.AddDays(1).AddHours(8));

            btnSave.Text = "Add Reminder";
            btnSave.BorderRadius = 10;
            btnSave.FillColor = Color.FromArgb(46, 125, 82);
            btnSave.ForeColor = Color.White;
            btnSave.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btnSave.Bounds = new Rectangle(24, 388, 352, 44);
            btnSave.Click += (s, e) => SaveReminder();

            btnCancelEdit.Text = "Cancel edit";
            btnCancelEdit.BorderRadius = 10;
            btnCancelEdit.FillColor = Color.FromArgb(232, 242, 235);
            btnCancelEdit.ForeColor = Color.FromArgb(40, 60, 50);
            btnCancelEdit.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancelEdit.Bounds = new Rectangle(24, 442, 352, 38);
            btnCancelEdit.Visible = false;
            btnCancelEdit.Click += (s, e) => ResetForm();

            card.Controls.Add(lblFormTitle);
            card.Controls.Add(Caption("Title", 54));
            card.Controls.Add(txtTitle);
            card.Controls.Add(Caption("Notes", 134));
            card.Controls.Add(txtNotes);
            card.Controls.Add(Caption("Date", 254));
            card.Controls.Add(Caption("Time", 254));
            card.Controls[card.Controls.Count - 1].Left = 240;
            card.Controls.Add(dtpDate);
            card.Controls.Add(dtpTime);
            card.Controls.Add(q1);
            card.Controls.Add(q2);
            card.Controls.Add(q3);
            card.Controls.Add(btnSave);
            card.Controls.Add(btnCancelEdit);
            Controls.Add(card);

            // ---- Deadlines ng activities (info lang) ----
            var dl = new Guna2Panel
            {
                FillColor = Color.White,
                BorderRadius = 14,
                Bounds = new Rectangle(30, 650, 400, 180)
            };

            var dlTitle = new Guna2HtmlLabel
            {
                Text = "Activity deadlines (next 3 days)",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 50),
                BackColor = Color.Transparent,
                Location = new Point(24, 14)
            };

            lblDeadlines.Font = new Font("Segoe UI", 9.5F);
            lblDeadlines.ForeColor = Color.FromArgb(40, 60, 50);
            lblDeadlines.BackColor = Color.Transparent;
            lblDeadlines.Bounds = new Rectangle(24, 48, 352, 120);

            dl.Controls.Add(dlTitle);
            dl.Controls.Add(lblDeadlines);
            Controls.Add(dl);
        }

        void BuildListCard()
        {
            var card = new Guna2Panel
            {
                FillColor = Color.White,
                BorderRadius = 14,
                Bounds = new Rectangle(460, 100, 915, 730)
            };

            // tabs
            for (int i = 0; i < tabs.Length; i++)
            {
                string name = tabNames[i];
                tabs[i] = new Guna2Button
                {
                    BorderRadius = 8,
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    Bounds = new Rectangle(24 + i * 150, 20, 140, 38),
                    Tag = "notheme"
                };
                tabs[i].Click += (s, e) => { _filter = name; UpdateTabs(); RefreshList(); };
                card.Controls.Add(tabs[i]);
            }

            // listahan
            grid.Bounds = new Rectangle(24, 78, 867, 568);
            grid.Tag = "notheme";
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.RowHeadersVisible = false;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 42;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.Font = new Font("Segoe UI", 10F);

            grid.Columns.Add("colTitle", "Title");
            grid.Columns.Add("colNotes", "Notes");
            grid.Columns.Add("colWhen", "When");
            grid.Columns.Add("colStatus", "Status");
            grid.Columns[0].FillWeight = 34;
            grid.Columns[1].FillWeight = 28;
            grid.Columns[2].FillWeight = 22;
            grid.Columns[3].FillWeight = 16;

            grid.SelectionChanged += (s, e) => UpdateButtons();
            grid.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) StartEdit(); };

            lblEmpty.Text = "Nothing here yet.";
            lblEmpty.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblEmpty.TextAlign = ContentAlignment.MiddleCenter;
            lblEmpty.Bounds = new Rectangle(24, 300, 867, 40);
            lblEmpty.Tag = "notheme";

            // mga button sa ilalim
            void Style(Guna2Button b, string text, int x, int w, bool danger = false)
            {
                b.Text = text;
                b.BorderRadius = 10;
                b.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                b.FillColor = danger ? Color.FromArgb(200, 55, 70) : Color.FromArgb(46, 125, 82);
                b.ForeColor = Color.White;
                b.Bounds = new Rectangle(x, 666, w, 42);
                b.Tag = "notheme";
            }

            Style(btnDone, "Mark Done", 24, 170);
            Style(btnEdit, "Edit", 206, 140);
            Style(btnSnooze, "Snooze +1 hr", 358, 170);
            Style(btnDelete, "Delete", 741, 150, true);

            btnDone.Click += (s, e) => ToggleDone();
            btnEdit.Click += (s, e) => StartEdit();
            btnSnooze.Click += (s, e) => SnoozeSelected();
            btnDelete.Click += (s, e) => DeleteSelected();

            card.Controls.Add(grid);
            card.Controls.Add(lblEmpty);
            card.Controls.Add(btnDone);
            card.Controls.Add(btnEdit);
            card.Controls.Add(btnSnooze);
            card.Controls.Add(btnDelete);
            Controls.Add(card);

            UpdateTabs();
            UpdateButtons();
        }

        // Kulay ng listahan at tabs (sumusunod sa dark / light mode)
        void StyleGrid()
        {
            grid.BackgroundColor = Theme.Card;
            grid.GridColor = Theme.Line;
            grid.EnableHeadersVisualStyles = false;

            grid.DefaultCellStyle.BackColor = Theme.Card;
            grid.DefaultCellStyle.ForeColor = Theme.Text;
            grid.DefaultCellStyle.SelectionBackColor = Theme.Accent;
            grid.DefaultCellStyle.SelectionForeColor = Color.White;
            grid.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            grid.ColumnHeadersDefaultCellStyle.BackColor = Theme.Accent;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Theme.Accent;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            lblEmpty.BackColor = Theme.Card;
            lblEmpty.ForeColor = Theme.Muted;
        }

        void UpdateTabs()
        {
            for (int i = 0; i < tabs.Length; i++)
            {
                bool active = tabNames[i] == _filter;
                tabs[i].FillColor = active ? Theme.Accent : Theme.CardAlt;
                tabs[i].ForeColor = active ? Color.White : Theme.Text;
                tabs[i].Text = $"{tabNames[i]} ({CountFor(tabNames[i])})";
            }
        }

        // =====================================================================
        //  LISTAHAN
        // =====================================================================
        static bool Matches(Reminder r, string filter)
        {
            DateTime now = DateTime.Now;
            switch (filter)
            {
                case "Upcoming": return !r.IsDone && r.RemindAt >= now;
                case "Overdue": return !r.IsDone && r.RemindAt < now;
                case "Done": return r.IsDone;
                default: return true;
            }
        }

        static int CountFor(string filter) => ReminderStore.Items.Count(r => Matches(r, filter));

        // Public para matawag ng alert service pagkatapos mag-pop up
        public void RefreshList()
        {
            if (IsDisposed || !IsHandleCreated) return;

            int keepId = SelectedReminder()?.Id ?? -1;

            var rows = ReminderStore.Items.Where(r => Matches(r, _filter));
            rows = _filter == "Done" ? rows.OrderByDescending(r => r.RemindAt) : rows.OrderBy(r => r.RemindAt);

            grid.Rows.Clear();
            DateTime now = DateTime.Now;

            foreach (var r in rows)
            {
                string status = r.IsDone ? "Done" : (r.RemindAt < now ? "Overdue" : "Upcoming");
                int idx = grid.Rows.Add(
                    r.Title,
                    r.Notes,
                    r.RemindAt.ToString("MMM dd, yyyy  h:mm tt"),
                    status);

                var row = grid.Rows[idx];
                row.Tag = r;

                Color statusColor = r.IsDone ? Color.FromArgb(60, 160, 105)
                                  : r.RemindAt < now ? Color.FromArgb(220, 60, 80)
                                  : Theme.Text;
                row.Cells[3].Style.ForeColor = statusColor;
                row.Cells[3].Style.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                if (r.IsDone)
                {
                    row.Cells[0].Style.ForeColor = Theme.Muted;
                    row.Cells[0].Style.Font = new Font("Segoe UI", 10F, FontStyle.Strikeout);
                }

                if (r.Id == keepId) { grid.ClearSelection(); row.Selected = true; }
            }

            lblEmpty.Visible = grid.Rows.Count == 0;
            lblEmpty.Text = _filter switch
            {
                "Upcoming" => "No upcoming reminders. Add one on the left!",
                "Overdue" => "Nothing overdue. Nice!",
                "Done" => "No finished reminders yet.",
                _ => "No reminders yet. Add one on the left!"
            };

            UpdateTabs();
            UpdateButtons();
            UpdateDeadlines();
        }

        Reminder? SelectedReminder() =>
            grid.SelectedRows.Count > 0 ? grid.SelectedRows[0].Tag as Reminder : null;

        void UpdateButtons()
        {
            var r = SelectedReminder();
            bool has = r != null;
            btnDone.Enabled = has;
            btnEdit.Enabled = has;
            btnSnooze.Enabled = has;
            btnDelete.Enabled = has;
            btnDone.Text = (r != null && r.IsDone) ? "Reopen" : "Mark Done";
        }

        void UpdateDeadlines()
        {
            DateTime today = DateTime.Today;
            var soon = ActivityStore.Pending()
                .Where(a => a.End.Date <= today.AddDays(3))
                .Take(5)
                .Select(a => (a.End.Date < today ? "(overdue)  " : "") + a.Title + "  -  " + a.End.ToString("MMM dd"))
                .ToList();

            lblDeadlines.Text = soon.Count == 0
                ? "No activity deadlines in the next 3 days."
                : string.Join(Environment.NewLine, soon.Select(x => "•  " + x));
        }

        // =====================================================================
        //  MGA ACTION
        // =====================================================================
        void SetWhen(DateTime when)
        {
            dtpDate.Value = when.Date;
            dtpTime.Value = DateTime.Today.Add(when.TimeOfDay);
        }

        void ResetForm()
        {
            _editing = null;
            lblFormTitle.Text = "New Reminder";
            btnSave.Text = "Add Reminder";
            btnCancelEdit.Visible = false;
            txtTitle.Clear();
            txtNotes.Clear();
            SetWhen(DateTime.Now.AddHours(1));
        }

        void StartEdit()
        {
            var r = SelectedReminder();
            if (r == null) return;

            _editing = r;
            lblFormTitle.Text = "Edit Reminder";
            btnSave.Text = "Save Changes";
            btnCancelEdit.Visible = true;
            txtTitle.Text = r.Title;
            txtNotes.Text = r.Notes;
            SetWhen(r.RemindAt);
            txtTitle.Focus();
        }

        void SaveReminder()
        {
            string title = txtTitle.Text.Trim();
            if (title.Length == 0)
            {
                MessageBox.Show("Enter what you want to be reminded about.");
                txtTitle.Focus();
                return;
            }

            DateTime when = dtpDate.Value.Date + dtpTime.Value.TimeOfDay;
            when = new DateTime(when.Year, when.Month, when.Day, when.Hour, when.Minute, 0);

            if (_editing == null && when < DateTime.Now)
            {
                MessageBox.Show("That time already passed. Pick a time in the future.");
                return;
            }

            try
            {
                var r = _editing ?? new Reminder();
                r.Title = title;
                r.Notes = txtNotes.Text.Trim();
                r.RemindAt = when;

                if (_editing == null) ReminderDb.Add(r);
                else ReminderDb.Update(r);

                // pagkatapos mag-save, ipakita sa tamang tab
                _filter = when < DateTime.Now ? "Overdue" : "Upcoming";
                ResetForm();
                RefreshList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving: " + ex.Message);
            }
        }

        void ToggleDone()
        {
            var r = SelectedReminder();
            if (r == null) return;

            try
            {
                ReminderDb.SetDone(r.Id, !r.IsDone);
                RefreshList();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        void SnoozeSelected()
        {
            var r = SelectedReminder();
            if (r == null) return;

            try
            {
                DateTime baseTime = r.RemindAt > DateTime.Now ? r.RemindAt : DateTime.Now;
                ReminderDb.Snooze(r.Id, baseTime.AddHours(1));
                RefreshList();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            btnEnableNotif = new Guna2Button();
            cardOverdue = new Guna2ShadowPanel();
            lblEmptyOverdue = new Guna2HtmlLabel();
            flpOverdue = new FlowLayoutPanel();
            guna2HtmlLabel1 = new Guna2HtmlLabel();
            cardToday = new Guna2ShadowPanel();
            lblEmptyToday = new Guna2HtmlLabel();
            flpToday = new FlowLayoutPanel();
            guna2HtmlLabel2 = new Guna2HtmlLabel();
            cardUpcoming = new Guna2ShadowPanel();
            guna2CircleButton1 = new Guna2CircleButton();
            lblEmptyUpcoming = new Guna2HtmlLabel();
            flpUpcoming = new FlowLayoutPanel();
            guna2HtmlLabel3 = new Guna2HtmlLabel();
            btnBell = new Guna2CircleButton();
            lblBellBadge = new Guna2HtmlLabel();
            txtSearch = new Guna2TextBox();
            cardOverdue.SuspendLayout();
            cardToday.SuspendLayout();
            cardUpcoming.SuspendLayout();
            SuspendLayout();
            // 
            // btnEnableNotif
            // 
            btnEnableNotif.CustomizableEdges = customizableEdges1;
            btnEnableNotif.DisabledState.BorderColor = Color.DarkGray;
            btnEnableNotif.DisabledState.CustomBorderColor = Color.DarkGray;
            btnEnableNotif.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnEnableNotif.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnEnableNotif.Font = new Font("Segoe UI", 9F);
            btnEnableNotif.ForeColor = Color.White;
            btnEnableNotif.Location = new Point(1012, 49);
            btnEnableNotif.Name = "btnEnableNotif";
            btnEnableNotif.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnEnableNotif.Size = new Size(180, 45);
            btnEnableNotif.TabIndex = 0;
            btnEnableNotif.Text = "btnEnableNotif";
            // 
            // cardOverdue
            // 
            cardOverdue.BackColor = Color.Transparent;
            cardOverdue.Controls.Add(lblEmptyOverdue);
            cardOverdue.Controls.Add(flpOverdue);
            cardOverdue.Controls.Add(guna2HtmlLabel1);
            cardOverdue.FillColor = Color.White;
            cardOverdue.Location = new Point(55, 144);
            cardOverdue.Name = "cardOverdue";
            cardOverdue.Radius = 20;
            cardOverdue.ShadowColor = Color.Black;
            cardOverdue.Size = new Size(1303, 187);
            cardOverdue.TabIndex = 1;
            // 
            // lblEmptyOverdue
            // 
            lblEmptyOverdue.BackColor = Color.Transparent;
            lblEmptyOverdue.Location = new Point(591, 141);
            lblEmptyOverdue.Name = "lblEmptyOverdue";
            lblEmptyOverdue.Size = new Size(97, 17);
            lblEmptyOverdue.TabIndex = 4;
            lblEmptyOverdue.Text = "guna2HtmlLabel4";
            // 
            // flpOverdue
            // 
            flpOverdue.AutoScroll = true;
            flpOverdue.FlowDirection = FlowDirection.TopDown;
            flpOverdue.Location = new Point(37, 58);
            flpOverdue.Name = "flpOverdue";
            flpOverdue.Size = new Size(200, 100);
            flpOverdue.TabIndex = 1;
            flpOverdue.WrapContents = false;
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Location = new Point(37, 26);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(48, 17);
            guna2HtmlLabel1.TabIndex = 0;
            guna2HtmlLabel1.Text = "Overdue";
            // 
            // cardToday
            // 
            cardToday.BackColor = Color.Transparent;
            cardToday.Controls.Add(lblEmptyToday);
            cardToday.Controls.Add(flpToday);
            cardToday.Controls.Add(guna2HtmlLabel2);
            cardToday.FillColor = Color.White;
            cardToday.Location = new Point(55, 353);
            cardToday.Name = "cardToday";
            cardToday.Radius = 20;
            cardToday.ShadowColor = Color.Black;
            cardToday.Size = new Size(1303, 187);
            cardToday.TabIndex = 2;
            // 
            // lblEmptyToday
            // 
            lblEmptyToday.BackColor = Color.Transparent;
            lblEmptyToday.Location = new Point(591, 128);
            lblEmptyToday.Name = "lblEmptyToday";
            lblEmptyToday.Size = new Size(97, 17);
            lblEmptyToday.TabIndex = 5;
            lblEmptyToday.Text = "guna2HtmlLabel5";
            // 
            // flpToday
            // 
            flpToday.AutoScroll = true;
            flpToday.FlowDirection = FlowDirection.TopDown;
            flpToday.Location = new Point(37, 45);
            flpToday.Name = "flpToday";
            flpToday.Size = new Size(200, 100);
            flpToday.TabIndex = 2;
            flpToday.WrapContents = false;
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Location = new Point(37, 22);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(36, 17);
            guna2HtmlLabel2.TabIndex = 0;
            guna2HtmlLabel2.Text = "Today";
            // 
            // cardUpcoming
            // 
            cardUpcoming.BackColor = Color.Transparent;
            cardUpcoming.Controls.Add(guna2CircleButton1);
            cardUpcoming.Controls.Add(lblEmptyUpcoming);
            cardUpcoming.Controls.Add(flpUpcoming);
            cardUpcoming.Controls.Add(guna2HtmlLabel3);
            cardUpcoming.FillColor = Color.White;
            cardUpcoming.Location = new Point(55, 562);
            cardUpcoming.Name = "cardUpcoming";
            cardUpcoming.Radius = 20;
            cardUpcoming.ShadowColor = Color.Black;
            cardUpcoming.Size = new Size(1303, 187);
            cardUpcoming.TabIndex = 3;
            // 
            // guna2CircleButton1
            // 
            guna2CircleButton1.DisabledState.BorderColor = Color.DarkGray;
            guna2CircleButton1.DisabledState.CustomBorderColor = Color.DarkGray;
            guna2CircleButton1.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            guna2CircleButton1.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            guna2CircleButton1.Font = new Font("Segoe UI", 9F);
            guna2CircleButton1.ForeColor = Color.White;
            guna2CircleButton1.Location = new Point(1325, 114);
            guna2CircleButton1.Name = "guna2CircleButton1";
            guna2CircleButton1.ShadowDecoration.CustomizableEdges = customizableEdges3;
            guna2CircleButton1.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            guna2CircleButton1.Size = new Size(78, 95);
            guna2CircleButton1.TabIndex = 4;
            guna2CircleButton1.Text = "guna2CircleButton1";
            // 
            // lblEmptyUpcoming
            // 
            lblEmptyUpcoming.BackColor = Color.Transparent;
            lblEmptyUpcoming.Location = new Point(591, 144);
            lblEmptyUpcoming.Name = "lblEmptyUpcoming";
            lblEmptyUpcoming.Size = new Size(97, 17);
            lblEmptyUpcoming.TabIndex = 5;
            lblEmptyUpcoming.Text = "guna2HtmlLabel6";
            // 
            // flpUpcoming
            // 
            flpUpcoming.AutoScroll = true;
            flpUpcoming.FlowDirection = FlowDirection.TopDown;
            flpUpcoming.Location = new Point(59, 61);
            flpUpcoming.Name = "flpUpcoming";
            flpUpcoming.Size = new Size(200, 100);
            flpUpcoming.TabIndex = 2;
            flpUpcoming.WrapContents = false;
            // 
            // guna2HtmlLabel3
            // 
            guna2HtmlLabel3.BackColor = Color.Transparent;
            guna2HtmlLabel3.Location = new Point(37, 28);
            guna2HtmlLabel3.Name = "guna2HtmlLabel3";
            guna2HtmlLabel3.Size = new Size(59, 17);
            guna2HtmlLabel3.TabIndex = 1;
            guna2HtmlLabel3.Text = "Upcoming";
            // 
            // btnBell
            // 
            btnBell.DisabledState.BorderColor = Color.DarkGray;
            btnBell.DisabledState.CustomBorderColor = Color.DarkGray;
            btnBell.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnBell.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnBell.Font = new Font("Segoe UI", 9F);
            btnBell.ForeColor = Color.White;
            btnBell.Location = new Point(1334, 49);
            btnBell.Name = "btnBell";
            btnBell.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnBell.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            btnBell.Size = new Size(47, 45);
            btnBell.TabIndex = 4;
            btnBell.Text = "guna2CircleButton2";
            // 
            // lblBellBadge
            // 
            lblBellBadge.BackColor = Color.Transparent;
            lblBellBadge.Location = new Point(1260, 64);
            lblBellBadge.Name = "lblBellBadge";
            lblBellBadge.Size = new Size(68, 17);
            lblBellBadge.TabIndex = 5;
            lblBellBadge.Text = "lblBellBadge";
            // 
            // txtSearch
            // 
            txtSearch.BorderRadius = 20;
            txtSearch.CustomizableEdges = customizableEdges5;
            txtSearch.DefaultText = "";
            txtSearch.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtSearch.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtSearch.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtSearch.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtSearch.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearch.Font = new Font("Segoe UI", 9F);
            txtSearch.HoverState.BorderColor = Color.FromArgb(94, 148, 255);
            txtSearch.Location = new Point(330, 41);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtSearch.Size = new Size(538, 53);
            txtSearch.TabIndex = 6;
            // 
            // Reminders
            // 
            ClientSize = new Size(1537, 891);
            Controls.Add(txtSearch);
            Controls.Add(lblBellBadge);
            Controls.Add(btnBell);
            Controls.Add(cardUpcoming);
            Controls.Add(cardToday);
            Controls.Add(cardOverdue);
            Controls.Add(btnEnableNotif);
            Name = "Reminders";
            Load += Reminders_Load;
            cardOverdue.ResumeLayout(false);
            cardOverdue.PerformLayout();
            cardToday.ResumeLayout(false);
            cardToday.PerformLayout();
            cardUpcoming.ResumeLayout(false);
            cardUpcoming.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        void DeleteSelected()
        {
            var r = SelectedReminder();
            if (r == null) return;

            var ok = MessageBox.Show($"Delete \"{r.Title}\"?", "Delete reminder",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (ok != DialogResult.Yes) return;

            try
            {
                if (_editing?.Id == r.Id) ResetForm();
                ReminderDb.Delete(r.Id);
                RefreshList();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void guna2CircleButton1_Click(object sender, EventArgs e)
        {

        }

        private void Reminders_Load(object sender, EventArgs e)
        {

        }
    }
}
