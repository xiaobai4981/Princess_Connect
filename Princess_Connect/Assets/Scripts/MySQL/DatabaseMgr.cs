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

    // 数据库配置
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
    // 连接MySQL数据库
    public void Initialize()
    {
        try
        {
            conn = new MySqlConnection(connStr);
            conn.Open();
            Debug.Log("连接MySQL数据库成功");
        }
        catch (System.Exception e)
        {
            throw new System.Exception("连接MySQL数据库失败：" + e.Message.ToString());
        }
    }

    // 注册用户
    public bool RegisterUser(string username, string password)
    {
        try
        {
            // 检查用户名是否已存在
            string checkQuery = "SELECT COUNT(*) FROM users WHERE username = @username";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@username", username);

            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Debug.Log("Username already exists.");
                return false;
            }

            // 插入新用户
            string insertQuery = "INSERT INTO users (username, password) VALUES (@username, @password)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@password", password); // 注意: 实际应用中应该使用哈希密码

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Registration failed: {e.Message}");
            return false;
        }
    }

    // 用户登录
    public bool LoginUser(string username, string password)
    {
        try
        {
            string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password); // 注意: 实际应用中应该比较哈希值

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Login failed: {e.Message}");
            return false;
        }
    }

    // 数据库关闭
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
