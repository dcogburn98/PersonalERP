using System;
using System.Data;
using System.Diagnostics;
using System.Threading;
using Microsoft.Data.Sqlite;

namespace PersonalERP_Server
{
    internal static class TaskSchedulerEngine
    {
        private static SqliteConnection sql;
        private static Timer timer;
        private static bool running;

        public static void Initialize(SqliteConnection conn)
        {
            sql = conn;
        }

        public static void Start()
        {
            timer = new Timer(Tick, null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(15));
            Console.WriteLine("Task scheduler started (15s tick).");
        }

        public static void Stop()
        {
            timer?.Dispose();
            timer = null;
        }

        private static void Tick(object state)
        {
            if (running) return;
            running = true;
            try
            {
                sql.Open();
                try
                {
                    var cmd = sql.CreateCommand();
                    cmd.CommandText = @"SELECT id, task_name, action_type, action_data
                        FROM perp_scheduled_tasks
                        WHERE is_enabled = 1
                          AND (next_run IS NULL OR next_run <= $now)";
                    cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("o"));

                    var dueTasks = new DataTable();
                    using (var r = cmd.ExecuteReader())
                        dueTasks.Load(r);

                    foreach (DataRow row in dueTasks.Rows)
                    {
                        int taskId = Convert.ToInt32(row["id"]);
                        string name = row["task_name"].ToString();
                        string actionType = row["action_type"].ToString();
                        string actionData = row["action_data"].ToString();
                        ExecuteTask(taskId, name, actionType, actionData);
                    }
                }
                finally { sql.Close(); }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Scheduler] Error: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            finally { running = false; }
        }

        private static void ExecuteTask(int taskId, string name, string actionType, string actionData)
        {
            var sw = Stopwatch.StartNew();
            string status = "Success";
            string message = "";
            try
            {
                switch (actionType.ToUpper())
                {
                    case "SQL":
                        var cmd = sql.CreateCommand();
                        cmd.CommandText = actionData;
                        int affected = cmd.ExecuteNonQuery();
                        message = $"Rows affected: {affected}";
                        break;

                    case "SQL_QUERY":
                        var qcmd = sql.CreateCommand();
                        qcmd.CommandText = actionData;
                        using (var reader = qcmd.ExecuteReader())
                        {
                            var dt = new DataTable();
                            dt.Load(reader);
                            message = $"Rows returned: {dt.Rows.Count}";
                        }
                        break;

                    case "LOG":
                        Console.WriteLine($"[Scheduled] {actionData}");
                        message = "Message logged";
                        break;

                    case "CLEANUP_SESSIONS":
                        var cleanup = sql.CreateCommand();
                        cleanup.CommandText = "DELETE FROM perp_sessions WHERE expires_date < $now";
                        cleanup.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("o"));
                        int deleted = cleanup.ExecuteNonQuery();
                        message = $"Expired sessions cleaned: {deleted}";
                        break;

                    case "CLEANUP_HISTORY":
                        int keepDays = 30;
                        if (int.TryParse(actionData, out int d)) keepDays = d;
                        var hclean = sql.CreateCommand();
                        hclean.CommandText = "DELETE FROM perp_task_history WHERE run_date < $cutoff";
                        hclean.Parameters.AddWithValue("$cutoff",
                            DateTime.UtcNow.AddDays(-keepDays).ToString("o"));
                        int hdeleted = hclean.ExecuteNonQuery();
                        message = $"Old history entries cleaned: {hdeleted}";
                        break;

                    default:
                        status = "Error";
                        message = $"Unknown action type: {actionType}";
                        break;
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"[Scheduler] Ran '{name}': {message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception ex)
            {
                status = "Error";
                message = ex.Message;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[Scheduler] Task '{name}' failed: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.White;
            }
            sw.Stop();

            try
            {
                RecordHistory(taskId, status, message, (int)sw.ElapsedMilliseconds);
                UpdateNextRun(taskId);
            }
            catch { }
        }

        private static void RecordHistory(int taskId, string status, string message, int durationMs)
        {
            var cmd = sql.CreateCommand();
            cmd.CommandText = @"INSERT INTO perp_task_history (task_id,run_date,status,result_message,duration_ms)
                VALUES ($tid,$rd,$st,$msg,$dur)";
            cmd.Parameters.AddWithValue("$tid", taskId);
            cmd.Parameters.AddWithValue("$rd", DateTime.UtcNow.ToString("o"));
            cmd.Parameters.AddWithValue("$st", status);
            cmd.Parameters.AddWithValue("$msg", message);
            cmd.Parameters.AddWithValue("$dur", durationMs);
            cmd.ExecuteNonQuery();
        }

        private static void UpdateNextRun(int taskId)
        {
            var cmd = sql.CreateCommand();
            cmd.CommandText = @"UPDATE perp_scheduled_tasks
                SET last_run = $now,
                    next_run = datetime($now, '+' || interval_seconds || ' seconds')
                WHERE id = $id";
            cmd.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("o"));
            cmd.Parameters.AddWithValue("$id", taskId);
            cmd.ExecuteNonQuery();
        }

