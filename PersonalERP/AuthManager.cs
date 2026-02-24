using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using PERP_API;

namespace PersonalERP_Server
{
    internal static class AuthManager
    {
        private static SqliteConnection sql;

        public static void Initialize(SqliteConnection conn) { sql = conn; }

        public static string GenerateSalt()
        {
            byte[] bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(salt + password));
                return Convert.ToBase64String(bytes);
            }
        }

        public static string Authenticate(string username, string password)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT id, password_hash, salt, is_active FROM perp_users WHERE username = $u";
                cmd.Parameters.AddWithValue("$u", username);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    if (r.GetInt32(3) == 0) return null;
                    string storedHash = r.GetString(1);
                    string salt = r.GetString(2);
                    if (HashPassword(password, salt) != storedHash) return null;

                    int userId = r.GetInt32(0);
                    r.Close();

                    string token = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
                    string now = DateTime.UtcNow.ToString("o");
                    string exp = DateTime.UtcNow.AddHours(24).ToString("o");

                    var ins = sql.CreateCommand();
                    ins.CommandText = "INSERT INTO perp_sessions (user_id,session_token,created_date,expires_date) VALUES ($uid,$tok,$now,$exp)";
                    ins.Parameters.AddWithValue("$uid", userId);
                    ins.Parameters.AddWithValue("$tok", token);
                    ins.Parameters.AddWithValue("$now", now);
                    ins.Parameters.AddWithValue("$exp", exp);
                    ins.ExecuteNonQuery();
                    return token;
                }
            }
            finally { sql.Close(); }
        }

        public static bool ValidateSession(string token)
        {
            if (string.IsNullOrEmpty(token)) return false;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT expires_date FROM perp_sessions WHERE session_token = $t";
                cmd.Parameters.AddWithValue("$t", token);
                object val = cmd.ExecuteScalar();
                if (val == null) return false;
                return DateTime.Parse(val.ToString()) > DateTime.UtcNow;
            }
            finally { sql.Close(); }
        }

        public static void Logout(string token)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "DELETE FROM perp_sessions WHERE session_token = $t";
                cmd.Parameters.AddWithValue("$t", token);
                cmd.ExecuteNonQuery();
            }
            finally { sql.Close(); }
        }

        public static UserInfo GetCurrentUser(string token)
        {
            if (!ValidateSession(token)) return null;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"SELECT u.id, u.username, u.display_name, u.role, u.is_active
                    FROM perp_users u JOIN perp_sessions s ON u.id = s.user_id
                    WHERE s.session_token = $t";
                cmd.Parameters.AddWithValue("$t", token);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new UserInfo
                    {
                        Id = r.GetInt32(0),
                        Username = r.GetString(1),
                        DisplayName = r.GetString(2),
                        Role = r.GetString(3),
                        IsActive = r.GetInt32(4) == 1
                    };
                }
            }
            finally { sql.Close(); }
        }

        public static bool ChangePassword(string token, string oldPw, string newPw)
        {
            UserInfo user = GetCurrentUser(token);
            if (user == null) return false;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT password_hash, salt FROM perp_users WHERE id = $id";
                cmd.Parameters.AddWithValue("$id", user.Id);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return false;
                    if (HashPassword(oldPw, r.GetString(1)) != r.GetString(0)) return false;
                }

                string newSalt = GenerateSalt();
                string newHash = HashPassword(newPw, newSalt);
                var upd = sql.CreateCommand();
                upd.CommandText = "UPDATE perp_users SET password_hash=$h, salt=$s WHERE id=$id";
                upd.Parameters.AddWithValue("$h", newHash);
                upd.Parameters.AddWithValue("$s", newSalt);
                upd.Parameters.AddWithValue("$id", user.Id);
                return upd.ExecuteNonQuery() > 0;
            }
            finally { sql.Close(); }
        }

        public static bool IsAdmin(string token)
        {
            var u = GetCurrentUser(token);
            return u != null && u.Role == "Admin";
        }

        public static List<UserInfo> ListUsers(string token)
        {
            if (!IsAdmin(token)) return new List<UserInfo>();
            sql.Open();
            try
            {
                var list = new List<UserInfo>();
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT id, username, display_name, role, is_active FROM perp_users ORDER BY id";
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new UserInfo
                        {
                            Id = r.GetInt32(0), Username = r.GetString(1),
                            DisplayName = r.GetString(2), Role = r.GetString(3),
                            IsActive = r.GetInt32(4) == 1
                        });
                return list;
            }
            finally { sql.Close(); }
        }

        public static bool CreateUser(string token, string username, string password,
                                       string displayName, string role)
        {
            if (!IsAdmin(token)) return false;
            sql.Open();
            try
            {
                string salt = GenerateSalt();
                string hash = HashPassword(password, salt);
                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"INSERT INTO perp_users
                    (username,password_hash,salt,display_name,role,created_date,is_active)
                    VALUES ($u,$h,$s,$d,$r,$n,1)";
                cmd.Parameters.AddWithValue("$u", username);
                cmd.Parameters.AddWithValue("$h", hash);
                cmd.Parameters.AddWithValue("$s", salt);
                cmd.Parameters.AddWithValue("$d", displayName);
                cmd.Parameters.AddWithValue("$r", role);
                cmd.Parameters.AddWithValue("$n", now);
                return cmd.ExecuteNonQuery() > 0;
            }
            catch { return false; }
            finally { sql.Close(); }
        }

        public static bool SetUserActive(string token, int userId, bool active)
        {
            if (!IsAdmin(token)) return false;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "UPDATE perp_users SET is_active=$a WHERE id=$id";
                cmd.Parameters.AddWithValue("$a", active ? 1 : 0);
                cmd.Parameters.AddWithValue("$id", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally { sql.Close(); }
        }

        public static bool UpdateUserRole(string token, int userId, string role)
        {
            if (!IsAdmin(token)) return false;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "UPDATE perp_users SET role=$r WHERE id=$id";
                cmd.Parameters.AddWithValue("$r", role);
                cmd.Parameters.AddWithValue("$id", userId);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally { sql.Close(); }
        }

        public static bool HasPermission(string token, string moduleName, string permType)
        {
            UserInfo user = GetCurrentUser(token);
            if (user == null) return false;
            if (user.Role == "Admin") return true;

            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                string col;
                switch (permType.ToLower())
                {
                    case "edit":  col = "can_edit";  break;
                    case "admin": col = "can_admin"; break;
                    default:      col = "can_view";  break;
                }
                cmd.CommandText = $"SELECT {col} FROM perp_module_permissions WHERE role=$r AND module_name=$m";
                cmd.Parameters.AddWithValue("$r", user.Role);
                cmd.Parameters.AddWithValue("$m", moduleName);
                object val = cmd.ExecuteScalar();
                return val != null && Convert.ToInt32(val) == 1;
            }
            finally { sql.Close(); }
        }

        public static List<ModulePermission> GetPermissionsForRole(string token, string role)
        {
            if (!IsAdmin(token)) return new List<ModulePermission>();
            sql.Open();
            try
            {
                var list = new List<ModulePermission>();
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT role, module_name, can_view, can_edit, can_admin FROM perp_module_permissions WHERE role=$r";
                cmd.Parameters.AddWithValue("$r", role);
                using (var r = cmd.ExecuteReader())
                    while (r.Read())
                        list.Add(new ModulePermission
                        {
                            Role = r.GetString(0), ModuleName = r.GetString(1),
                            CanView = r.GetInt32(2)==1, CanEdit = r.GetInt32(3)==1,
                            CanAdmin = r.GetInt32(4)==1
                        });
                return list;
            }
            finally { sql.Close(); }
        }

        public static bool SetPermission(string token, string role, string moduleName,
                                          bool canView, bool canEdit, bool canAdmin)
        {
            if (!IsAdmin(token)) return false;
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"INSERT OR REPLACE INTO perp_module_permissions
                    (role,module_name,can_view,can_edit,can_admin) VALUES ($r,$m,$v,$e,$a)";
                cmd.Parameters.AddWithValue("$r", role);
                cmd.Parameters.AddWithValue("$m", moduleName);
                cmd.Parameters.AddWithValue("$v", canView ? 1 : 0);
                cmd.Parameters.AddWithValue("$e", canEdit ? 1 : 0);
                cmd.Parameters.AddWithValue("$a", canAdmin ? 1 : 0);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally { sql.Close(); }
        }
    }
}
