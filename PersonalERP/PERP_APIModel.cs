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

        public void Log(string Message)
        {
            Console.WriteLine(Message);
        }

        public List<string> DB_ListTables()
        {
            sql.Open();
            List<string> TableNames = new List<string>();

            SqliteCommand command = sql.CreateCommand();
            command.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' ORDER BY 1";
            SqliteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                TableNames.Add(reader.GetValue(0).ToString());
            }

            reader.Close();
            sql.Close();
            return TableNames;
        }

        private void ValidateSqlCommand(string sqlText)
        {
            string upperSql = sqlText.ToUpper().Trim();
            if (upperSql.StartsWith("DROP ") || upperSql.Contains("; DROP "))
            {
                throw new InvalidOperationException("DROP commands are not allowed for security reasons");
            }
            if (upperSql.StartsWith("TRUNCATE ") || upperSql.Contains("; TRUNCATE "))
            {
                throw new InvalidOperationException("TRUNCATE commands are not allowed for security reasons");
            }
        }

        public DataTable DB_GetTableSchema(string tableName)
        {
            sql.Open();
            try
            {
                SqliteCommand command = sql.CreateCommand();
                command.CommandText = $"SELECT * FROM {tableName};";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    DataTable schemaTable = new DataTable(tableName);
                    schemaTable.Load(reader);
                    return schemaTable;
                }
            }
            finally
            {
                sql.Close();
            }
        }

        public int DB_ExecuteNonQuery(string sqlText)
        {
            ValidateSqlCommand(sqlText);
            sql.Open();
            try
            {
                SqliteCommand command = sql.CreateCommand();
                command.CommandText = sqlText;
                int result = command.ExecuteNonQuery();
                return result;
            }
            finally
            {
                sql.Close();
            }
        }

        public DataTable DB_ExecuteQuery(string sqlText)
        {
            ValidateSqlCommand(sqlText);
            sql.Open();
            try
            {
                SqliteCommand command = sql.CreateCommand();
                command.CommandText = sqlText;
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    DataTable result = new DataTable();
                    result.Load(reader);
                    return result;
                }
            }
            finally
            {
                sql.Close();
            }
        }
    }
}
