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

}
