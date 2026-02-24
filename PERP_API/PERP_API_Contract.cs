using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace PERP_API
{
    [ServiceContract]
    public interface PERP_API_Contract
    {
        // ── Logging ──────────────────────────────────────────────

        [OperationContract]
        void Log(string Message);

        // ── Legacy database (kept for backward compatibility) ────

        [OperationContract]
        List<string> DB_ListTables();

        [OperationContract]
        DataTable DB_GetTableSchema(string TableName);

        [OperationContract]
        int DB_ExecuteNonQuery(string sql);

        [OperationContract]
        DataTable DB_ExecuteQuery(string sql);

        // ── Authentication ───────────────────────────────────────

        [OperationContract]
        string Authenticate(string username, string password);

        [OperationContract]
        bool ValidateSession(string sessionToken);

        [OperationContract]
        void Logout(string sessionToken);

        [OperationContract]
        UserInfo GetCurrentUser(string sessionToken);

        [OperationContract]
        bool ChangePassword(string sessionToken, string oldPassword, string newPassword);

        // ── User management (Admin only) ─────────────────────────

        [OperationContract]
        List<UserInfo> ListUsers(string sessionToken);

        [OperationContract]
        bool CreateUser(string sessionToken, string username, string password,
                        string displayName, string role);

        [OperationContract]
        bool SetUserActive(string sessionToken, int userId, bool isActive);

        [OperationContract]
        bool UpdateUserRole(string sessionToken, int userId, string newRole);

        // ── Permissions ──────────────────────────────────────────

        [OperationContract]
        bool HasPermission(string sessionToken, string moduleName, string permissionType);

        [OperationContract]
        List<ModulePermission> GetPermissionsForRole(string sessionToken, string role);

        [OperationContract]
        bool SetPermission(string sessionToken, string role, string moduleName,
                           bool canView, bool canEdit, bool canAdmin);

        // ── Secure database (requires session) ───────────────────

        [OperationContract]
        DataTable DB_QuerySecure(string sessionToken, string sql,
                                 string[] paramNames, string[] paramValues);

        [OperationContract]
        int DB_ExecuteSecure(string sessionToken, string sql,
                             string[] paramNames, string[] paramValues);

        // ── Module settings ──────────────────────────────────────

        [OperationContract]
        string GetModuleSetting(string moduleName, string key);

        [OperationContract]
        void SetModuleSetting(string sessionToken, string moduleName,
                              string key, string value);

        [OperationContract]
        DataTable GetAllModuleSettings(string moduleName);

        // ── Scheduled tasks ──────────────────────────────────────

        [OperationContract]
        int RegisterScheduledTask(string sessionToken, string taskName,
            string moduleName, string description, string actionType,
            string actionData, int intervalSeconds);

        [OperationContract]
        bool UnregisterScheduledTask(string sessionToken, int taskId);

        [OperationContract]
        bool SetTaskEnabled(string sessionToken, int taskId, bool enabled);

        [OperationContract]
        DataTable ListScheduledTasks(string sessionToken);

        [OperationContract]
        DataTable GetTaskHistory(string sessionToken, int taskId, int limit);

        [OperationContract]
        bool RunTaskNow(string sessionToken, int taskId);
    }
}
