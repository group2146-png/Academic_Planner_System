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

    internal enum RegisterResult { Success, UsernameTaken }

    internal static class Db
    {
        // Default ng Laragon: user = root, walang password, port 3306.
        // Kung may nilagay kang password sa MySQL, ilagay sa Password=
        private const string ConnectionString =
            "Server=127.0.0.1;Port=3306;Database=academic_planner;" +
            "User ID=root;Password=;SslMode=None;AllowPublicKeyRetrieval=True;";

        public static MySqlConnection GetConnection() => new MySqlConnection(ConnectionString);

        private static bool _dbReady;
        private static bool _usersReady;

        // Gagawa ng database na academic_planner kung wala pa
        private static async Task EnsureDatabaseAsync()
        {
            if (_dbReady) return;

            var csb = new MySqlConnectionStringBuilder(ConnectionString);
            csb.Remove("Database");   // kumonekta muna sa MySQL server nang walang database

            await using var conn = new MySqlConnection(csb.ConnectionString);
            await conn.OpenAsync();
            await using var cmd = new MySqlCommand(
                "CREATE DATABASE IF NOT EXISTS academic_planner CHARACTER SET utf8mb4", conn);
            await cmd.ExecuteNonQueryAsync();

            _dbReady = true;
        }

        // Gagawa ng users table kung wala pa
        private static async Task EnsureUsersTableAsync(MySqlConnection conn)
        {
            if (_usersReady) return;

            const string create =
                "CREATE TABLE IF NOT EXISTS users (" +
                " id INT AUTO_INCREMENT PRIMARY KEY," +
                " username VARCHAR(50) NOT NULL UNIQUE," +
                " full_name VARCHAR(100) NOT NULL," +
                " password_hash VARCHAR(255) NOT NULL," +
                " created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP)";

            await using var cmd = new MySqlCommand(create, conn);
            await cmd.ExecuteNonQueryAsync();

            _usersReady = true;
        }

        /// <summary>Gumagawa ng bagong account. Naka-hash ang password.</summary>
        public static async Task<RegisterResult> RegisterAsync(string username, string password)
        {
            await EnsureDatabaseAsync();

            await using var conn = GetConnection();
            await conn.OpenAsync();
            await EnsureUsersTableAsync(conn);

            // may gumagamit na ba ng username na ito?
            await using (var check = new MySqlCommand(
                "SELECT 1 FROM users WHERE username = @username LIMIT 1", conn))
            {
                check.Parameters.AddWithValue("@username", username);
                if (await check.ExecuteScalarAsync() != null)
                    return RegisterResult.UsernameTaken;
            }

            await using var insert = new MySqlCommand(
                "INSERT INTO users (username, full_name, password_hash) " +
                "VALUES (@username, @fullName, @hash)", conn);
            insert.Parameters.AddWithValue("@username", username);
            insert.Parameters.AddWithValue("@fullName", username);   // walang Full Name field sa form, username muna
            insert.Parameters.AddWithValue("@hash", PasswordHelper.Hash(password));

            try
            {
                await insert.ExecuteNonQueryAsync();
            }
            catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.DuplicateKeyEntry)
            {
                return RegisterResult.UsernameTaken;
            }

            return RegisterResult.Success;
        }

        /// <summary>
        /// Hinahanap ang user at chine-check ang password.
        /// Ibabalik ang UserInfo kapag tama, null kapag mali ang username o password.
        /// </summary>
        public static async Task<UserInfo?> AuthenticateAsync(string username, string password)
        {
            const string sql =
                "SELECT id, username, full_name, password_hash " +
                "FROM users WHERE username = @username LIMIT 1";

            await EnsureDatabaseAsync();

            await using var conn = GetConnection();
            await conn.OpenAsync();
            await EnsureUsersTableAsync(conn);

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