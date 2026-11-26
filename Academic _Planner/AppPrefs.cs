using System.IO;

namespace Academic__Planner
{
    // Mga setting na naka-save sa PC na ito (hindi sa database):
    //   - Dark / Light mode
    //   - "Remember Me" (username + token, HINDI ang password)
    // Folder: %AppData%\AcademicPlanner
    public static class AppPrefs
    {
        static readonly string Dir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AcademicPlanner");

        static readonly string SettingsFile = Path.Combine(Dir, "settings.ini");
        static readonly string RememberFile = Path.Combine(Dir, "remember.dat");

        // ---------- GENERAL SETTINGS (key=value) ----------
        static Dictionary<string, string>? _cache;

        static Dictionary<string, string> Read()
        {
            if (_cache != null) return _cache;
            var d = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (File.Exists(SettingsFile))
                    foreach (string line in File.ReadAllLines(SettingsFile))
                    {
                        int i = line.IndexOf('=');
                        if (i > 0) d[line.Substring(0, i).Trim()] = line.Substring(i + 1).Trim();
                    }
            }
            catch { }
            _cache = d;
            return d;
        }

        static void Write()
        {
            try
            {
                Directory.CreateDirectory(Dir);
                File.WriteAllLines(SettingsFile, Read().Select(kv => kv.Key + "=" + kv.Value));
            }
            catch { }
        }

        public static string Get(string key, string def) =>
            Read().TryGetValue(key, out string? v) ? v : def;

        public static void Set(string key, string value)
        {
            Read()[key] = value;
            Write();
        }

        public static bool GetBool(string key, bool def)
        {
            string v = Get(key, def ? "1" : "0");
            return v == "1" || v.Equals("true", StringComparison.OrdinalIgnoreCase);
        }

        public static void SetBool(string key, bool value) => Set(key, value ? "1" : "0");

        public static int GetInt(string key, int def) =>
            int.TryParse(Get(key, def.ToString()), out int n) ? n : def;

        public static void SetInt(string key, int value) => Set(key, value.ToString());

        // ---------- DARK MODE ----------
        public static bool LoadDark() => GetBool("dark", false);

        public static void SaveDark(bool dark) => SetBool("dark", dark);

        // ---------- MGA SETTING NG APP ----------
        public static bool ReminderAlerts { get => GetBool("reminderAlerts", true); set => SetBool("reminderAlerts", value); }
        public static bool ReminderSound  { get => GetBool("reminderSound", true);  set => SetBool("reminderSound", value); }
        public static bool TimerSound     { get => GetBool("timerSound", true);     set => SetBool("timerSound", value); }
        public static int FocusMinutes    { get => Math.Clamp(GetInt("focusMinutes", 30), 5, 120); set => SetInt("focusMinutes", Math.Clamp(value, 5, 120)); }
        public static int BreakMinutes    { get => Math.Clamp(GetInt("breakMinutes", 5), 1, 60);   set => SetInt("breakMinutes", Math.Clamp(value, 1, 60)); }

        // ---------- REMEMBER ME ----------
        public static void SaveRemember(string username, string token)
        {
            try
            {
                Directory.CreateDirectory(Dir);
                File.WriteAllLines(RememberFile, new[] { username, token });
            }
            catch { }
        }

        public static (string Username, string Token)? LoadRemember()
        {
            try
            {
                if (!File.Exists(RememberFile)) return null;
                string[] lines = File.ReadAllLines(RememberFile);
                if (lines.Length >= 2 && lines[0].Length > 0 && lines[1].Length > 0)
                    return (lines[0], lines[1]);
            }
            catch { }
            return null;
        }

        public static void ClearRemember()
        {
            try { if (File.Exists(RememberFile)) File.Delete(RememberFile); }
            catch { }
        }
    }
}
