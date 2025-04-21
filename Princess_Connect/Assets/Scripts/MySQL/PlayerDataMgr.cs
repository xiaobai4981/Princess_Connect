using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDataMgr
{
    // 数据库操作——用户部分
    private static PlayerDataMgr instance;
    private MySqlConnection conn;
    private PlayerDataMgr()
    {
        conn = DatabaseMgr.Instance.conn;
    }

    public static PlayerDataMgr Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerDataMgr();
            }
            return instance;
        }
    }

    // 注册用户
    public bool RegisterUser(string username, string password)
    {
        try
        {
            // 检查用户名是否已存在
            string checkQuery = "SELECT COUNT(*) FROM player_data WHERE username = @username";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@username", username);

            int count = Convert.ToInt32(checkCmd.ExecuteScalar());
            if (count > 0)
            {
                Debug.Log("Username already exists.");
                return false;
            }

            // 插入新用户
            string insertQuery = "INSERT INTO player_data (username, password_any) VALUES (@username, @password)";
            MySqlCommand insertCmd = new MySqlCommand(insertQuery, conn);
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@password", password); // 注意: 实际应用中应该使用哈希密码

            int rowsAffected = insertCmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Registration failed: {e.Message}");
            return false;
        }
    }

    // 用户登录
    public bool LoginUser(string username, string password)
    {
        try
        {
            string query = "SELECT COUNT(*) FROM player_data WHERE username = @username AND password_any = @password";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            cmd.Parameters.AddWithValue("@password", password); // 注意: 实际应用中应该比较哈希值

            int count = Convert.ToInt32(cmd.ExecuteScalar());
            return count > 0;
        }
        catch (Exception e)
        {
            Debug.LogError($"Login failed: {e.Message}");
            return false;
        }
    }

    // 查询用户的信息
    // 查询用户表的类型为 int 的信息
    public int SearchUserIntInfo(string username, string anInfo)
    {
        try
        {
            string query = $"SELECT {anInfo} FROM player_data WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);

            int res = Convert.ToInt32(cmd.ExecuteScalar());
            return res;
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return -1;
        }
    }
    // 查询用户上次体力更新的时间
    public int SearchUserLastStaminaUpdateInfo(string username, string anInfo)
    {
        MySqlDataReader reader = null;
        try
        {
            string query = $"SELECT {anInfo} FROM player_data WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            reader = cmd.ExecuteReader();

            // 必须先调用 Read() 才能访问数据
            if (!reader.Read())
            {
                Debug.LogWarning("No data found for the given username.");
                return -1; // 没有数据
            }

            // 检查字段是否为 NULL
            if (reader.IsDBNull(reader.GetOrdinal(anInfo)))
            {
                Debug.LogWarning("The field is NULL.");
                return -1;
            }

            DateTime res = reader.GetDateTime("last_stamina_update");
            TimeSpan span = DateTime.Now - res;
            int hours = span.Days * 24 + span.Hours;
            return hours;
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return -1;
        }
        finally
        {
            reader?.Close();
            // 根据conn的管理策略决定是否在这里关闭连接
        }
    }
    // 查询用户仓库信息
    public string SearchUserPlayerFactoryInfo(string username, string anInfo)
    {
        try
        {
            string query = $"SELECT {anInfo} FROM player_data WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@username", username);
            string jsonString = cmd.ExecuteScalar() as string;
            return jsonString;
        }
        catch (Exception e)
        {
            Debug.LogError($"Search failed: {e.Message}");
            return null;
        }
    }
    // 查询玩家等级配置
    public int SearchUserLevelConfig(int level, string anInfo)
    {
        try
        {
            string query = $"SELECT {anInfo} FROM player_level_config WHERE level = @level";
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
    // 修改用户的信息
    public bool ModifyUserIntInfo(string username, Dictionary<string, object> columnUpdates, bool isAdd)
    {
        if (columnUpdates == null || columnUpdates.Count == 0)
            return false;
        // 列名白名单（防止 SQL 注入）
        var allowedColumns = new HashSet<string> { "level", "current_exp", "current_stamina", "mana_cnt", "diamond_cnt", "last_stamina_update" , "inventory"}; // 示例列
        var invalidColumns = columnUpdates.Keys.Where(col => !allowedColumns.Contains(col)).ToList();
        if (invalidColumns.Any())
        {
            Debug.LogError($"Invalid columns: {string.Join(", ", invalidColumns)}");
            return false;
        }
        try
        {
            var setClauses = new List<string>();
            foreach (var column in columnUpdates.Keys)
            {
                // 根据 isAdd 决定是直接赋值还是增量
                var valuePlaceholder = isAdd ? $"{column} + @{column}" : $"@{column}";
                setClauses.Add($"`{column}` = {valuePlaceholder}"); // MySQL 列名用反引号包裹
            }
            string setClause = string.Join(", ", setClauses);

            string sql = $"UPDATE player_data SET {setClause} WHERE username = @username";
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@username", username);
                foreach (var kv in columnUpdates)
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

}
