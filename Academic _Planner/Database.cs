using MySqlConnector;

namespace Academic__Planner
{
    /// <summary>Impormasyon ng user na naka-login.</summary>
    internal record UserInfo(int Id, string Username, string FullName);

    /// <summary>Dito naka-save kung sino ang naka-login (magagamit sa ibang form).</summary>
    internal static class Session
    {
        public static UserInfo? CurrentUser { get; set; }
    }

    internal static class Db
    {
        // Default ng Laragon: user = root, walang password, port 3306.
        // Kung may nilagay kang password sa MySQL, ilagay sa Password=
        private const string ConnectionString =
            "Server=127.0.0.1;Port=3306;Database=academic_planner;" +
            "User ID=root;Password=;SslMode=None;AllowPublicKeyRetrieval=True;";

        public static MySqlConnection GetConnection() => new MySqlConnection(ConnectionString);

        /// <summary>
        /// Hinahanap ang user at chine-check ang password.
        /// Ibabalik ang UserInfo kapag tama, null kapag mali ang username o password.
        /// </summary>
        public static async Task<UserInfo?> AuthenticateAsync(string username, string password)
        {
            const string sql =
                "SELECT id, username, full_name, password_hash " +
                "FROM users WHERE username = @username LIMIT 1";

            await using var conn = GetConnection();
            await conn.OpenAsync();

            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);   // parameterized: ligtas sa SQL injection

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
                return null;

            string storedHash = reader.GetString("password_hash");
            if (!PasswordHelper.Verify(password, storedHash))
                return null;

            return new UserInfo(
                reader.GetInt32("id"),
                reader.GetString("username"),
                reader.GetString("full_name"));
        }
    }
}
