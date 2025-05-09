using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using UnityEngine;

public class MonsterStats
{
    public int stand_pos;
    public int level;
    public int atk { set; get; }
    public int physical_def { set; get; }
    public int magical_def { set; get; }
    public int hp { set; get; }
    public int crit { set; get; }
    public int dod { set; get; }
    public int tp_get { set; get; }
    public string attack_type;
    public int atk_cap;
}
// 技能配置
public class MonsterSkillEffect
{
    public string type;
    public string target;
    public string formula;
}

public class MonsterSkillConfig
{
    public List<Effect> effect;
}
public class MonsterSkillsConfig
{
    public SkillConfig skill_1;
    public SkillConfig skill_2;
}
public class MonsterData
{
    public int monster_id;
    public string name;
    public MonsterStats base_stats { set; get; }
    public MonsterSkillsConfig skills_config; // 根据实际数据结构调整
    public string monster_type;
}
public class MonsterConfig
{
    public Dictionary<string, MonsterData> monsters;
}
public class MonsterDataMgr
{
    // 数据库操作——怪物数据部分
    private static MonsterDataMgr instance;
    private MySqlConnection conn;
    private MonsterDataMgr()
    {
        conn = DatabaseMgr.Instance.conn;
    }

    public static MonsterDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MonsterDataMgr();
            }
            return instance;
        }
    }
    public void InitMonsterData()
    {
        MonsterConfig monsterConfig = new MonsterConfig
        {
            monsters = new Dictionary<string, MonsterData>()
        };
        try
        {
            string query = @"SELECT monster_id, name, base_stats ->> '$' as base_stats, 
                skill_config ->> '$' as skill_config, monster_type 
                FROM monster_template";
            using (MySqlCommand cmd = new MySqlCommand(query, conn))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        MonsterData monster = new MonsterData
                        {
                            monster_id = reader.GetInt32("monster_id"),
                            name = reader.IsDBNull("name") ? null : reader.GetString("name"),
                            monster_type = reader.IsDBNull("monster_type") ? null : reader.GetString("monster_type")
                        };

                        // 解析JSON字段
                        if (!reader.IsDBNull("base_stats"))
                        {
                            string statsJson = reader.GetString("base_stats");
                            monster.base_stats = JsonMapper.ToObject<MonsterStats>(statsJson);
                        }

                        if (!reader.IsDBNull("skill_config"))
                        {
                            string skillsConfigJson = reader.GetString("skill_config");
                            monster.skills_config = JsonMapper.ToObject<MonsterSkillsConfig>(skillsConfigJson);
                        }

                        // 添加到字典，以character_id为键
                        monsterConfig.monsters[monster.monster_id.ToString()] = monster;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"初始化失败: {e.Message}\n堆栈跟踪：{e.StackTrace}");
        }
        string jsonStr = JsonMapper.ToJson(monsterConfig);
        string filePath = Path.Combine(Application.persistentDataPath, "monster_config.json");
        File.WriteAllText(filePath, jsonStr);
    }

}
