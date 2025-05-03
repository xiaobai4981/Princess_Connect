using LitJson;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInfo
{
    public string username;
    public string password_any;
    public int level;
    public int current_exp;
    public int current_stamina;
    public int mana_cnt;
    public int diamond_cnt;
    public DateTime last_stamina_update;
    public string inventory;
    public int now_emblem;
}
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

    #region 初始化保存用户数据
    public void InitPlayerData(string username)
    {
        try
        {
            string query = "SELECT username, password_any, level, current_exp, " +
                "current_stamina, mana_cnt, diamond_cnt, last_stamina_update, inventory ->> '$' as inventory, " +
                "now_emblem FROM player_data WHERE username = @username"; 
            MySqlCommand cmd = new MySqlCommand(query, conn);
            cmd.Parameters.Add("@username", MySqlDbType.VarChar, 50).Value = username;

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.Read())
                {
                    Debug.LogWarning("未找到对应用户名的数据。");
                    return;
                }

                // 处理可能为 NULL 的字段
                PlayerInfo playerInfo = new PlayerInfo
                {
                    username = reader.GetString("username"),
                    password_any = reader.GetString("password_any"),
                    level = reader.GetInt32("level"),
                    current_exp = reader.GetInt32("current_exp"),
                    current_stamina = reader.GetInt32("current_stamina"),
                    mana_cnt = reader.GetInt32("mana_cnt"),
                    diamond_cnt = reader.GetInt32("diamond_cnt"),
                    last_stamina_update = reader.GetDateTime("last_stamina_update"),
                    inventory = reader.IsDBNull(reader.GetOrdinal("inventory")) ? "" : reader.GetString("inventory"),
                    now_emblem = reader.IsDBNull(reader.GetOrdinal("now_emblem")) ? 0 : reader.GetInt32("now_emblem")
                };

                // 将玩家数据保存到本地
                string jsonStr = JsonMapper.ToJson(playerInfo);
                string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
                File.WriteAllText(filePath, jsonStr);
                // 将玩家仓库数据保存到本地
                PlayerInventoryInfo playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(playerInfo.inventory); ;
                string jsonInventoryStr = JsonMapper.ToJson(playerInventoryInfo);
                string inventoryPath = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
                File.WriteAllText(inventoryPath, jsonInventoryStr);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"初始化失败: {e.Message}\n堆栈跟踪：{e.StackTrace}");
        }
    }
    #endregion

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
