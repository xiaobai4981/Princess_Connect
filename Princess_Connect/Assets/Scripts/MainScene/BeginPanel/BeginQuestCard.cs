using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeginQuestCardReward
{
    public Sprite exceptGloryIcon;
    public int exceptGloryIconId;
    public int exceptGloryNum;
    public int gloryNum = -1;
}

public class BeginQuestCardData
{
    public int questId;
    public string questStat;
    public string questType;
    public string questDesc;
    public int questNowProgress;
    public int questCompleteProgress;
    public List<BeginQuestCardReward> questRewards;
    public BeginQuestCardData(int questId, string questStat, string questType, string questDesc, int questNowProgress, int questCompleteProgress, List<BeginQuestCardReward> questRewards)
    {
        this.questId = questId;
        this.questStat = questStat;
        this.questType = questType;
        this.questDesc = questDesc;
        this.questNowProgress = questNowProgress;
        this.questCompleteProgress = questCompleteProgress;
        this.questRewards = questRewards;
    }
}


public class BeginQuestCard : MonoBehaviour
{
    public TMP_Text questTypeText;
    public TMP_Text questDescText;
    public TMP_Text questProgressText;
    public RectTransform progressFill;
    public GameObject[] rewards;
    public Button confiremBtn;
    BeginQuestCardData _data;

    PlayerInfo nowPlayerInfo;
    PlayerInventoryInfo playerInventoryInfo;

    private void Awake()
    {
        confiremBtn.onClick.AddListener(OnCardButtonClicked);
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_data.json");
        nowPlayerInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath1));
        string filePath2 = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(File.ReadAllText(filePath2));
    }

    private void Start()
    {
        if (_data.questStat == "confirm")
        {
            this.gameObject.GetComponent<Image>().color = Color.gray;
            confiremBtn.GetComponent<Image>().color = Color.gray;
            confiremBtn.interactable = false;
        }
        else if (_data.questStat == "completed")
        {
            confiremBtn.GetComponent<Image>().color = Color.white;
            confiremBtn.interactable = true;
        }
        else
        {
            this.gameObject.GetComponent<Image>().color = Color.white;
            confiremBtn.GetComponent<Image>().color = Color.gray;
            confiremBtn.interactable = false;
        }
    }

    public void Initialize(BeginQuestCardData data)
    {
        _data = data;
        questTypeText.text = data.questType;
        questDescText .text = data.questDesc;
        questProgressText .text = data.questNowProgress + "/" + data.questCompleteProgress;
        progressFill.sizeDelta = new Vector2((float)data.questNowProgress / (float)data.questCompleteProgress * 250f, progressFill.sizeDelta.y);
        if (data.questRewards[0].gloryNum != -1)
        {
            for (int i = 1; i < rewards.Length; i++)
            {
                rewards[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < data.questRewards.Count; i++)
            {
                rewards[i].GetComponent<Image>().sprite = data.questRewards[i].exceptGloryIcon;
                if (data.questRewards[i].gloryNum != -1)
                {
                    rewards[i].transform.Find("RewardNum" + (i + 1).ToString()).GetComponentInChildren<TMP_Text>().text = "x1";
                }
                else
                {
                    rewards[i].transform.Find("RewardNum" + (i + 1).ToString()).GetComponentInChildren<TMP_Text>().text = "x" + data.questRewards[i].exceptGloryNum.ToString();
                }
            }
            for (int i = data.questRewards.Count; i < rewards.Length; i++)
            {
                rewards[i].SetActive(false);
            }
        }
    }
    private void OnDestroy()
    {
        confiremBtn.onClick.RemoveListener(OnCardButtonClicked);
    }
    public void OnCardButtonClicked()
    {
        PlayerQuestInfo thisQuestInfo = new PlayerQuestInfo();
        thisQuestInfo.progress_data = new QuestProgress() { now_progress = -1, complete_progress = -1 };
        thisQuestInfo.last_updated = System.DateTime.Now;
        thisQuestInfo.status = "confirm";
        MissionDataMgr.Instance.UpdateUserQuest(nowPlayerInfo.username, _data.questId, thisQuestInfo);
        foreach (BeginQuestCardReward reward in _data.questRewards)
        {
            // 非称号奖励
            if (reward.gloryNum == -1)
            {
                // mana奖励
                if (reward.exceptGloryIconId == 94001)
                {
                    nowPlayerInfo.mana_cnt += reward.exceptGloryNum;
                }
                // 钻石奖励
                else
                {
                    nowPlayerInfo.diamond_cnt += reward.exceptGloryNum;
                }
            }
            // 称号奖励
            else
            {
                playerInventoryInfo.glory.Add(reward.gloryNum);
            }
        }
        // 保存并上传数据
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_data.json");
        File.WriteAllText(filePath1, JsonMapper.ToJson(nowPlayerInfo));
        PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerInfo.username, new Dictionary<string, object>() { { "mana_cnt", nowPlayerInfo.mana_cnt },
            { "diamond_cnt", nowPlayerInfo.diamond_cnt } }, false);


        string filePath2 = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        File.WriteAllText(filePath2, JsonMapper.ToJson(playerInventoryInfo));
        string inventoryData = File.ReadAllText(filePath2);
        GloryDataMgr.Instance.UpdateUserPlayerFactoryInfo(nowPlayerInfo.username, inventoryData);
        // 触发任务刷新
        EventCenter.Instance.EventTrigger(E_EventType.E_Quest_Confirm);

    }

}
