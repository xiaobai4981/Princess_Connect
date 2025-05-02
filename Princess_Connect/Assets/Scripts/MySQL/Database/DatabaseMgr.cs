using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DatabaseMgr
{
    private static DatabaseMgr instance;
    public MySqlConnection conn;

    // 数据库配置
    private string connStr;
    private DatabaseMgr()
    {
        connStr = "Server = 127.0.0.1;port=3306;DataBase=princess_connect;UID=root;Pwd=dajiaxingkule123;Allow User Variables=True";
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
    // 数据库关闭
    public void DatabaseClose()
    {
        if (conn != null)
        {
            Debug.Log("关闭数据库连接");
            conn.Close();
            conn.Dispose();
            conn = null;
        }
    }
    // 加载关卡配置表


}
