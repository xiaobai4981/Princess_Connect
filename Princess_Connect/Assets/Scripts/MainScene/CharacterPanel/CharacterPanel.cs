using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanel : BasePanel
{
    private string nowPushBtn = "all";
    #region 按钮初始化
    // 按钮背景的字典
    private Dictionary<string, Button> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    private bool isFirst = true;

    // 初始化按钮
    private void InitializeButtons(string defaultBtnName)
    {
        Transform ModeBtn = transform.Find("TopBtn");
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
        Transform ModeBtn = transform.Find("TopBtn");
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
    #region 角色卡片初始化
    // Content
    public Transform contentParent;
    // 记录所有的角色
    private List<CharacterFactoryData> allCharacter = new List<CharacterFactoryData>();
    // 暂存按钮数据
    private List<CharacterFactoryCardData> cardDatas = new List<CharacterFactoryCardData>();
    void OnEnable()
    {
        allCharacter = CharacterDataMgr.Instance.GetCharacterFactoryData(nowPlayerName);
        Refresh();
    }
    void Start()
    {
        InitializeButtons("AllBtn");
        allCharacter = CharacterDataMgr.Instance.GetCharacterFactoryData(nowPlayerName);
        Refresh();
    }
    // 类型不同的道具仓库卡牌初始化
    private void PrepareCardDatas(string nowCharacterType, bool isSync = false)
    {
        cardDatas.Clear();
        foreach (CharacterFactoryData data in allCharacter)
        {
            if (nowCharacterType == "all" || data.characterType == nowCharacterType)
            {
                Sprite characterSprite = null;
                Sprite rankSprite = null;
                string rankNum;
                if (data.characterStar <= 3)
                {
                    rankNum = "11";
                }
                else if (data.characterStar <= 5)
                {
                    rankNum = "31";
                }
                else
                {
                    rankNum = "61";
                }
                string characterSpriteFileName = $"unit_plate_{data.characterId}" + rankNum;
                // 加载角色卡片图片
                ABResMgr.Instance.LoadResAsync<Sprite>("character_plate", characterSpriteFileName, (res) =>
                {
                    characterSprite = res;
                }, isSync);
                // 加载边框
                ABResMgr.Instance.LoadResAsync<Sprite>("rank_frame", $"character_rank_{data.characterRank}", (res) =>
                {
                    rankSprite = res;
                }, isSync);
                cardDatas.Add(new CharacterFactoryCardData(
                    data.characterId,
                    characterSprite,
                    data.characterStar,
                    rankSprite,
                    data.characterRank
                ));
            }
        }
    }

    private void GenerateAllCards()
    {
        // 清除现有卡牌（如果有）
        ClearAllCards();

        // 生成所有卡牌
        foreach (var cardData in cardDatas)
        {
            CreateCard(cardData);
        }
    }

    private void CreateCard(CharacterFactoryCardData data, bool isSync = false)
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "CharacterCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            // 按钮初始化
            Transform cardBtn = cardObj.transform.Find("CharacterBtn");
            // 按钮绑定角色图片,其子组件绑定边框图片
            Button btn = cardBtn.GetComponent<Button>();
            btn.image.sprite = data.characterSprite;
            cardBtn.Find("Rarity").GetComponent<Image>().sprite = data.rankSprite;
            // 按钮绑定事件
            CharacterCard card = cardObj.GetComponent<CharacterCard>();
            card.Initialize(data);
            card.OnCharcterClicked.AddListener(OnCardSelected);
        }, isSync);
    }
    private void OnCardSelected(CharacterFactoryCardData data)
    {
        // todo: 角色卡片点击事件
        UIMgr.Instance.HidePanel<CharacterPanel>(false);
        UIMgr.Instance.ShowPanel<CharacterDevelopPanel>(E_UILayer.Bottom, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            panel.UpdateCharacterId(data.characterId);
        });
    }

    private void ClearAllCards()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
    #endregion
    // 改写Button的OnClick方法
    protected override void ClickBtn(string buttonName)
    {
        switch (buttonName)
        {
            case "AllBtn":
                SetActiveButton("AllBtn");
                nowPushBtn = "all";
                Refresh();
                break;
            case "VanguardBtn":
                SetActiveButton("VanguardBtn");
                nowPushBtn = "vanguard";
                Refresh();
                break;
            case "CenterBtn":
                SetActiveButton("CenterBtn");
                nowPushBtn = "center";
                Refresh();
                break;
            case "DefenderBtn":
                SetActiveButton("DefenderBtn");
                nowPushBtn = "defender";
                Refresh();
                break;
        }
    }
    public void Refresh()
    {
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
        if (MusicMgr.Instance.GetNowBKMusicName() != "CharacterBG")
            MusicMgr.Instance.PlayBKMusic("CharacterBG");
    }
}
