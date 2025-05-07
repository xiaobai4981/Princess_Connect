using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeginQuestPanel : BasePanel
{
    public TMP_Text dailyTimeLimit;
    private void Update()
    {
        DateTime now = DateTime.Now;
        DateTime midnightTonight = now.Date.AddDays(1);
        TimeSpan timeUntilMidnight = midnightTonight - now;
        if (timeUntilMidnight == TimeSpan.Zero)
        {
            if (nowPushBtn == "daily")
            {
                Refresh();
            }
        }
        string timeStr = timeUntilMidnight.ToString("hh\\:mm\\:ss");
        dailyTimeLimit.text = timeStr;
    }

    private string nowPushBtn = "daily";
    #region 按钮初始化
    // 按钮背景的字典
    private Dictionary<string, Button> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    private bool isFirst = true;

    // 初始化按钮
    private void InitializeButtons(string defaultBtnName)
    {
        Transform ModeBtn = transform.Find("BtnArea");
        if (isFirst)
        {
            // 保存按钮的原始图片
            Button[] buttons = GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                if (btn.transition == Selectable.Transition.SpriteSwap)
                {
                    Image img = btn.GetComponent<Image>();
                    if (img != null)
                    {
                        originalSprites[btn] = img.sprite;
                    }
                }
            }
            // 保存按钮数据
            buttonDataDict = new Dictionary<string, Button>();
            foreach (Transform child in ModeBtn)
            {
                buttonDataDict[child.name] = child.GetComponent<Button>();
            }
            isFirst = false;
        }
        else
        {
            foreach (var pair in buttonDataDict)
            {
                Button btn = ModeBtn.Find(pair.Key).GetComponent<Button>();
                Image btnImg = btn.GetComponent<Image>();
                btnImg.sprite = originalSprites[btn];
            }
        }
        // 设置默认按钮为按下状态
        Transform defaultBtn = ModeBtn.Find(defaultBtnName);
        if (defaultBtn != null)
        {
            Button btn = defaultBtn.GetComponent<Button>();
            if (btn != null)
            {
                SpriteState spriteState = btn.spriteState;
                Image btnImage = btn.GetComponent<Image>();
                btnImage.sprite = spriteState.pressedSprite;
            }
        }
    }
    public void SetActiveButton(string btnName)
    {
        if (!buttonDataDict.TryGetValue(btnName, out var activeData)) return;
        Transform ModeBtn = transform.Find("BtnArea");
        foreach (var pair in buttonDataDict)
        {
            bool isActive = pair.Key == btnName;
            // 手动设置 Sprite（不依赖 Button 组件）
            Image btnImage = pair.Value.GetComponent<Image>();
            btnImage.sprite = isActive ?
            pair.Value.spriteState.pressedSprite :
            originalSprites[pair.Value];
            if (!isActive)
            {
                Button btn = ModeBtn.Find(pair.Key).GetComponent<Button>();
                Image btnImg = btn.GetComponent<Image>();
                btnImg.sprite = originalSprites[btn];
            }
        }
    }
    #endregion
    #region 任务卡片初始化
    // Content
    public Transform contentParent;
    // 记录所有的任务
    private List<BeginQuestCardData> allQuestDatas = new List<BeginQuestCardData>();
    private List<BeginQuestCardData> cardQuestDatas = new List<BeginQuestCardData>();
    void Start()
    {
        InitializeButtons("DailyBtn");
        allQuestDatas = QuestListener.Instance.GetQuestDatas(nowPlayerName);
        EventCenter.Instance.AddEventListener(E_EventType.E_Quest_Confirm, Refresh);
        Refresh();
    }
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.E_Quest_Confirm, Refresh);
    }
    // 类型不同的道具仓库卡牌初始化
    private void PrepareCardDatas(string nowQuestType, bool isSync = false)
    {
        cardQuestDatas.Clear();
        foreach (BeginQuestCardData questData in allQuestDatas)
        {
            if (questData.questType != nowQuestType)
            {
                continue;
            }
            string questType;
            if (nowQuestType == "daily")
            {
                questType = "每日";
            }else if (nowQuestType == "normal")
            {
                questType = "通用";
            }
            else
            {
                questType = "称号";
            }
            BeginQuestCardData cardData = new BeginQuestCardData(questData.questId, questData.questStat
                , questType, questData.questDesc, questData.questNowProgress, questData.questCompleteProgress
                , questData.questRewards);
            foreach (BeginQuestCardReward reward in cardData.questRewards)
            {
                if (reward.gloryNum == -1)
                {
                    ABResMgr.Instance.LoadResAsync<Sprite>("item", $"icon_item_{reward.exceptGloryIconId}", (sprite) =>
                    {
                        reward.exceptGloryIcon = sprite;
                    }, true);
                }
                else
                {
                    ABResMgr.Instance.LoadResAsync<Sprite>("item", "icon_item_01003", (sprite) =>
                    {
                        reward.exceptGloryIcon = sprite;
                    }, true);
                }
            }
            cardQuestDatas.Add(cardData);
        }
    }

    private void GenerateAllCards()
    {
        // 清除现有卡牌（如果有）
        ClearAllCards();

        // 生成所有卡牌
        foreach (var cardData in cardQuestDatas)
        {
            CreateCard(cardData);
        }
    }

    private void CreateCard(BeginQuestCardData data, bool isSync = false)
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "BeginQuestCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            BeginQuestCard card = cardObj.GetComponent<BeginQuestCard>();
            card.Initialize(data);
        }, isSync);
    }

    private void ClearAllCards()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion


    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "DailyBtn":
                SetActiveButton("DailyBtn");
                nowPushBtn = "daily";
                Refresh();
                break;
            case "NormalBtn":
                SetActiveButton("NormalBtn");
                nowPushBtn = "normal";
                Refresh();
                break;
            case "GloryBtn":
                SetActiveButton("GloryBtn");
                nowPushBtn = "achievement";
                Refresh();
                break;
            case "BackToBegin":
                UIMgr.Instance.HidePanel<BeginQuestPanel>(true);
                UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Bottom, (panel) =>
                {
                    panel.UpdatePlayerInfo();
                });
                break;
        }
    }
    public void Refresh()
    {
        allQuestDatas = QuestListener.Instance.GetQuestDatas(nowPlayerName);
        PrepareCardDatas(nowPushBtn);
        GenerateAllCards();
    }
    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
