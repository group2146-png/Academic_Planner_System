using Guna.UI2.WinForms;
using Microsoft.Data.SqlClient;
using System.Media;

namespace Academic__Planner
{
    public class Reminder
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Notes { get; set; } = "";
        public DateTime RemindAt { get; set; }
        public bool IsDone { get; set; }
        public bool Notified { get; set; }   // naipakita na ba ang alert
    }

    // Isang listahan na gamit ng Reminders form at ng alert service
    public static class ReminderStore
    {
        public static List<Reminder> Items { get; } = new List<Reminder>();
    }

    // ---------- DATABASE ----------
    public static class ReminderDb
    {
        public static void Load(int userId)
        {
            Db.EnsureSchema();
            ReminderStore.Items.Clear();

            using (var conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT ReminderId, Title, Notes, RemindAt, IsDone, Notified
                      FROM Reminders WHERE UserId = @uid ORDER BY RemindAt, ReminderId", conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            ReminderStore.Items.Add(new Reminder
                            {
                                Id = r.GetInt32(0),
                                Title = r.GetString(1),
                                Notes = r.IsDBNull(2) ? "" : r.GetString(2),
                                RemindAt = r.GetDateTime(3),
                                IsDone = r.GetBoolean(4),
                                Notified = r.GetBoolean(5)
                            });
                        }
                    }
                }
            }
        }

        static void Run(string sql, Action<SqlCommand> addParams)
        {
            Db.EnsureSchema();
            using (var conn = new SqlConnection(Db.ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", Session.UserId);
                    addParams(cmd);
                    cmd.ExecuteNonQuery();
                }
            }
            Load(Session.UserId);
        }

        public static void Add(Reminder r) =>
            Run(@"INSERT INTO Reminders (UserId, Title, Notes, RemindAt, IsDone, Notified)
                  VALUES (@uid, @title, @notes, @at, 0, 0)", c =>
            {
                c.Parameters.AddWithValue("@title", r.Title);
                c.Parameters.AddWithValue("@notes", r.Notes);
                c.Parameters.AddWithValue("@at", r.RemindAt);
            });

        public static void Update(Reminder r) =>
            Run(@"UPDATE Reminders SET Title = @title, Notes = @notes, RemindAt = @at, Notified = 0
                  WHERE ReminderId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", r.Id);
                c.Parameters.AddWithValue("@title", r.Title);
                c.Parameters.AddWithValue("@notes", r.Notes);
                c.Parameters.AddWithValue("@at", r.RemindAt);
            });

        public static void SetDone(int id, bool done) =>
            Run("UPDATE Reminders SET IsDone = @done WHERE ReminderId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", id);
                c.Parameters.AddWithValue("@done", done);
            });

        public static void Delete(int id) =>
            Run("DELETE FROM Reminders WHERE ReminderId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", id);
            });

        public static void DeleteAll() =>
            Run("DELETE FROM Reminders WHERE UserId = @uid", c => { });

        public static void MarkNotified(int id) =>
            Run("UPDATE Reminders SET Notified = 1 WHERE ReminderId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", id);
            });

        // Ililipat ang oras at babalik sa "hindi pa na-alert", kaya tutunog ulit
        public static void Snooze(int id, DateTime newTime) =>
            Run(@"UPDATE Reminders SET RemindAt = @at, Notified = 0, IsDone = 0
                  WHERE ReminderId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", id);
                c.Parameters.AddWithValue("@at", newTime);
            });
    }

    // ---------- ALERT SERVICE ----------
    // Tumatakbo habang naka-login: bawat 15 segundo tinitingnan kung may reminder na dumating na ang oras.
    public static class ReminderService
    {
        static System.Windows.Forms.Timer? _timer;
        static int _uid;

        public static void Start()
        {
            if (Session.UserId == 0) return;
            if (_timer != null && _uid == Session.UserId) return;

            Stop();
            _uid = Session.UserId;

            try { ReminderDb.Load(_uid); }
            catch (Exception ex) { MessageBox.Show("Error loading reminders: " + ex.Message); }

            _timer = new System.Windows.Forms.Timer { Interval = 15000 };
            _timer.Tick += (s, e) => Check();
            _timer.Start();
            Check();
        }

        public static void Stop()
        {
            _timer?.Stop();
            _timer?.Dispose();
            _timer = null;
            _uid = 0;
            ReminderStore.Items.Clear();
        }

        static void Check()
        {
            var due = ReminderStore.Items
                .Where(r => !r.IsDone && !r.Notified && r.RemindAt <= DateTime.Now)
                .OrderBy(r => r.RemindAt)
                .ToList();

            if (due.Count == 0) return;
            if (!AppPrefs.ReminderAlerts) return;   // naka-off ang alerts sa Settings

            try
            {
                foreach (var r in due) ReminderDb.MarkNotified(r.Id);
            }
            catch { return; }   // walang connection, subukan ulit sa susunod

            if (AppPrefs.ReminderSound) SystemSounds.Exclamation.Play();

            int slot = 0;
            foreach (var r in due.Take(3))
                ReminderToast.ShowFor(r, slot++);

            Application.OpenForms.OfType<Reminders>().FirstOrDefault()?.RefreshList();
        }
    }

    // ---------- ANG POP-UP NA LUMALABAS ----------
    public class ReminderToast : Form
    {
        protected override bool ShowWithoutActivation => true;

        public static void ShowFor(Reminder r, int slot)
        {
            var t = new ReminderToast(r);
            var wa = Screen.PrimaryScreen!.WorkingArea;
            t.StartPosition = FormStartPosition.Manual;
            t.Location = new Point(wa.Right - t.Width - 16, wa.Bottom - (t.Height + 10) * (slot + 1) - 6);
            t.Show();
        }

        ReminderToast(Reminder r)
        {
            Size = new Size(360, 170);
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            BackColor = Color.White;
            Theme.Hook(this, false);

            var head = new Guna2HtmlLabel
            {
                Text = "Reminder",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 125, 82),
                BackColor = Color.Transparent,
                Location = new Point(16, 12)
            };

            var title = new Label
            {
                Text = r.Title,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 60, 50),
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                Bounds = new Rectangle(16, 36, 328, 28)
            };

            var note = new Label
            {
                Text = r.Notes.Length > 0 ? r.Notes : "It's time!  (" + r.RemindAt.ToString("MMM dd, h:mm tt") + ")",
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(110, 125, 116),
                BackColor = Color.Transparent,
                AutoEllipsis = true,
                Bounds = new Rectangle(16, 66, 328, 38)
            };

            Guna2Button MakeBtn(string text, int x, int w, bool primary)
            {
                var b = new Guna2Button
                {
                    Text = text,
                    BorderRadius = 8,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    FillColor = primary ? Color.FromArgb(46, 125, 82) : Color.FromArgb(232, 242, 235),
                    ForeColor = primary ? Color.White : Color.FromArgb(40, 60, 50),
                    Bounds = new Rectangle(x, 120, w, 36)
                };
                return b;
            }

            var done = MakeBtn("Done", 16, 90, true);
            var snooze = MakeBtn("Snooze 10 min", 114, 130, false);
            var close = MakeBtn("Dismiss", 252, 92, false);

            done.Click += (s, e) =>
            {
                try { ReminderDb.SetDone(r.Id, true); } catch { }
                Application.OpenForms.OfType<Reminders>().FirstOrDefault()?.RefreshList();
                Close();
            };
            snooze.Click += (s, e) =>
            {
                try { ReminderDb.Snooze(r.Id, DateTime.Now.AddMinutes(10)); } catch { }
                Application.OpenForms.OfType<Reminders>().FirstOrDefault()?.RefreshList();
                Close();
            };
            close.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { head, title, note, done, snooze, close });
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            ControlPaint.DrawBorder(e.Graphics, ClientRectangle, Color.FromArgb(46, 125, 82), ButtonBorderStyle.Solid);
        }
    }
}
