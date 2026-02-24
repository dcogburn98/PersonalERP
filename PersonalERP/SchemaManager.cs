using System;
using Microsoft.Data.Sqlite;

namespace PersonalERP_Server
{
    internal static class SchemaManager
    {
        public static void InitializeSchema(SqliteConnection sql)
        {
            sql.Open();
            try
            {
                Execute(sql, @"CREATE TABLE IF NOT EXISTS perp_users (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    username TEXT UNIQUE NOT NULL,
                    password_hash TEXT NOT NULL,
                    salt TEXT NOT NULL,
                    display_name TEXT NOT NULL,
                    role TEXT NOT NULL DEFAULT 'User',
                    created_date TEXT NOT NULL,
                    is_active INTEGER NOT NULL DEFAULT 1)");

                Execute(sql, @"CREATE TABLE IF NOT EXISTS perp_sessions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    user_id INTEGER NOT NULL,
                    session_token TEXT UNIQUE NOT NULL,
                    created_date TEXT NOT NULL,
                    expires_date TEXT NOT NULL,
                    FOREIGN KEY (user_id) REFERENCES perp_users(id))");

                Execute(sql, @"CREATE TABLE IF NOT EXISTS perp_module_permissions (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    role TEXT NOT NULL,
                    module_name TEXT NOT NULL,
                    can_view INTEGER NOT NULL DEFAULT 1,
                    can_edit INTEGER NOT NULL DEFAULT 0,
                    can_admin INTEGER NOT NULL DEFAULT 0,
                    UNIQUE(role, module_name))");

                Execute(sql, @"CREATE TABLE IF NOT EXISTS perp_scheduled_tasks (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    task_name TEXT NOT NULL,
                    module_name TEXT NOT NULL,
                    description TEXT,
                    action_type TEXT NOT NULL,
                    action_data TEXT NOT NULL,
                    interval_seconds INTEGER NOT NULL,
                    is_enabled INTEGER NOT NULL DEFAULT 1,
                    last_run TEXT,
                    next_run TEXT,
                    created_by INTEGER,
                    created_date TEXT NOT NULL)");

                Execute(sql, @"CREATE TABLE IF NOT EXISTS perp_task_history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    task_id INTEGER NOT NULL,
                    run_date TEXT NOT NULL,
                    status TEXT NOT NULL,
                    result_message TEXT,
                    duration_ms INTEGER,
                    FOREIGN KEY (task_id) REFERENCES perp_scheduled_tasks(id))");

                Execute(sql, @"CREATE TABLE IF NOT EXISTS perp_module_settings (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    module_name TEXT NOT NULL,
                    setting_key TEXT NOT NULL,
                    setting_value TEXT,
                    UNIQUE(module_name, setting_key))");

                Console.WriteLine("Database schema initialized.");
            }
            finally { sql.Close(); }
        }

        public static void SeedDefaults(SqliteConnection sql)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM perp_users";
                long count = (long)cmd.ExecuteScalar();
                if (count > 0) return;

                string salt = AuthManager.GenerateSalt();
                string hash = AuthManager.HashPassword("admin123", salt);
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                Execute(sql, $"INSERT INTO perp_users (username,password_hash,salt,display_name,role,created_date,is_active) " +
                    $"VALUES ('admin','{hash}','{salt}','Administrator','Admin','{now}',1)");

                salt = AuthManager.GenerateSalt();
                hash = AuthManager.HashPassword("user123", salt);
                Execute(sql, $"INSERT INTO perp_users (username,password_hash,salt,display_name,role,created_date,is_active) " +
                    $"VALUES ('user','{hash}','{salt}','Standard User','User','{now}',1)");

                salt = AuthManager.GenerateSalt();
                hash = AuthManager.HashPassword("viewer123", salt);
                Execute(sql, $"INSERT INTO perp_users (username,password_hash,salt,display_name,role,created_date,is_active) " +
                    $"VALUES ('viewer','{hash}','{salt}','Read-Only User','Viewer','{now}',1)");

                string[] modules = { "DBBrowser", "Accounting", "TaskManager", "TaskScheduler" };
                foreach (string mod in modules)
                {
                    Execute(sql, $"INSERT OR IGNORE INTO perp_module_permissions (role,module_name,can_view,can_edit,can_admin) VALUES ('Admin','{mod}',1,1,1)");
                }
                foreach (string mod in new[] { "DBBrowser", "Accounting", "TaskManager" })
                {
                    Execute(sql, $"INSERT OR IGNORE INTO perp_module_permissions (role,module_name,can_view,can_edit,can_admin) VALUES ('User','{mod}',1,1,0)");
                }
                Execute(sql, "INSERT OR IGNORE INTO perp_module_permissions (role,module_name,can_view,can_edit,can_admin) VALUES ('User','TaskScheduler',1,0,0)");

                foreach (string mod in new[] { "DBBrowser", "Accounting", "TaskManager" })
                {
                    Execute(sql, $"INSERT OR IGNORE INTO perp_module_permissions (role,module_name,can_view,can_edit,can_admin) VALUES ('Viewer','{mod}',1,0,0)");
                }

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Default users created:  admin/admin123  user/user123  viewer/viewer123");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally { sql.Close(); }
        }

        private static void Execute(SqliteConnection sql, string text)
        {
            var c = sql.CreateCommand();
            c.CommandText = text;
            c.ExecuteNonQuery();
        }
    }
}
