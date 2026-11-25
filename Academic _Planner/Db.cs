using System;
using System.Security.Cryptography;
using System.Text;

namespace Academic_Planner
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
    }
}