using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    
}
