using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TestMySQL : MonoBehaviour
{
    //MySQL 连接对象
    public static MySqlConnection dbConnection;
    //连接或关闭 MySQL数据库
    public void ConnectMySQL(string connectionStr, bool isOpen)
    {
        if (isOpen)
        {
            try
            {
                dbConnection = new MySqlConnection(connectionStr);
                dbConnection.Open();
                Debug.Log("连接MySQL数据库成功");
            }
            catch (System.Exception e)
            {
                throw new System.Exception("连接MySQL数据库失败：" + e.Message.ToString());
            }
        }
        else
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
                dbConnection = null;
            }
        }
    }

    //SQL语句执行方法
    public DataSet ExecuteQuery(string sqlStr, MySqlConnection ConnectionDB)
    {
        if (dbConnection.State == ConnectionState.Open)
        {
            //表的集合
            DataSet dataSet = new DataSet();
            try
            {
                MySqlDataAdapter data = new MySqlDataAdapter(sqlStr, ConnectionDB);
                data.Fill(dataSet);
            }
            catch (System.Exception e)
            {
                throw new System.Exception("SQL：" + sqlStr + "/n" + e.Message.ToString());
            }
            return dataSet;
        }
        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        string connectionStr = "Server = 127.0.0.1;port=3306;DataBase=test;UID=root;Pwd=dajiaxingkule123";
        ConnectMySQL(connectionStr, true);

        string sqlQuery = "select * from user";
        DataSet ds = ExecuteQuery(sqlQuery, dbConnection);
        // Debug.Log("检索到："+ds.Tables[0].Rows.Count+" 条数据");
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            Debug.Log("姓名：" + ds.Tables[0].Rows[i]["name"] + " 年龄：" + ds.Tables[0].Rows[i]["age"]);
        }
    }

}
