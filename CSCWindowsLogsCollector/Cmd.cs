using CSCWindowsLogsCollector.Properties;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace CSCWindowsLogsCollector
{
    public class Cmd
    {
        public static string ConnectionString = Encryption.Decode( Settings.Default.connectionStringM);

        private static SqlConnection cn = null;
        private static SqlCommand cmd = null;
        private static SqlDataAdapter da = null;
        public static SqlTransaction tran = null;
        private static string lastCmd = "";

        public static Dictionary<string, object> Parameters = new Dictionary<string, object>();

        public static SqlCommand Command(string sql, params object[] args)
        {
            for (int i = 0; i <= args.Length - 1; i++)
            {
                sql = sql.Replace("{" + i + "}", "@P" + i);
                Parameters["@P" + i] = args[i];
            }

            cmd = new SqlCommand(sql);
            cmd.Connection = connection();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = sql;

            if ((tran != null))
            {
                cmd.Transaction = tran;
            }

            foreach (KeyValuePair<string, object> p in Parameters)
            {
                if (p.Value is DateTime)
                {
                    cmd.Parameters.Add(p.Key, SqlDbType.DateTime).Value = p.Value;
                }
                else if (p.Value is long || p.Value is int)
                {
                    cmd.Parameters.Add(p.Key, SqlDbType.Int).Value = p.Value;
                }
                else if (p.Value is double || p.Value is decimal || p.Value is float)
                {
                    cmd.Parameters.Add(p.Key, SqlDbType.Decimal).Value = p.Value;
                }
                else if (p.Value is bool)
                {
                    cmd.Parameters.Add(p.Key, SqlDbType.Bit).Value = p.Value;
                }
                else if (p.Value == null || object.ReferenceEquals(p.Value, DBNull.Value))
                {
                    cmd.CommandText = cmd.CommandText.Replace(p.Key, "null");
                }
                else if (p.Value is byte[])
                {
                    cmd.Parameters.Add(p.Key, SqlDbType.Image).Value = p.Value;
                }
                else
                {
                    cmd.Parameters.Add(p.Key, SqlDbType.NVarChar).Value = p.Value;
                }
            }

            lastCmd = cmd.CommandText;

            Parameters.Clear();

            return cmd;
        }

        public static SqlConnection connection()
        {
            try
            {
                if (cn == null)
                {
                    cn = new SqlConnection(ConnectionString);
                }
                if (cn.State != ConnectionState.Open)
                {
                    cn.Open();
                }
                return cn;
            }
            catch (SqlException generatedExceptionName)
            {
                cn = new SqlConnection(ConnectionString);
                cn.Open();
                return cn;
            }
        }

        public static object ExecuteScalar(string sql, params object[] values)
        {
            return Command(sql, values).ExecuteScalar();
        }

        public static int ExecuteNonQuery(string sql, params object[] values)
        {
            return Command(sql, values).ExecuteNonQuery();
        }

        public static DataTable ExecuteDataTable(string sql, params object[] values)
        {
            if (da == null)
            {
                da = new SqlDataAdapter();
            }
            da.SelectCommand = Command(sql, values);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        public static IDataReader ExecuteDataReader(string sql, params object[] values)
        {
            return Command(sql, values).ExecuteReader();
        }

        public static DataSet ExecuteDataSet(string sql, params object[] values)
        {
            if (da == null)
            {
                da = new SqlDataAdapter();
            }
            da.SelectCommand = Command(sql, values);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds;
        }

        public static void Begin()
        {
            if (tran == null)
            {
                tran = connection().BeginTransaction();
                if (cmd == null)
                {
                    cmd = connection().CreateCommand();
                }
                cmd.Transaction = tran;
            }
        }

        public static void Commit()
        {
            tran.Commit();
            tran = null;
        }

        public static void Rollback()
        {
            tran.Rollback();
            tran = null;
        }

        public static SqlTransaction Trans
        {
            get { return tran; }
        }
    }
}