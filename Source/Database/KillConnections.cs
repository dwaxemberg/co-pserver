
using System;

using System.Data;

using MySql.Data.MySqlClient;

using System.Collections.Generic;
using Conquer_Online_Server;



public class KillConnections
{

    public static MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand();
    public static MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
    public static string myConnectionString = "Username=root;Password=D35kt0pSu990rt;Host=localhost;Database=5632";
    public static void Kill()
    {
        string command = "SHOW processlist";
        List<ulong> processes = new List<ulong>();
        MySqlCommand cmd = new MySqlCommand(command, conn);
        MySqlDataReader reader = null;
        try
        {
            conn.ConnectionString = myConnectionString;
            conn.Open();
            //conn.Open();
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ulong identity = ulong.Parse(reader["Id"].ToString());
                if (reader["Command"].ToString() == "Sleep"
                    && uint.Parse(reader["Time"].ToString()) >= conn.ConnectionTimeout
                    && identity > 0)
                    processes.Add(identity);
            }
            reader.Close();
            //reader.Dispose();
            reader = null;

            foreach (int identity in processes)
            {
                command = "KILL " + identity;
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }
            cmd.Dispose();
            cmd = null;
        }
        catch (Exception e) { Conquer_Online_Server.Console.WriteLine(e); }
        finally
        {
            if (reader != null && !reader.IsClosed)
            {
                reader.Close();
                //reader.Dispose();
                reader = null;
            }
            if (conn != null && conn.State == ConnectionState.Open)
            {
                conn.Close();
                conn.Dispose();
            }
            if (cmd != null)
            {
                cmd.Dispose();
                cmd = null;
            }
        }
    }
}