﻿
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Framework
{
    public class MsSQL
    {
        #region public
        public static SqlConnection connection;
        public SqlConnection connection1;
        public SqlCommand cmd1;
        public string SQL;
        public SqlDataReader reader;

        public MsSQL(string sql)
        {
            if (reader != null) { this.Close(); SQL = null; cmd1 = null; }

            SQL = sql;
        }
        public SqlDataReader Read()
        {
            try
            {
                cmd1 = new SqlCommand(SQL, connection);
                reader = cmd1.ExecuteReader();
            }
            catch (Exception ex)
            {
                Console.WriteLine("db error command:{0}", SQL);
                OnDatabaseError(ex);
            }
            return reader;
        }
        public void Close()
        {
            if (!reader.IsClosed)
            {
                reader.Dispose();
                reader.Close();
            }
        }

        public int Count()
        {
            int count = 0;
            try
            {
                da = new SqlDataAdapter(SQL, connection);
                DataSet ds = new DataSet();

                da.Fill(ds);
                if (ds.Tables.Count > 0)
                    count = ds.Tables[0].Rows.Count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("db error command:{0}", SQL);
                OnDatabaseError(ex);
            }
            return count;
        }

        public delegate void dError(Exception ex);
        public static event dError OnDatabaseError;
        public delegate void dConnected();
        public static event dConnected OnConnectedToDatabase;


        public static SqlDataAdapter da;
        #endregion

        #region Baglanti
        public static void Connection(string connections)
        {
            //string Connection = "Data Source=ALIM\\SQLExpress;Initial Catalog=Silkroad_Server;User ID=sa;Password=shaman1;";
            string Connection = connections;
            if (connection != null)
                connection.Close();

            try
            {
                connection = new SqlConnection();

                connection.ConnectionString = Connection;
                connection.Open();
                OnConnectedToDatabase();
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
            }

        }
        #endregion

        #region Single data
        public static string GetData(string sql, string column)
        {
            string GetResults = null;

            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand(sql, connection);


            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetResults = reader[column].ToString();
                }

            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();
            }
            return GetResults;
        }
        public static double GetDataDouble(string sql, string column)
        {
            double GetResults = 0;

            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand(sql, connection);


            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetResults = Convert.ToDouble(reader[column].ToString());
                }
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();
            }
            return GetResults;
        }
        public static int GetDataInt(string sql, string column)
        {
            int GetResults = 0;

            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand(sql, connection);

            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetResults = Convert.ToInt32(reader[column].ToString());
                }
                if (!reader.IsClosed) reader.Close();
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();
            }
            return GetResults;
        }
        public static long GetDataLong(string sql, string column)
        {
            long GetResults = 0;

            SqlDataReader reader = null;
            SqlCommand cmd = new SqlCommand(sql, connection);


            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    GetResults = Convert.ToInt64(reader[column].ToString());
                }
                if (!reader.IsClosed) reader.Close();
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
            }
            finally
            {
                if (!reader.IsClosed)
                    reader.Close();
            }
            return GetResults;
        }
        #endregion

        #region Rows Count
        public static int GetRowsCount(string command)
        {
            int count = 0;
            try
            {
                da = new SqlDataAdapter(command, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                count = ds.Tables[0].Rows.Count;
                da.Dispose();
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
                Console.WriteLine("GetRowsCount:{0}", command);
            }
            return count;
        }
        #endregion

        #region Insert Data
        public static void InsertData(string command)
        {
            SqlCommand cmd = new SqlCommand(command, connection);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
                Console.WriteLine(command);
            }
        }
        #endregion

        #region Update Data

        public static void UpdateData(string command)
        {
            InsertData(command);
        }

        #endregion

        #region Delete Data

        public static void DeleteData(string command)
        {
            InsertData(command);
        }

        #endregion


        public static bool Varsa(string command)
        {
            bool count = false;
            try
            {
                da = new SqlDataAdapter(command, connection);
                DataSet ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count != 0) count = true;
                else count = false;
            }
            catch (Exception ex)
            {
                OnDatabaseError(ex);
            }
            return count;

        }
    }

}

