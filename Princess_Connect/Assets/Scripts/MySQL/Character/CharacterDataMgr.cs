using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterFactoryData
{
    public int characterId;
    public int characterStar;
    public int characterRank;
    public string characterType;
}

public class CharacterDataMgr
{
    // 数据库操作——角色部分
    private static CharacterDataMgr instance;
    private MySqlConnection conn;
    private CharacterDataMgr()
    {
        conn = DatabaseMgr.Instance.conn;
    }

    public static CharacterDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterDataMgr();
            }
            return instance;
        }
    }

    // 添加角色到对应玩家角色仓库
    public bool AddUserPlayerInfo(string username, int characterId)
    {
        try
        {
            // 检查用户名是否已存在
            string checkQuery = "SELECT COUNT(*) FROM player_character WHERE username = @username AND character_id = @character_id";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@username", username);

            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Debug.Log("UserPlayerInfo already exists.");
                return false;
            }

            // 插入新记录
            string insertQuery = "INSERT INTO player_character (username, character_id) " +
                "VALUES (@username, @characterId)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@characterId", characterId); // 注意: 实际应用中应该使用哈希密码

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"AddUserPlayerInfo failed: {e.Message}");
            return false;
        }
    }

    // 初始化玩家对应角色
    public bool InitPlayerCharacter(string username, int characterId, string defaultCharacterEquipment, string defaultCharacterSkillLevel)
    {
        try
        {
            // equipments 和 SkillLevel字段更新
            string query = $"UPDATE player_character SET equipment_slots = @equipment, skills_level = @skillLevel " +
                $"WHERE username = @username AND character_id = @characterId";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@equipment", defaultCharacterEquipment);
            cmd.Parameters.AddWithValue("@skillLevel", defaultCharacterSkillLevel);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@characterId", characterId);

            int rows = cmd.ExecuteNonQuery();

            if (rows == 0)
            {
                Debug.LogWarning($"No data found for username: {username}");
                return false;
            }

            // 其余字段更新
            string otherQuery = $"UPDATE player_character target " +
                $"JOIN character_template source ON target.character_id = source.character_id " +
                $"SET target.character_name = source.name, target.character_type = source.character_type, " +
                $"target.current_stats = source.base_stats, target.skills_config = source.skills_config " +
                $"WHERE target.username = @username AND source.character_id = @characterId";
            MySqlCommand otherCmd = new MySqlCommand(otherQuery, conn);
            otherCmd.Parameters.AddWithValue("@username", username);
            otherCmd.Parameters.AddWithValue("@characterId", characterId);
            int rowsAffected = otherCmd.ExecuteNonQuery();

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

    // 获取角色仓库所需要的角色信息
    public List<CharacterFactoryData> GetCharacterFactoryData(string username)
    {
        List<CharacterFactoryData> characterFactoryDataList = new List<CharacterFactoryData>();
        try
        {
            string sql = "SELECT character_id, current_star, current_rank, character_type FROM player_character WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    CharacterFactoryData characterFactoryData = new CharacterFactoryData();
                    characterFactoryData.characterId = reader.GetInt32("character_id");
                    characterFactoryData.characterStar = reader.GetInt32("current_star");
                    characterFactoryData.characterRank = reader.GetInt32("current_rank");
                    characterFactoryData.characterType = reader.GetString("character_type");
                    characterFactoryDataList.Add(characterFactoryData);
                }
                return characterFactoryDataList;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Get failed: {e.Message}");
            return null;
        }
    }

    // 获取角色基本信息


    // 更改对应玩家角色仓库的角色信息


}
