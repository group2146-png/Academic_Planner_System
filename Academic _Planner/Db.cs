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

        // Kinukuha ang activities ng naka-login na user mula sa database
        public static void LoadActivitiesForUser(int userId)
        {
            ActivityStore.Items.Clear();

            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    @"SELECT ActivityId, Title, Difficulty, StartDate, EndDate, IsDone
                      FROM Activities
                      WHERE UserId = @uid
                      ORDER BY StartDate", conn))
                {
                    cmd.Parameters.AddWithValue("@uid", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ActivityStore.Items.Add(new Activity
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Difficulty = reader.GetString(2),
                                Start = reader.GetDateTime(3),
                                End = reader.GetDateTime(4),
                                IsCompleted = reader.GetBoolean(5)
                            });
                        }
                    }
                }
            }
        }

        // Sine-save sa database na tapos na ang activity
        public static void MarkActivityDone(int activityId)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(
                    "UPDATE Activities SET IsDone = 1 WHERE ActivityId = @id AND UserId = @uid", conn))
                {
                    cmd.Parameters.AddWithValue("@id", activityId);
                    cmd.Parameters.AddWithValue("@uid", Session.UserId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}