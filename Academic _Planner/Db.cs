using Microsoft.Data.SqlClient;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Academic__Planner
{
    public static class Db
    {
        public const string ConnectionString =
            "Server=DESKTOP-7C194HH\\SQLEXPRESS;Database=AcademicPlannerDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToHexString(bytes);
            }
        }

        // Dinadagdag nito ang Subject at Notes columns sa Activities kung wala pa
        static bool _schemaChecked;
        public static void EnsureSchema()
        {
            if (_schemaChecked) return;

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    @"IF COL_LENGTH('dbo.Activities','Subject') IS NULL ALTER TABLE dbo.Activities ADD Subject NVARCHAR(100) NULL;
                      IF COL_LENGTH('dbo.Activities','Notes')   IS NULL ALTER TABLE dbo.Activities ADD Notes   NVARCHAR(500) NULL;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            _schemaChecked = true;
        }

        static string Str(SqlDataReader r, int i) => r.IsDBNull(i) ? "" : r.GetString(i);

        // Kinukuha ang activities ng naka-login na user mula sa database
        public static void LoadActivitiesForUser(int userId)
        {
            EnsureSchema();
            ActivityStore.Items.Clear();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT ActivityId, Title, Difficulty, StartDate, EndDate, IsDone, Subject, Notes
                      FROM Activities
                      WHERE UserId = @uid
                      ORDER BY EndDate, ActivityId", conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ActivityStore.Items.Add(new Activity
                            {
                                Id = reader.GetInt32(0),
                                Title = Str(reader, 1),
                                Difficulty = Str(reader, 2),
                                Start = reader.GetDateTime(3),
                                End = reader.GetDateTime(4),
                                IsCompleted = reader.GetBoolean(5),
                                Subject = Str(reader, 6),
                                Notes = Str(reader, 7)
                            });
                        }
                    }
                }
            }
        }

        static void Run(string sql, Action<SqlCommand> addParams)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@uid", Session.UserId);
                    addParams(cmd);
                    cmd.ExecuteNonQuery();
                }
            }
            LoadActivitiesForUser(Session.UserId);   // para pareho na ang lahat ng forms
        }

        // BAGONG ACTIVITY
        public static void AddActivity(Activity a)
        {
            EnsureSchema();
            Run(@"INSERT INTO Activities (UserId, Title, Difficulty, StartDate, EndDate, IsDone, Subject, Notes)
                  VALUES (@uid, @title, @diff, @start, @end, @done, @subj, @notes)", c =>
            {
                c.Parameters.AddWithValue("@title", a.Title);
                c.Parameters.AddWithValue("@diff", a.Difficulty);
                c.Parameters.AddWithValue("@start", a.Start);
                c.Parameters.AddWithValue("@end", a.End);
                c.Parameters.AddWithValue("@done", a.IsCompleted);
                c.Parameters.AddWithValue("@subj", a.Subject);
                c.Parameters.AddWithValue("@notes", a.Notes);
            });
        }

        // I-EDIT ANG ACTIVITY
        public static void UpdateActivity(Activity a)
        {
            Run(@"UPDATE Activities
                  SET Title = @title, Difficulty = @diff, EndDate = @end, IsDone = @done, Subject = @subj, Notes = @notes
                  WHERE ActivityId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", a.Id);
                c.Parameters.AddWithValue("@title", a.Title);
                c.Parameters.AddWithValue("@diff", a.Difficulty);
                c.Parameters.AddWithValue("@end", a.End);
                c.Parameters.AddWithValue("@done", a.IsCompleted);
                c.Parameters.AddWithValue("@subj", a.Subject);
                c.Parameters.AddWithValue("@notes", a.Notes);
            });
        }

        // DONE / REOPEN
        public static void SetDone(int activityId, bool done)
        {
            Run("UPDATE Activities SET IsDone = @done WHERE ActivityId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", activityId);
                c.Parameters.AddWithValue("@done", done);
            });
        }

        public static void MarkActivityDone(int activityId) => SetDone(activityId, true);

        // DELETE
        public static void DeleteActivity(int activityId)
        {
            Run("DELETE FROM Activities WHERE ActivityId = @id AND UserId = @uid", c =>
            {
                c.Parameters.AddWithValue("@id", activityId);
            });
        }
    }
}
