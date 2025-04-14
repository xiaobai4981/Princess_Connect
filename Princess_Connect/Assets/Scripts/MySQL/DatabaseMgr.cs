using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using Unity.VisualScripting;
using UnityEngine;

public class DatabaseMgr
{
    private static DatabaseMgr instance;
    private MySqlConnection conn;

    // ���ݿ�����
    private string connStr;
    private DatabaseMgr()
    {
        connStr = "Server = 127.0.0.1;port=3306;DataBase=princess_connect;UID=root;Pwd=dajiaxingkule123";
    }

    public static DatabaseMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new DatabaseMgr();
            }
            return instance;
        }
    }
    // ����MySQL���ݿ�
    public void Initialize()
    {
        try
        {
            conn = new MySqlConnection(connStr);
            conn.Open();
            Debug.Log("����MySQL���ݿ�ɹ�");
        }
        catch (System.Exception e)
        {
            throw new System.Exception("����MySQL���ݿ�ʧ�ܣ�" + e.Message.ToString());
        }
    }

    // ע���û�
    public bool RegisterUser(string username, string password)
    {
        try
        {
            // ����û����Ƿ��Ѵ���
            string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@username", username);

            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Debug.Log("Username already exists.");
                return false;
            }

            // �������û�
            string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@password", password); // ע��: ʵ��Ӧ����Ӧ��ʹ�ù�ϣ����

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Registration failed: {e.Message}");
            return false;
        }
    }

    // �û���¼
    public bool LoginUser(string username, string password)
    {
        try
        {
            string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password); // ע��: ʵ��Ӧ����Ӧ�ñȽϹ�ϣֵ

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Login failed: {e.Message}");
            return false;
        }
    }

    // ���ݿ�ر�
    public void DatabaseClose()
    {
        if (conn != null)
        {
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }
}
