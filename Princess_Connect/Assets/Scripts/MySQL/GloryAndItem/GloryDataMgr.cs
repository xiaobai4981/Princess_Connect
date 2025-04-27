using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemInfo
{
    public int num;
    public string type;
}
public struct PlayerInventoryInfo
{
    public Dictionary<string, ItemInfo> itemDic;
    public List<int> glory;
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

    // 查询称号的描述和获得条件描述
    public Dictionary<string, string> GetGloryDescription(int gloryId)
    {
        Dictionary<string, string> description = new Dictionary<string, string>();
        try
        {
            string query = $"SELECT description, condition_desc FROM glory_template WHERE glory_id = @glory_id";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@glory_id", gloryId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string desc = reader.GetString("description");
                    string condDesc = reader.GetString("condition_desc");
                    description.Add("description", desc);
                    description.Add("condition_desc", condDesc);
                }
            }
            return description;
        }
        catch (Exception e)
        {
            Debug.LogError($"Get description failed: {e.Message}");
            return null;
        }
    }

    // 查询道具的名称和描述
    public Dictionary<string, string> GetItemDescription(int equipmentId)
    {
        Dictionary<string, string> description = new Dictionary<string, string>();
        try
        {
            string query = $"SELECT name, description FROM equipment_template WHERE equipment_id = @equipment_id";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@equipment_id", equipmentId);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = reader.GetString("name");
                    string desc = reader.GetString("description");
                    description.Add("name", name);
                    description.Add("description", desc);
                }
            }
            return description;
        }
        catch (Exception e)
        {
            Debug.LogError($"Get description failed: {e.Message}");
            return null;
        }
    }

    // 更新用户的称号
    public bool UpdateUserGlory(string username, int gloryId)
    {
        try
        {
            string query = $"UPDATE player_data SET now_emblem = {gloryId} WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                Debug.LogWarning($"No data found for username: {username}");
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Update failed: {e.Message}");
            return false;
        }
    }
}
