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

        public DataTable DB_GetTableSchema(string tableName)
        {
            sql.Open();

            SqliteCommand command = sql.CreateCommand();
            command.CommandText = $"SELECT * FROM {tableName};";
            //command.Parameters.Add(new SqliteParameter("$TABLENAME", tableName));

            using (SqliteDataReader reader = command.ExecuteReader())
            {
                DataTable schemaTable = new DataTable(tableName);
                schemaTable.Load(reader);
                return schemaTable;
            }
        }
    }
}
