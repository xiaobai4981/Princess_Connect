using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerInventoryInfo
{
    public Dictionary<string, int> item { get; set; }
    public List<int> glory { get; set; }
}

public class GloryDataMgr
{
    // 数据库操作——道具/称号系统部分
    private static GloryDataMgr instance;
    private MySqlConnection conn;
    private GloryDataMgr()
    {
        conn = DatabaseMgr.Instance.conn;
    }

    public static GloryDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GloryDataMgr();
            }
            return instance;
        }
    }

    // 查询用户仓库信息
    public string SearchUserPlayerFactoryInfo(string username, string anInfo)
    {
        try
        {
            string query = $"SELECT JSON_UNQUOTE(JSON_EXTRACT({anInfo}, '$')) FROM player_data WHERE username = @username";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);

                object result = cmd.ExecuteScalar();

                if (result == null || result == DBNull.Value)
                {
                    Debug.LogWarning($"No data found for username: {username}");
                    return null;
                }

                return result.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return null;
        }
    }

    // 统计并返回所有称号的id
    public List<int> GetAllGloryIds()
    {
        List<int> gloryIds = new List<int>();
        try
        {
            string query = $"SELECT glory_id FROM glory_template";
            MySqlCommand cmd = new MySqlCommand(query, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = reader.GetInt32("glory_id"); // 假设glory_id是INT类型
                    gloryIds.Add(id);
                }
            }
            return gloryIds;
        }
        catch (Exception e)
        {
            Debug.LogError($"Count failed: {e.Message}");
            return null;
        }
    }
}
