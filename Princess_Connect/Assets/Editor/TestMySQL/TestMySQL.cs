using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class TestMySQL : MonoBehaviour
{
    //MySQL ���Ӷ���
    public static MySqlConnection dbConnection;
    //���ӻ�ر� MySQL���ݿ�
    public void ConnectMySQL(string connectionStr, bool isOpen)
    {
        if (isOpen)
        {
            try
            {
                dbConnection = new MySqlConnection(connectionStr);
                dbConnection.Open();
                Debug.Log("����MySQL���ݿ�ɹ�");
            }
            catch (System.Exception e)
            {
                throw new System.Exception("����MySQL���ݿ�ʧ�ܣ�" + e.Message.ToString());
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

    //SQL���ִ�з���
    public DataSet ExecuteQuery(string sqlStr, MySqlConnection ConnectionDB)
    {
        if (dbConnection.State == ConnectionState.Open)
        {
            //��ļ���
            DataSet dataSet = new DataSet();
            try
            {
                MySqlDataAdapter data = new MySqlDataAdapter(sqlStr, ConnectionDB);
                data.Fill(dataSet);
            }
            catch (System.Exception e)
            {
                throw new System.Exception("SQL��" + sqlStr + "/n" + e.Message.ToString());
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
        // Debug.Log("��������"+ds.Tables[0].Rows.Count+" ������");
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            Debug.Log("������" + ds.Tables[0].Rows[i]["name"] + " ���䣺" + ds.Tables[0].Rows[i]["age"]);
        }
    }

}
