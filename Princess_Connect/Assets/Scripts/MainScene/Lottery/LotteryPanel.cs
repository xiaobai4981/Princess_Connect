using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LotteryPanel : BasePanel
{
    // 需要更新的
    public TMP_Text playerDiamondHave;
    public TMP_Text timeLimit;
    public GameObject NormalBtn;
    public GameObject OneTime;
    public GameObject TenTimes;
    // 按钮背景的字典
    private Dictionary<string, Button> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    // 两种扭蛋的页面
    public GameObject CharacterLot;
    public GameObject NormalLot;
    private bool isFirst = true;
    // Start is called before the first frame update
    void Start()
    {
        playerDiamondHave.text = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "diamond_cnt").ToString();
        InitializeButtons("CharacterLotBtn");
    }

    private void Update()
    {
        DateTime now = DateTime.Now;
        DateTime midnightTonight = now.Date.AddDays(1);
        TimeSpan timeUntilMidnight = midnightTonight - now;
        if (NormalLot.activeSelf)
        {
            string questStatus = MissionDataMgr.Instance.GetPlayerQuestAnInfo(nowPlayerName, 1004, "status");
            if (questStatus == "in_progress")
            {
                NormalBtn.transform.GetComponent<Button>().interactable = true;
                NormalBtn.transform.GetComponent<Image>().color = Color.white;
            }
            else
            {
                NormalBtn.transform.GetComponent<Button>().interactable = false;
                NormalBtn.transform.GetComponent<Image>().color = Color.gray;
            }
            if (timeUntilMidnight == TimeSpan.Zero)
            {
                // 刷新扭蛋
                NormalBtn.transform.GetComponent<Button>().interactable = true;
                NormalBtn.transform.GetComponent<Image>().color = Color.white;
            }
        }
        else
        {
            int nowDiamond = int.Parse(playerDiamondHave.text);
            if (nowDiamond >= 1500)
            {
                OneTime.transform.GetComponent<Button>().interactable = true;
                OneTime.transform.GetComponent<Image>().color = Color.white;
                TenTimes.transform.GetComponent<Button>().interactable = true;
                TenTimes.transform.GetComponent<Image>().color = Color.white;
            }
            else if (nowDiamond >= 150)
            {
                OneTime.transform.GetComponent<Button>().interactable = true;
                OneTime.transform.GetComponent<Image>().color = Color.white;
                TenTimes.transform.GetComponent<Button>().interactable = false;
                TenTimes.transform.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                OneTime.transform.GetComponent<Button>().interactable = false;
                OneTime.transform.GetComponent<Image>().color = Color.gray;
                TenTimes.transform.GetComponent<Button>().interactable = false;
                TenTimes.transform.GetComponent<Image>().color = Color.gray;
            }
        }
        string timeStr = timeUntilMidnight.ToString("hh\\:mm\\:ss");
        timeLimit.text = timeStr;
    }
    // 初始化按钮
    private void InitializeButtons(string defaultBtnName)
    {
        Transform ModeBtn = transform.Find("LotteryPart/LotteryModeBtn");
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
        CharacterLot.SetActive(true);
        NormalLot.SetActive(false);
    }

    // 重写Click方法
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "CharacterLotBtn":
                SetActiveButton("CharacterLotBtn");
                break;
            case "NormalLotBtn":
                SetActiveButton("NormalLotBtn");
                break;
            case "NormalBtn":
                NormalLottery(10, false);
                break;
            case "OneTime":
                if (OneTime.transform.GetComponent<Button>().interactable)
                    CharacterLottery(1, true);
                break;
            case "TenTimes":
                if (TenTimes.transform.GetComponent<Button>().interactable)
                    CharacterLottery(10, true);
                break;
        }
    }

    private void NormalLottery(int count, bool isCharacterLottery)
    {
        // 准备数据
        List<int> result = GetRandomList(count, isCharacterLottery);
        // 传输数据
        UIMgr.Instance.HidePanel<LotteryPanel>();
        UIMgr.Instance.ShowPanel<LotteryResultPanel>(E_UILayer.Middle, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            panel.UpdateLotteryResult(result, false);
        });
        // 触发扭蛋刷新
        NormalBtn.transform.GetComponent<Button>().interactable = false;
        NormalBtn.transform.GetComponent<Image>().color = Color.gray;
        EventCenter.Instance.EventTrigger<string>(E_EventType.E_Lottery_Quest_Update, nowPlayerName);
    }

    private void CharacterLottery(int count, bool isCharacterLottery)
    {
        // 准备数据
        List<int> result = GetRandomList(count, isCharacterLottery);
        
        // 修改玩家数据
        int nowDiamond = int.Parse(playerDiamondHave.text) - count * 150;
        PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>() { { "diamond_cnt", nowDiamond } }, false);
        TMP_Text diamondText = GetControl<TMP_Text>("DiamondText");
        diamondText.text = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "diamond_cnt").ToString("N0");

        // 传输数据
        UIMgr.Instance.HidePanel<LotteryPanel>();
        UIMgr.Instance.ShowPanel<LotteryResultPanel>(E_UILayer.Middle, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            panel.UpdateLotteryResult(result, true);
        });
    }

    private List<int> GetRandomList(int count, bool isCharacterLottery)
    {
        List<int> result = new List<int>();
        for (int i = 0; i < count; i++)
        {
            result.Add(LotteryList.GetRandomLotteryItem(isCharacterLottery));
        }
        return result;
    }

    public void SetActiveButton(string btnName)
    {
        if (btnName == "CharacterLotBtn")
        {
            CharacterLot.SetActive(true);
            NormalLot.SetActive(false);
        }
        else
        {
            CharacterLot.SetActive(false);
            NormalLot.SetActive(true);
        }
        if (!buttonDataDict.TryGetValue(btnName, out var activeData)) return;
        Transform ModeBtn = transform.Find("LotteryPart/LotteryModeBtn");
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

    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    public override void HideMe()
    {

    }

    public override void ShowMe()
    {
        playerDiamondHave.text = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "diamond_cnt").ToString();
        if (MusicMgr.Instance.GetNowBKMusicName() != "LotteryNormalBG")
        {
            MusicMgr.Instance.PlayBKMusic("LotteryNormalBG");
        }
    }
}
