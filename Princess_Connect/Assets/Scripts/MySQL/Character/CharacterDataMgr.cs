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
    public int ce { set; get; }
    public int likb { set; get; }
    public int atk { set; get; }
    public int physical_def { set; get; }
    public int magical_def { set; get; }
    public int hp { set; get; }
    public int crit { set; get; }
    public int dod { set; get; }
    public int hp_recover_per_wave { set; get; }
    public int tp_recover_per_wave { set; get; }
    public int tp_get { set; get; }
    public int hit_rate { set; get; }
    public string attack_type;
    public int atk_cap;
}
// 技能配置
public class Effect
{
    public string type;
    public string target;
    public string formula;
    public string text;
    public string buffWhat; // 这个字段在某些effect中存在，某些中没有
}

public class SkillConfig
{
    public string id;
    public string name;
    public string description;
    public List<Effect> effect;
}
public class SkillsConfig
{
    public SkillConfig n_ub_skill;
    public SkillConfig s_ub_skill;
    public SkillConfig skill_1;
    public SkillConfig skill_2;
    public SkillConfig ex_skill;
}

public class RankRequirements
{
    public List<List<int>> rank_requirements;
}

public class EquipmentSlots
{
    public List<int> equipment_slot;
}

// 接收数据库查询结果的结构体
public class PlayerCharacterData
{
    public int character_id;
    public string character_name;
    public int level { set; get; }
    public int current_exp { set; get; }
    public int current_star { set; get; }
    public int current_rank { set; get; }
    public int current_likb { set; get; }
    public CharacterStats current_stats { set; get; }
    public RankRequirements rank_requirements;
    public EquipmentSlots equipment_slots { set; get; }
    public SkillsConfig skills_config; // 根据实际数据结构调整
    public Dictionary<string, int> skills_level { set; get; }
    public string character_type;
}
public class PlayerCharacterCollection
{
    public Dictionary<string, PlayerCharacterData> characters;
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
            characters = new Dictionary<string, PlayerCharacterData>()
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
                            character.rank_requirements = JsonMapper.ToObject<RankRequirements>(rankJson);
                        }

                        if (!reader.IsDBNull("equipment_slots"))
                        {
                            string slotsJson = reader.GetString("equipment_slots");
                            character.equipment_slots = JsonMapper.ToObject<EquipmentSlots>(slotsJson);
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
                        playerCharacterCollection.characters[character.character_id.ToString()] = character;
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

    // 上传玩家角色信息
    public void UploadUserCharacterData(string username, int characterId, PlayerCharacterData playerCharacterData)
    {
        // 准备数据
        // 提取基础字段
        int level = playerCharacterData.level;
        int currentExp = playerCharacterData.current_exp;
        int currentStar = playerCharacterData.current_star;
        int currentRank = playerCharacterData.current_rank;
        int currentLikb = playerCharacterData.current_likb;
        // 将复杂对象序列化为JSON字符串
        string currentStatsJson = JsonMapper.ToJson(playerCharacterData.current_stats);
        string rankRequirementsJson = JsonMapper.ToJson(playerCharacterData.rank_requirements);
        string equipmentSlotsJson = JsonMapper.ToJson(playerCharacterData.equipment_slots);
        string skillsConfigJson = JsonMapper.ToJson(playerCharacterData.skills_config);
        string skillsLevelJson = JsonMapper.ToJson(playerCharacterData.skills_level);
        try
        {
            string updateSql = @"
                UPDATE player_character SET 
                    level = @level, 
                    current_exp = @currentExp, 
                    current_star = @currentStar, 
                    current_rank = @currentRank, 
                    current_likb = @currentLikb, 
                    current_stats = @currentStats, 
                    rank_requirements = @rankRequirements, 
                    equipment_slots = @equipmentSlots, 
                    skills_config = @skillsConfig, 
                    skills_level = @skillsLevel 
                    WHERE username = @username AND character_id = @characterId";
            using (MySqlCommand updateCmd = new MySqlCommand(updateSql, conn))
            {
                updateCmd.Parameters.AddWithValue("@level", level);
                updateCmd.Parameters.AddWithValue("@currentExp", currentExp);
                updateCmd.Parameters.AddWithValue("@currentStar", currentStar);
                updateCmd.Parameters.AddWithValue("@currentRank", currentRank);
                updateCmd.Parameters.AddWithValue("@currentLikb", currentLikb);
                updateCmd.Parameters.AddWithValue("@currentStats", currentStatsJson);
                updateCmd.Parameters.AddWithValue("@rankRequirements", rankRequirementsJson);
                updateCmd.Parameters.AddWithValue("@equipmentSlots", equipmentSlotsJson);
                updateCmd.Parameters.AddWithValue("@skillsConfig", skillsConfigJson);
                updateCmd.Parameters.AddWithValue("@skillsLevel", skillsLevelJson);
                updateCmd.Parameters.AddWithValue("@username", username);
                updateCmd.Parameters.AddWithValue("@characterId", characterId);
                int rows = updateCmd.ExecuteNonQuery();
                if (rows == 0)
                {
                    Debug.LogWarning($"No data update for characterId: {characterId}");
                    return;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"更新数据库时出错: {e.Message}");
            throw;
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

    // 查询角色等级配置表
    public int SearchCharacterLevelConfig(int level)
    {
        try
        {
            string query = $"SELECT require_exp FROM character_level_config WHERE level = @level";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@level", level);

            int res = Convert.ToInt32(cmd.ExecuteScalar());
            return res;
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return -1;
        }
    }

    // 更改对应玩家角色仓库的角色信息


}
