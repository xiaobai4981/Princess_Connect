using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

// 角色在仓库中的信息
public struct CharacterFactoryData
{
    public int characterId;
    public int characterStar;
    public int characterRank;
    public string characterType;
}

#region 角色信息结构体
// 基础属性
public class CharacterStats
{
    public int stand_pos;
    public int ce;
    public int likb;
    public int atk;
    public int physical_def;
    public int magical_def;
    public int hp;
    public int crit;
    public int dod;
    public int hp_recover_per_wave;
    public int tp_recover_per_wave;
    public int tp_get;
    public int hit_rate;
    public string attack_type;
    public int atk_cap;
}
// 技能配置（数据库未配置完备，是个demo版本）
public class SkillConfig
{
    public string name;
    public string description;
    // 这个就是要继续配置的字段
    public string effect;
}
public class SkillsConfig
{
    public SkillConfig n_ub_skill;
    public SkillConfig s_ub_skill;
    public SkillConfig skill_1;
    public SkillConfig skill_2;
}
// 接收数据库查询结果的结构体
public class PlayerCharacterData
{
    public int character_id;
    public string character_name;
    public int level;
    public int current_exp;
    public int current_star;
    public int current_rank;
    public int current_likb;
    public CharacterStats current_stats;
    public List<List<int>> rank_requirements;
    public List<int> equipment_slots;
    public SkillsConfig skills_config; // 根据实际数据结构调整
    public Dictionary<string, int> skills_level;
    public string character_type;
}
public class PlayerCharacterCollection
{
    public Dictionary<int, PlayerCharacterData> characters;
}
#endregion


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
    // 初始化玩家角色信息
    public void InitUserCharacterData(string username)
    {
        PlayerCharacterCollection playerCharacterCollection = new PlayerCharacterCollection
        {
            characters = new Dictionary<int, PlayerCharacterData>()
        };
        try
        {
            string query = @"SELECT character_id, character_name, level, current_exp, current_star, 
                current_rank, current_likb, current_stats ->> '$' as current_stats, 
                rank_requirements ->> '$' as rank_requirements, equipment_slots ->> '$' as equipment_slots, 
                skills_config ->> '$' as skills_config, skills_level ->> '$' as skills_level, character_type 
                FROM player_character WHERE username = @username";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        PlayerCharacterData character = new PlayerCharacterData
                        {
                            character_id = reader.GetInt32("character_id"),
                            character_name = reader.IsDBNull("character_name") ? null : reader.GetString("character_name"),
                            level = reader.GetInt32("level"),
                            current_exp = reader.GetInt32("current_exp"),
                            current_star = reader.GetInt32("current_star"),
                            current_rank = reader.GetInt32("current_rank"),
                            current_likb = reader.GetInt32("current_likb"),
                            character_type = reader.IsDBNull("character_type") ? null : reader.GetString("character_type")
                        };

                        // 解析JSON字段
                        if (!reader.IsDBNull("current_stats"))
                        {
                            string statsJson = reader.GetString("current_stats");
                            character.current_stats = JsonMapper.ToObject<CharacterStats>(statsJson);
                        }

                        if (!reader.IsDBNull("rank_requirements"))
                        {
                            string rankJson = reader.GetString("rank_requirements");
                            character.rank_requirements = JsonMapper.ToObject<List<List<int>>>(rankJson);
                        }

                        if (!reader.IsDBNull("equipment_slots"))
                        {
                            string slotsJson = reader.GetString("equipment_slots");
                            character.equipment_slots = JsonMapper.ToObject<List<int>>(slotsJson);
                        }

                        if (!reader.IsDBNull("skills_level"))
                        {
                            string skillsLevelJson = reader.GetString("skills_level");
                            character.skills_level = JsonMapper.ToObject<Dictionary<string, int>>(skillsLevelJson);
                        }

                        if (!reader.IsDBNull("skills_config"))
                        {
                            string skillsConfigJson = reader.GetString("skills_config");
                            character.skills_config = JsonMapper.ToObject<SkillsConfig>(skillsConfigJson);
                        }

                        // 添加到字典，以character_id为键
                        playerCharacterCollection.characters[character.character_id] = character;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"初始化失败: {e.Message}\n堆栈跟踪：{e.StackTrace}");
        }
        string jsonStr = JsonMapper.ToJson(playerCharacterCollection);
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        File.WriteAllText(filePath, jsonStr);
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
                $"target.current_stats = source.base_stats, target.skills_config = source.skills_config , target.rank_requirements = source.rank_requirements " +
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

    // 更改对应玩家角色仓库的角色信息


}
