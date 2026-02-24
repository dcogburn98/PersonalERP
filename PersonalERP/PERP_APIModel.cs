using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;

using PERP_API;

namespace PersonalERP_Server
{
    internal class PERP_APIModel : PERP_API_Contract
    {
        public static SqliteConnection sql;

        public static void Initialize(SqliteConnection sql_conn)
        {
            sql = sql_conn;
        }

        // ── Logging ──────────────────────────────────────────────

        public void Log(string Message)
        {
            Console.WriteLine(Message);
        }

        // ── Legacy database ──────────────────────────────────────

        public List<string> DB_ListTables()
        {
            sql.Open();
            try
            {
                List<string> names = new List<string>();
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY 1";
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        names.Add(r.GetValue(0).ToString());
                return names;
            }
            finally { sql.Close(); }
        }

        public DataTable DB_GetTableSchema(string tableName)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = $"SELECT * FROM {tableName}";
                using (var r = cmd.ExecuteReader())
                {
                    var dt = new DataTable(tableName);
                    dt.Load(r);
                    return dt;
                }
            }
            finally { sql.Close(); }
        }

        public int DB_ExecuteNonQuery(string sqlText)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = sqlText;
                return cmd.ExecuteNonQuery();
            }
            finally { sql.Close(); }
        }

        public DataTable DB_ExecuteQuery(string sqlText)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = sqlText;
                using (var r = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(r);
                    return dt;
                }
            }
            finally { sql.Close(); }
        }

        // ── Authentication ───────────────────────────────────────

        public string Authenticate(string username, string password)
        {
            return AuthManager.Authenticate(username, password);
        }

        public bool ValidateSession(string sessionToken)
        {
            return AuthManager.ValidateSession(sessionToken);
        }

        public void Logout(string sessionToken)
        {
            AuthManager.Logout(sessionToken);
        }

        public UserInfo GetCurrentUser(string sessionToken)
        {
            return AuthManager.GetCurrentUser(sessionToken);
        }

        public bool ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            return AuthManager.ChangePassword(sessionToken, oldPassword, newPassword);
        }

        // ── User management ──────────────────────────────────────

        public List<UserInfo> ListUsers(string sessionToken)
        {
            return AuthManager.ListUsers(sessionToken);
        }

        public bool CreateUser(string sessionToken, string username, string password,
                               string displayName, string role)
        {
            return AuthManager.CreateUser(sessionToken, username, password, displayName, role);
        }

        public bool SetUserActive(string sessionToken, int userId, bool isActive)
        {
            return AuthManager.SetUserActive(sessionToken, userId, isActive);
        }

        public bool UpdateUserRole(string sessionToken, int userId, string newRole)
        {
            return AuthManager.UpdateUserRole(sessionToken, userId, newRole);
        }

        // ── Permissions ──────────────────────────────────────────

        public bool HasPermission(string sessionToken, string moduleName, string permissionType)
        {
            return AuthManager.HasPermission(sessionToken, moduleName, permissionType);
        }

        public List<ModulePermission> GetPermissionsForRole(string sessionToken, string role)
        {
            return AuthManager.GetPermissionsForRole(sessionToken, role);
        }

        public bool SetPermission(string sessionToken, string role, string moduleName,
                                  bool canView, bool canEdit, bool canAdmin)
        {
            return AuthManager.SetPermission(sessionToken, role, moduleName, canView, canEdit, canAdmin);
        }

        // ── Secure database ──────────────────────────────────────

        public DataTable DB_QuerySecure(string sessionToken, string sqlText,
                                        string[] paramNames, string[] paramValues)
        {
            if (!AuthManager.ValidateSession(sessionToken))
                throw new Exception("Invalid session");

            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = sqlText;
                if (paramNames != null)
                    for (int i = 0; i < paramNames.Length && i < paramValues.Length; i++)
                        cmd.Parameters.AddWithValue(paramNames[i], paramValues[i]);

                using (var r = cmd.ExecuteReader())
                {
                    var dt = new DataTable();
                    dt.Load(r);
                    return dt;
                }
            }
            finally { sql.Close(); }
        }

        public int DB_ExecuteSecure(string sessionToken, string sqlText,
                                    string[] paramNames, string[] paramValues)
        {
            if (!AuthManager.ValidateSession(sessionToken))
                throw new Exception("Invalid session");

            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = sqlText;
                if (paramNames != null)
                    for (int i = 0; i < paramNames.Length && i < paramValues.Length; i++)
                        cmd.Parameters.AddWithValue(paramNames[i], paramValues[i]);
                return cmd.ExecuteNonQuery();
            }
            finally { sql.Close(); }
        }

        // ── Module settings ──────────────────────────────────────

        public string GetModuleSetting(string moduleName, string key)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT setting_value FROM perp_module_settings WHERE module_name=$m AND setting_key=$k";
                cmd.Parameters.AddWithValue("$m", moduleName);
                cmd.Parameters.AddWithValue("$k", key);
                object val = cmd.ExecuteScalar();
                return val?.ToString();
            }
            finally { sql.Close(); }
        }

        public void SetModuleSetting(string sessionToken, string moduleName, string key, string value)
        {
            if (!AuthManager.ValidateSession(sessionToken)) return;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO perp_module_settings
                    (module_name, setting_key, setting_value) VALUES ($m,$k,$v)";
                cmd.Parameters.AddWithValue("$m", moduleName);
                cmd.Parameters.AddWithValue("$k", key);
                cmd.Parameters.AddWithValue("$v", value);
                cmd.ExecuteNonQuery();
            }
            finally { sql.Close(); }
        }

        public DataTable GetAllModuleSettings(string moduleName)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT setting_key AS [Key], setting_value AS Value FROM perp_module_settings WHERE module_name=$m";
                cmd.Parameters.AddWithValue("$m", moduleName);
                var dt = new DataTable();
                using (var r = cmd.ExecuteReader()) dt.Load(r);
                return dt;
            }
            finally { sql.Close(); }
        }

        // ── Scheduled tasks ──────────────────────────────────────

        public int RegisterScheduledTask(string sessionToken, string taskName,
            string moduleName, string description, string actionType,
            string actionData, int intervalSeconds)
        {
            if (!AuthManager.ValidateSession(sessionToken)) return -1;
            UserInfo user = AuthManager.GetCurrentUser(sessionToken);
            return TaskSchedulerEngine.Register(taskName, moduleName, description,
                actionType, actionData, intervalSeconds, user?.Id);
        }

        public bool UnregisterScheduledTask(string sessionToken, int taskId)
        {
            if (!AuthManager.HasPermission(sessionToken, "TaskScheduler", "admin")) return false;
            return TaskSchedulerEngine.Unregister(taskId);
        }

        public bool SetTaskEnabled(string sessionToken, int taskId, bool enabled)
        {
            if (!AuthManager.HasPermission(sessionToken, "TaskScheduler", "edit")) return false;
            return TaskSchedulerEngine.SetEnabled(taskId, enabled);
        }

        public DataTable ListScheduledTasks(string sessionToken)
        {
            if (!AuthManager.ValidateSession(sessionToken)) return new DataTable();
            return TaskSchedulerEngine.List();
        }

        public DataTable GetTaskHistory(string sessionToken, int taskId, int limit)
        {
            if (!AuthManager.ValidateSession(sessionToken)) return new DataTable();
            return TaskSchedulerEngine.History(taskId, limit);
        }

        public bool RunTaskNow(string sessionToken, int taskId)
        {
            if (!AuthManager.HasPermission(sessionToken, "TaskScheduler", "admin")) return false;
            return TaskSchedulerEngine.RunNow(taskId);
        }
    }
}
