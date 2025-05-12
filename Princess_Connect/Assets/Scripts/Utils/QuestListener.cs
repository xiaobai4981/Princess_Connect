using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
public class QuestListener : MonoBehaviour
{
    private static QuestListener instance;
    private QuestConfig questConfig;
    private PlayerInfo playerData;
    private PlayerCharacterCollection playerCharacterCollection;
    private void Awake()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "quest_config.json");
        questConfig = JsonMapper.ToObject<QuestConfig>(File.ReadAllText(filePath));
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_data.json");
        playerData = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath1));
        string filePath2 = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath2));
        EventCenter.Instance.AddEventListener<string>(E_EventType.E_Lottery_Quest_Update, UpdateLotteryProgress);
        EventCenter.Instance.AddEventListener<string>(E_EventType.E_SkillUp_Quest_Update, UpdateSkillUpProgress);
        if (instance != null)
        {
            Destroy(gameObject); // 防止重复创建
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // 跨场景不销毁
    }
    private void Update()
    {
        DateTime now = DateTime.Now;
        DateTime midnightTonight = now.Date.AddDays(1);
        TimeSpan timeUntilMidnight = midnightTonight - now;
        if (timeUntilMidnight == TimeSpan.Zero)
        {
            PlayerDataMgr.Instance.ModifyUserIntInfo(playerData.username, new Dictionary<string, object>() {{"clear_stage_times", 0}, { "search_times", 5 }}
            , false);
            for (int i = 1001; i <= 1005; i++)
            {
                PlayerQuestInfo playerQuestInfo = new PlayerQuestInfo();
                if (i == 1002)
                {
                    playerQuestInfo.progress_data.now_progress = 0;
                    playerQuestInfo.progress_data.complete_progress = 10;
                    playerQuestInfo.status = "in_progress";
                    playerQuestInfo.last_updated = now;
                }
                else if (i == 1003)
                {
                    playerQuestInfo.progress_data.now_progress = 0;
                    playerQuestInfo.progress_data.complete_progress = 20;
                    playerQuestInfo.status = "in_progress";
                    playerQuestInfo.last_updated = now;
                }
                else
                {
                    if (i == 1001)
                    {
                        playerQuestInfo.progress_data.now_progress = 1;
                        playerQuestInfo.progress_data.complete_progress = 1;
                        playerQuestInfo.status = "completed";
                        playerQuestInfo.last_updated = now;
                    }
                    else
                    {
                        playerQuestInfo.progress_data.now_progress = 0;
                        playerQuestInfo.progress_data.complete_progress = 1;
                        playerQuestInfo.status = "in_progress";
                        playerQuestInfo.last_updated = now;
                    }
                }
                MissionDataMgr.Instance.UpdateUserQuest(playerData.username, i, playerQuestInfo);
            }
        }
    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.E_Lottery_Quest_Update, UpdateLotteryProgress);
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.E_SkillUp_Quest_Update, UpdateSkillUpProgress);
        if (instance == this)
        {
            instance = null;
        }
    }
    public static QuestListener Instance => instance;
    // 更新当前玩家的任务进度
    public void UpdateQuestProgress(string nowPlayerName)
    {
        PlayerDataMgr.Instance.InitPlayerData(nowPlayerName);
        CharacterDataMgr.Instance.InitUserCharacterData(nowPlayerName);
        string filePath = Path.Combine(Application.persistentDataPath, "quest_config.json");
        questConfig = JsonMapper.ToObject<QuestConfig>(File.ReadAllText(filePath));
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_data.json");
        playerData = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath1));
        string filePath2 = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath2));
        foreach (var quest in questConfig.quests)
        {
            QuestProgress questProgress = new QuestProgress();
            JsonData jsonData = JsonMapper.ToObject(quest.unlock_condition);
            // 每日任务
            if (quest.quest_id > 1000 && quest.quest_id < 2000)
            {
                // 每日关卡通关任务
                if (jsonData.Count != 0)
                {
                    int needClearTime = (int)jsonData["clearstagetimes"];
                    bool isClear = playerData.clear_stage_times >= needClearTime;
                    questProgress.now_progress = isClear ? needClearTime : playerData.clear_stage_times;
                    questProgress.complete_progress = needClearTime;

                    string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                    if (status == "in_progress")
                    {
                        PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                        thisQuestInfo.progress_data = questProgress;
                        thisQuestInfo.last_updated = System.DateTime.Now;
                        thisQuestInfo.status = isClear ? "completed" : status;
                        MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                    }
                }
                // 其他每日任务 只处理登录任务，另外的两个任务：抽取普通扭蛋和强化角色技能通过外部事件中心处理
                else
                {
                    if (quest.quest_id == 1001)
                    {
                        string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                        if (status == "in_progress")
                        {
                            questProgress.now_progress = 1;
                            questProgress.complete_progress = 1;
                            PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                            thisQuestInfo.progress_data = questProgress;
                            thisQuestInfo.last_updated = System.DateTime.Now;
                            thisQuestInfo.status = "completed";
                            MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                        }
                    }
                    else
                    {
                        string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                        if (status == "in_progress")
                        {
                            questProgress.now_progress = 0;
                            questProgress.complete_progress = 1;
                            PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                            thisQuestInfo.progress_data = questProgress;
                            thisQuestInfo.last_updated = System.DateTime.Now;
                            thisQuestInfo.status = "none";
                            MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                        }
                    }
                }
            }
            // todo 关卡相关任务 暂时搁置不处理
            else if (quest.quest_id > 2000 && quest.quest_id < 3000)
            {
                string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                if (status == "in_progress")
                {
                    questProgress.now_progress = 0;
                    questProgress.complete_progress = 1;
                    PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                    thisQuestInfo.progress_data = questProgress;
                    thisQuestInfo.last_updated = System.DateTime.Now;
                    thisQuestInfo.status = "none";
                    MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                }
            }
            // 玩家等级任务
            else if (quest.quest_id > 3000 && quest.quest_id < 4000)
            {
                int needLevel = (int)jsonData["level"];
                bool isClear = playerData.level >= needLevel;
                questProgress.now_progress = isClear ? needLevel : playerData.level;
                questProgress.complete_progress = needLevel;

                string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                if (status == "in_progress")
                {
                    PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                    thisQuestInfo.progress_data = questProgress;
                    thisQuestInfo.last_updated = System.DateTime.Now;
                    thisQuestInfo.status = isClear ? "completed" : status;
                    MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                }
            }
            // 称号任务
            else
            {
                // 获得角色得到的称号
                if (jsonData.Count == 1)
                {
                    bool isGet = CharacterDataMgr.Instance.SearchCharacterInPlayer(nowPlayerName, (int)jsonData["character_id"]);
                    string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                    if (status == "in_progress")
                    {
                        questProgress.now_progress = isGet ? 1 : 0;
                        questProgress.complete_progress = 1;
                        PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                        thisQuestInfo.progress_data = questProgress;
                        thisQuestInfo.last_updated = System.DateTime.Now;
                        thisQuestInfo.status = isGet ? "completed" : status;
                        MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                    }
                }
                // 角色升星得到的称号
                else
                {
                    bool isGet = CharacterDataMgr.Instance.SearchCharacterInPlayer(nowPlayerName, (int)jsonData["character_id"]);
                    if (isGet)
                    {
                        int nowStar = playerCharacterCollection.characters[jsonData["character_id"].ToString()].current_star;
                        int needStar = (int)jsonData["star"];
                        bool isClear = nowStar >= needStar;
                        string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                        if (status == "in_progress")
                        {
                            questProgress.now_progress = isClear ? needStar : nowStar;
                            questProgress.complete_progress = needStar;
                            PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                            thisQuestInfo.progress_data = questProgress;
                            thisQuestInfo.last_updated = System.DateTime.Now;
                            thisQuestInfo.status = isClear ? "completed" : status;
                            MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo);
                        }
                    }
                    else
                    {
                        string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
                        if (status == "in_progress")
                        {
                            questProgress.now_progress = 0;
                            questProgress.complete_progress = 1;
                            PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
                            thisQuestInfo.progress_data = questProgress;
                            thisQuestInfo.last_updated = System.DateTime.Now;
                            thisQuestInfo.status = "none";
                            MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, quest.quest_id, thisQuestInfo); 
                        }
                    }
                }
            }

        }
    }
    public void UpdateLotteryProgress(string nowPlayerName)
    {
        string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, 1004, "status");
        QuestProgress questProgress = new QuestProgress();
        if (status == "in_progress")
        {
            questProgress.now_progress = 1;
            questProgress.complete_progress = 1;
            PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
            thisQuestInfo.progress_data = questProgress;
            thisQuestInfo.last_updated = System.DateTime.Now;
            thisQuestInfo.status = "completed";
            MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, 1004, thisQuestInfo);
        }
    }
    public void UpdateSkillUpProgress(string nowPlayerName)
    {
        string status = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, 1005, "status");
        QuestProgress questProgress = new QuestProgress();
        if (status == "in_progress")
        {
            questProgress.now_progress = 1;
            questProgress.complete_progress = 1;
            PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
            thisQuestInfo.progress_data = questProgress;
            thisQuestInfo.last_updated = System.DateTime.Now;
            thisQuestInfo.status = "completed";
            MissionDataMgr.Instance.UpdateUserQuest(nowPlayerName, 1005, thisQuestInfo);
        }
    }
    // 将当前玩家任务信息解析成对应的数据结构返回
    public List<BeginQuestCardData> GetQuestDatas(string nowPlayerName)
    {
        List<BeginQuestCardData> allQuestDatas = new List<BeginQuestCardData>();
        foreach (var quest in questConfig.quests)
        {
            string rewardData = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "progress_data");
            QuestProgress questProgress = JsonMapper.ToObject<QuestProgress>(rewardData);
            JsonData jsonData = JsonMapper.ToObject(quest.rewards);
            string questStat = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, quest.quest_id, "status");
            string questType = quest.quest_type;
            string questDesc = quest.description;
            int questNowProgress = questProgress.now_progress;
            int questCompleteProgress = questProgress.complete_progress;
            // 每日任务
            if (quest.quest_id > 1000 && quest.quest_id < 2000)
            {
                if (quest.quest_id >= 1002 && quest.quest_id <= 1003)
                {
                    
                    List<BeginQuestCardReward> rewards = new List<BeginQuestCardReward>() 
                    {
                        new BeginQuestCardReward() {exceptGloryIcon = null, exceptGloryIconId = 91001, exceptGloryNum = (int)jsonData["diamond"], gloryNum = -1}
                    };
                    BeginQuestCardData thisQuestData = new BeginQuestCardData(quest.quest_id, questStat, questType, questDesc, questNowProgress, questCompleteProgress, rewards);
                    allQuestDatas.Add(thisQuestData);
                }
                else
                {
                    List<BeginQuestCardReward> rewards = new List<BeginQuestCardReward>()
                    {
                        new BeginQuestCardReward() {exceptGloryIcon = null, exceptGloryIconId = 94001, exceptGloryNum = (int)jsonData["mana"], gloryNum = -1}
                    };
                    BeginQuestCardData thisQuestData = new BeginQuestCardData(quest.quest_id, questStat, questType, questDesc, questNowProgress, questCompleteProgress, rewards);
                    allQuestDatas.Add(thisQuestData);
                }
            }
            // 关卡相关任务
            else if (quest.quest_id > 2000 && quest.quest_id < 3000)
            {
                List<BeginQuestCardReward> rewards = new List<BeginQuestCardReward>()
                {
                    new BeginQuestCardReward() {exceptGloryIcon = null, exceptGloryIconId = 91001, exceptGloryNum = (int)jsonData["diamond"], gloryNum = -1}
                };
                BeginQuestCardData thisQuestData = new BeginQuestCardData(quest.quest_id, questStat, questType, questDesc, questNowProgress, questCompleteProgress, rewards);
                allQuestDatas.Add(thisQuestData);
            }
            // 玩家等级任务
            else if (quest.quest_id > 3000 && quest.quest_id < 4000)
            {
                List<BeginQuestCardReward> rewards = new List<BeginQuestCardReward>()
                {
                    new BeginQuestCardReward() {exceptGloryIcon = null, exceptGloryIconId = 94001, exceptGloryNum = (int)jsonData["mana"], gloryNum = -1},
                    new BeginQuestCardReward() {exceptGloryIcon = null, exceptGloryIconId = 91001, exceptGloryNum = (int)jsonData["diamond"], gloryNum = -1}
                };
                BeginQuestCardData thisQuestData = new BeginQuestCardData(quest.quest_id, questStat, questType, questDesc, questNowProgress, questCompleteProgress, rewards);
                allQuestDatas.Add(thisQuestData);
            }
            // 称号任务
            else
            {
                List<BeginQuestCardReward> rewards = new List<BeginQuestCardReward>()
                {
                    new BeginQuestCardReward() {exceptGloryIcon = null, exceptGloryIconId = -1, exceptGloryNum = -1, gloryNum = (int)jsonData["glory"]},
                };
                BeginQuestCardData thisQuestData = new BeginQuestCardData(quest.quest_id, questStat, questType, questDesc, questNowProgress, questCompleteProgress, rewards);
                allQuestDatas.Add(thisQuestData);
            }
        }

        return allQuestDatas;
    }
}

