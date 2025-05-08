using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.Text;

// 玩家任务信息数据结构
public class QuestProgress
{
    public int now_progress = -1;
    public int complete_progress = -1;
}
public class PlayerQuestInfo
{
    public QuestProgress progress_data = new QuestProgress();
    public string status = "none";
    public DateTime last_updated;
}

// 任务配置数据结构
public class QuestData
{
    public int quest_id;
    public string description;
    public string quest_type;
    public string rewards;
    public string unlock_condition;
    public int is_repeatable;
}
public class QuestConfig
{
    public List<QuestData> quests;
}

public class MissionDataMgr
{
    // 数据库操作——任务系统部分
    private static MissionDataMgr instance;
    private MySqlConnection conn;
    private MissionDataMgr()
    {
        conn = DatabaseMgr.Instance.conn;
    }

    public static MissionDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MissionDataMgr();
            }
            return instance;
        }
    }

    // 查询玩家的任务完成数
    public int GetMissionAchieveCount(string playerName)
    {
        try
        {
            string query = $@"
                    SELECT COUNT(*) AS completed_tasks
                    FROM player_quests
                    WHERE 
                        username = @username
                        AND status = 'completed'";
            using(MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", playerName);
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
    
    // 查询任务配置表并存在本地
    public void GetQuestConfig()
    {
        QuestConfig quesetConfigs = new QuestConfig()
        {
            quests = new List<QuestData>()
        };
        try
        {
            string query = "SELECT quest_id, description, quest_type, rewards ->> '$' as rewards, unlock_condition ->> '$' as unlock_condition, is_repeatable FROM quests_config";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        QuestData questData = new QuestData()
                        {
                            quest_id = Convert.ToInt32(reader["quest_id"]),
                            description = reader["description"].ToString(),
                            quest_type = reader["quest_type"].ToString(),
                            rewards = reader["rewards"].ToString(),
                            unlock_condition = reader["unlock_condition"].ToString(),
                            is_repeatable = Convert.ToInt32(reader["is_repeatable"])
                        };
                        quesetConfigs.quests.Add(questData);
                    }
                }
            }
            string jsonStr = JsonMapper.ToJson(quesetConfigs);
            string filePath = Path.Combine(Application.persistentDataPath, "quest_config.json");
            File.WriteAllText(filePath, jsonStr);
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
        }

    }

    // 新玩家默认任务初始化
    public bool AddUserQuest(string playerName, int quest_id, string defaultPlayerQuestProgress)
    {
        try
        {
            // 检查用户名是否已存在
            string checkQuery = "SELECT COUNT(*) FROM player_quests WHERE username = @username AND quest_id = @quest_id";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@username", playerName);
            checkCmd.Parameters.AddWithValue("@quest_id", quest_id);

            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Debug.Log("player_quests_info already exists.");
                return false;
            }

            // 插入新记录
            string insertQuery = "INSERT INTO player_quests (username, quest_id, progress_data, status) " +
                "VALUES (@username, @questId, @progressData, @status)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@username", playerName);
            insertCmd.Parameters.AddWithValue("@questId", quest_id);
            insertCmd.Parameters.AddWithValue("@progressData", defaultPlayerQuestProgress);
            insertCmd.Parameters.AddWithValue("@status", "in_progress");

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"AddUserPlayerInfo failed: {e.Message}");
            return false;
        }
    }

    // 修改数据库中玩家任务信息(progress_data, status, last_updated)
    public bool UpdateUserQuest(string playerName, int questId, PlayerQuestInfo playerQuestInfo)
    {
        try
        {
            var setClauses = new List<string>();
            var parameters = new Dictionary<string, object>();
            if (playerQuestInfo.progress_data.now_progress != -1)
            {
                string nowProgressJson = JsonMapper.ToJson(playerQuestInfo.progress_data);
                setClauses.Add("progress_data = @progressData");
                parameters.Add("@progressData", nowProgressJson);
            }
            if (playerQuestInfo.status != "none")
            {
                setClauses.Add("status = @status");
                parameters.Add("@status", playerQuestInfo.status);
            }
            setClauses.Add("last_updated = @last_updated");
            parameters.Add("@last_updated", playerQuestInfo.last_updated);
            string sql = $"UPDATE player_quests SET { string.Join(", ", setClauses)} WHERE username = @username AND quest_id = @questId";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", playerName);
                cmd.Parameters.AddWithValue("@questId", questId);
                foreach (var kv in parameters)
                {
                    cmd.Parameters.AddWithValue($"@{kv.Key}", kv.Value);
                }
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Modify failed: {e.Message}");
            return false;
        }
    }

    // 查询玩家任务的某一项信息
    public string GetPlayerQuestAnInfo(string playerName, int quest_id, string anInfo)
    {
        try
        {
            if (anInfo == "progress_data")
            {
                anInfo = "progress_data ->> '$' as progress_data";
            }
            string query = $"SELECT {anInfo} FROM player_quests WHERE username = @username AND quest_id = @quest_id";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", playerName);
                cmd.Parameters.AddWithValue("@quest_id", quest_id);
                string res = cmd.ExecuteScalar().ToString();
                return res;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return null;
        }
    }

}
