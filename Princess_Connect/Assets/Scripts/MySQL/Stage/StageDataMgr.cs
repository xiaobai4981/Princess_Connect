using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

public class Article
{
    public int mana;
    public int diamond;
    public int player_exp;
    public List<int> item;
    public List<int> equipment;
}

public class Reward
{
    public Article normal_drop;
    public Article first_perfect_clear;
}

public class Monster 
{
    public int rank;
    public int monster_id;
}


public class Config_data
{
    public Reward reward;
    public List<Monster> monsters;
    public int stamina_cost;
}

public class StageData
{
    public Config_data config_data;
    public string stage_type;
}


public class StageDatas
{
    public Dictionary<string, StageData> stage_datas;
}

public class StageDataMgr
{
    // 数据库操作——关卡部分
    private static StageDataMgr instance;
    private MySqlConnection conn;
    private StageDataMgr()
    {
        conn = DatabaseMgr.Instance.conn;
    }

    public static StageDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new StageDataMgr();
            }
            return instance;
        }
    }
    // 获取并保存所有关卡信息到本地
    public void GetAllStages()
    {
        StageDatas stage_config_datas = new StageDatas
        {
            stage_datas = new Dictionary<string, StageData>()
        };
        try
        {
            string query = "SELECT stage_id, config_data ->> '$' as config_data, stage_type FROM stage_config";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int stage_id = Convert.ToInt32(reader["stage_id"]);
                    StageData stageData = new StageData();
                    if (!reader.IsDBNull("config_data"))
                    {
                        stageData.config_data = JsonMapper.ToObject<Config_data>(reader.GetString("config_data"));
                    }
                    if (!reader.IsDBNull("stage_type"))
                    {
                        stageData.stage_type = reader.GetString("stage_type");
                    }
                    stage_config_datas.stage_datas.Add(stage_id.ToString(), stageData);
                }
            } 
        }
        catch (Exception e)
        {
            Debug.LogError($"GetAllStages failed: {e.Message}");
        }
        string jsonStr = JsonMapper.ToJson(stage_config_datas);
        string filePath = Path.Combine(Application.persistentDataPath, "stage_config_data.json");
        File.WriteAllText(filePath, jsonStr);
    }

    // 增加玩家的关卡信息
    public bool AddUserStage(string playerName, int stage_id)
    {
        try
        {
            // 检查用户名是否已存在
            string checkQuery = "SELECT COUNT(*) FROM player_stages WHERE username = @username AND stage_id = @stage_id";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@username", playerName);
            checkCmd.Parameters.AddWithValue("@stage_id", stage_id);

            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Debug.Log("player_quests_info already exists.");
                return false;
            }

            // 插入新记录
            string insertQuery = "INSERT INTO player_stages (username, stage_id, star) " +
                "VALUES (@username, @stage_id, @star)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@username", playerName);
            insertCmd.Parameters.AddWithValue("@stage_id", stage_id);
            insertCmd.Parameters.AddWithValue("@star", 0);

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"AddUserPlayerInfo failed: {e.Message}");
            return false;
        }
    }

    // 更新玩家的关卡信息
    public bool UpdateStageStar(string username, int stage_id, int starCnt)
    {
        try
        {
            string query = $"UPDATE player_stages SET star = {starCnt} WHERE username = @username AND stage_id = @stage_id";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@stage_id", stage_id);

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

    // 查询玩家的关卡信息
    public int GetPlayerStageStar(string username, int stage_id)
    {
        try
        {
            string query = $"SELECT star FROM player_stages WHERE username = @username AND stage_id = @stage_id";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@stage_id", stage_id);
                int res = Convert.ToInt32(cmd.ExecuteScalar());
                return res;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return -1;
        }
    }
}
