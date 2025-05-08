using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