        public static int Register(string taskName, string moduleName, string description,
            string actionType, string actionData, int intervalSeconds, int? createdBy)
        {
            sql.Open();
            try
            {
                string now = DateTime.UtcNow.ToString("o");
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"INSERT INTO perp_scheduled_tasks
                    (task_name,module_name,description,action_type,action_data,
                     interval_seconds,is_enabled,next_run,created_by,created_date)
                    VALUES ($n,$m,$d,$at,$ad,$i,1,$nr,$cb,$cd)";
                cmd.Parameters.AddWithValue("$n", taskName);
                cmd.Parameters.AddWithValue("$m", moduleName);
                cmd.Parameters.AddWithValue("$d", description ?? "");
                cmd.Parameters.AddWithValue("$at", actionType);
                cmd.Parameters.AddWithValue("$ad", actionData);
                cmd.Parameters.AddWithValue("$i", intervalSeconds);
                cmd.Parameters.AddWithValue("$nr", DateTime.UtcNow.AddSeconds(intervalSeconds).ToString("o"));
                cmd.Parameters.AddWithValue("$cb", createdBy.HasValue ? (object)createdBy.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("$cd", now);
                cmd.ExecuteNonQuery();

                cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT last_insert_rowid()";
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            finally { sql.Close(); }
        }

        public static bool Unregister(int taskId)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "DELETE FROM perp_task_history WHERE task_id=$id";
                cmd.Parameters.AddWithValue("$id", taskId);
                cmd.ExecuteNonQuery();

                cmd = sql.CreateCommand();
                cmd.CommandText = "DELETE FROM perp_scheduled_tasks WHERE id=$id";
                cmd.Parameters.AddWithValue("$id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally { sql.Close(); }
        }

        public static bool SetEnabled(int taskId, bool enabled)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "UPDATE perp_scheduled_tasks SET is_enabled=$e WHERE id=$id";
                cmd.Parameters.AddWithValue("$e", enabled ? 1 : 0);
                cmd.Parameters.AddWithValue("$id", taskId);
                return cmd.ExecuteNonQuery() > 0;
            }
            finally { sql.Close(); }
        }

        public static bool RunNow(int taskId)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = "SELECT task_name, action_type, action_data FROM perp_scheduled_tasks WHERE id=$id";
                cmd.Parameters.AddWithValue("$id", taskId);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return false;
                    ExecuteTask(taskId, r.GetString(0), r.GetString(1), r.GetString(2));
                    return true;
                }
            }
            finally { sql.Close(); }
        }

        public static DataTable List()
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"SELECT id AS ID, task_name AS Name, module_name AS Module,
                    description AS Description, action_type AS ActionType, action_data AS ActionData,
                    interval_seconds AS IntervalSec, is_enabled AS Enabled,
                    last_run AS LastRun, next_run AS NextRun, created_date AS Created
                    FROM perp_scheduled_tasks ORDER BY id";
                var dt = new DataTable();
                using (var r = cmd.ExecuteReader()) dt.Load(r);
                return dt;
            }
            finally { sql.Close(); }
        }

        public static DataTable History(int taskId, int limit)
        {
            sql.Open();
            try
            {
                var cmd = sql.CreateCommand();
                cmd.CommandText = @"SELECT id AS ID, run_date AS RunDate, status AS Status,
                    result_message AS Result, duration_ms AS DurationMs
                    FROM perp_task_history WHERE task_id=$tid
                    ORDER BY id DESC LIMIT $lim";
                cmd.Parameters.AddWithValue("$tid", taskId);
                cmd.Parameters.AddWithValue("$lim", limit);
                var dt = new DataTable();
                using (var r = cmd.ExecuteReader()) dt.Load(r);
                return dt;
            }
            finally { sql.Close(); }
        }
    }
}
