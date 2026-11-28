using System.Net;
using Guna.UI2.WinForms;
using MySqlConnector;

namespace Academic__Planner
{
    /// <summary>Isang activity (assignment / project / exam, etc.).</summary>
    internal class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Subject { get; set; } = "";
        public string Priority { get; set; } = "Medium";   // High / Medium / Low
        public DateTime Deadline { get; set; } = DateTime.Today;
        public string Status { get; set; } = "Pending";    // Pending / Completed
        public string Notes { get; set; } = "";
    }

    /// <summary>Lahat ng database commands para sa activities. Naka-link sa naka-login na user.</summary>
    internal static class ActivityRepo
    {
        public const string Pending = "Pending";
        public const string Completed = "Completed";

        private static bool _tableReady;

        private static int UserId =>
            Session.CurrentUser?.Id ?? throw new InvalidOperationException("Walang naka-login na user.");

        private static MySqlConnection Open()
        {
            var conn = Db.GetConnection();
            conn.Open();

            if (!_tableReady)
            {
                // Awtomatikong gagawa ng table kapag wala pa (hindi na kailangan i-run ang .sql file).
                const string create =
                    "CREATE TABLE IF NOT EXISTS activities (" +
                    " id INT AUTO_INCREMENT PRIMARY KEY," +
                    " user_id INT NOT NULL," +
                    " title VARCHAR(150) NOT NULL," +
                    " subject VARCHAR(100) NOT NULL," +
                    " priority VARCHAR(10) NOT NULL DEFAULT 'Medium'," +
                    " deadline DATE NOT NULL," +
                    " status VARCHAR(15) NOT NULL DEFAULT 'Pending'," +
                    " notes TEXT NULL," +
                    " created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP," +
                    " completed_at DATETIME NULL," +
                    " INDEX idx_user_status (user_id, status))";
                using var cmd = new MySqlCommand(create, conn);
                cmd.ExecuteNonQuery();
                _tableReady = true;
            }
            return conn;
        }

        public static List<Activity> GetByStatus(string status)
        {
            string order = status == Completed ? "completed_at DESC, id DESC" : "deadline ASC, id ASC";
            string sql =
                "SELECT id, title, subject, priority, deadline, status, notes " +
                "FROM activities WHERE user_id = @u AND status = @s ORDER BY " + order;

            var list = new List<Activity>();
            using var conn = Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", UserId);
            cmd.Parameters.AddWithValue("@s", status);

            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new Activity
                {
                    Id       = r.GetInt32("id"),
                    Title    = r.GetString("title"),
                    Subject  = r.GetString("subject"),
                    Priority = r.GetString("priority"),
                    Deadline = r.GetDateTime("deadline"),
                    Status   = r.GetString("status"),
                    Notes    = r.IsDBNull(r.GetOrdinal("notes")) ? "" : r.GetString("notes")
                });
            }
            return list;
        }

        public static void Add(Activity a)
        {
            const string sql =
                "INSERT INTO activities (user_id, title, subject, priority, deadline, status, notes, completed_at) " +
                "VALUES (@u, @t, @sub, @p, @d, @st, @n, IF(@st = 'Completed', NOW(), NULL))";
            using var conn = Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", UserId);
            Fill(cmd, a);
            cmd.ExecuteNonQuery();
            a.Id = (int)cmd.LastInsertedId;
        }

        public static void Update(Activity a)
        {
            const string sql =
                "UPDATE activities SET title=@t, subject=@sub, priority=@p, deadline=@d, status=@st, notes=@n, " +
                "completed_at = IF(@st = 'Completed', COALESCE(completed_at, NOW()), NULL) " +
                "WHERE id=@id AND user_id=@u";
            using var conn = Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@u", UserId);
            cmd.Parameters.AddWithValue("@id", a.Id);
            Fill(cmd, a);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Gamit sa "Done" (Pending -> Completed) at "Reopen" (Completed -> Pending).</summary>
        public static void SetStatus(int id, string status)
        {
            const string sql =
                "UPDATE activities SET status=@s, completed_at = IF(@s = 'Completed', NOW(), NULL) " +
                "WHERE id=@id AND user_id=@u";
            using var conn = Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@s", status);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@u", UserId);
            cmd.ExecuteNonQuery();
        }

        public static void Delete(int id)
        {
            const string sql = "DELETE FROM activities WHERE id=@id AND user_id=@u";
            using var conn = Open();
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@u", UserId);
            cmd.ExecuteNonQuery();
        }

        private static void Fill(MySqlCommand cmd, Activity a)
        {
            cmd.Parameters.AddWithValue("@t", a.Title);
            cmd.Parameters.AddWithValue("@sub", a.Subject);
            cmd.Parameters.AddWithValue("@p", a.Priority);
            cmd.Parameters.AddWithValue("@d", a.Deadline.Date);
            cmd.Parameters.AddWithValue("@st", a.Status);
            cmd.Parameters.AddWithValue("@n", string.IsNullOrWhiteSpace(a.Notes) ? DBNull.Value : a.Notes);
        }
    }

    /// <summary>Mga maliit na tulong na ginagamit ng My Activities at Completed (itsura ng rows at pager).</summary>
    internal static class ActivityUi
    {
        private static readonly Color Red = Color.FromArgb(192, 57, 43);
        private static readonly Color Orange = Color.FromArgb(211, 133, 0);

        public static Color DeleteColor => Red;

        public static void ShowError(Exception ex)
        {
            string msg = ex is MySqlException
                ? "Hindi makakonekta sa database.\n\nI-check kung naka-Start All ang Laragon.\n\nDetalye: " + ex.Message
                : ex.Message;
            MessageBox.Show(msg, "Academic Planner", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void ShowRow(Activity a, Guna2HtmlLabel title, Guna2HtmlLabel subject,
                                   Guna2HtmlLabel priority, Guna2HtmlLabel deadline, Guna2HtmlLabel status)
        {
            title.Text    = Safe(a.Title, 18);
            subject.Text  = Safe(a.Subject, 18);
            priority.Text = a.Priority;
            deadline.Text = a.Deadline.ToString("MMM dd, yyyy");

            priority.ForeColor = a.Priority switch
            {
                "High" => Red,
                "Medium" => Orange,
                _ => Theme.Accent
            };

            if (a.Status == ActivityRepo.Completed)
            {
                status.Text = "Completed";
                status.ForeColor = Theme.Accent;
            }
            else if (a.Deadline.Date < DateTime.Today)
            {
                status.Text = "Overdue";
                status.ForeColor = Red;
            }
            else
            {
                status.Text = "Pending";
                status.ForeColor = Orange;
            }
        }

        /// <summary>Ina-update ang Previous / Next at ang mga numero ng page (3 buttons ang kasya).</summary>
        public static void UpdatePager(int page, int totalPages, Guna2Button prev, Guna2Button next, Guna2Button[] numbers)
        {
            int first = (page / numbers.Length) * numbers.Length;   // simula ng "window" ng 3 numero
            for (int k = 0; k < numbers.Length; k++)
            {
                int p = first + k;
                numbers[k].Visible = p < totalPages;
                numbers[k].Text = (p + 1).ToString();
                numbers[k].Tag = p;
                numbers[k].FillColor = p == page ? Theme.Accent : Theme.CardAlt;
                numbers[k].ForeColor = p == page ? Color.White : Theme.Accent;
            }
            prev.Enabled = page > 0;
            next.Enabled = page < totalPages - 1;
        }

        // Guna2HtmlLabel ay nagre-render ng HTML, kaya i-encode para hindi masira ng "<" o "&".
        private static string Safe(string text, int max)
        {
            if (text.Length > max) text = text.Substring(0, max - 1) + "…";
            return WebUtility.HtmlEncode(text);
        }
    }
}
