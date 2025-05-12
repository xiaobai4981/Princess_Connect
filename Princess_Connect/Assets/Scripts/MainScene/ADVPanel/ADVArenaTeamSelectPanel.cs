using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ADVArenaTeamSelectPanel : BasePanel
{
    private string nowPushBtn = "all";
    public GameObject[] allSelected;
    public Sprite defaultCharacterSprite;
    public Sprite defaultRankSprite;
    private int ce = 0;
    public TMP_Text ceCount;


    private PlayerCharacterCollection playerCharacterCollection;
    private List<bool> selectedList = new List<bool>();

    private List<CharacterFactoryTeamCardData> selectedIndex;

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
    private List<CharacterFactoryTeamCardData> cardDatas = new List<CharacterFactoryTeamCardData>();
    void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        selectedIndex = new List<CharacterFactoryTeamCardData>();
        InitializeButtons("AllBtn");
        allCharacter = CharacterDataMgr.Instance.GetCharacterFactoryData(nowPlayerName);
        for (int i = 0; i < allCharacter.Count; i++) selectedList.Add(false);
        Refresh();
    }
    // 类型不同的道具仓库卡牌初始化
    private void PrepareCardDatas(string nowCharacterType, bool isSync = false)
    {
        cardDatas.Clear();
        for (int i = 0; i < allCharacter.Count; i++)
        {
            CharacterFactoryData data = allCharacter[i];
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
                string characterSpriteFileName = $"icon_unit_{data.characterId}" + rankNum;
                // 加载角色卡片图片
                ABResMgr.Instance.LoadResAsync<Sprite>("character_icon", characterSpriteFileName, (res) =>
                {
                    characterSprite = res;
                }, isSync);
                // 加载边框
                ABResMgr.Instance.LoadResAsync<Sprite>("rank_frame", $"character_rank_{data.characterRank}", (res) =>
                {
                    rankSprite = res;
                }, isSync);
                cardDatas.Add(new CharacterFactoryTeamCardData(
                    i,
                    data.characterId,
                    playerCharacterCollection.characters[data.characterId.ToString()],
                    characterSprite,
                    rankSprite,
                    data.characterRank,
                    selectedList[i]
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

    private void CreateCard(CharacterFactoryTeamCardData data, bool isSync = false)
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "ADVTeamSelectCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            if (data.isSelected)
            {
                cardObj.transform.GetComponent<Image>().color = Color.gray;
                cardObj.transform.GetComponent<Button>().interactable = false;
            }
            // 按钮初始化
            Transform cardBtn = cardObj.transform;
            // 按钮绑定角色图片,其子组件绑定边框图片
            Button btn = cardBtn.GetComponent<Button>();
            btn.image.sprite = data.characterSprite;
            cardBtn.Find("Rarity").GetComponent<Image>().sprite = data.rankSprite;
            // 按钮绑定事件
            ADVTeamSelectCard card = cardObj.GetComponent<ADVTeamSelectCard>();
            card.Initialize(data);
            card.OnCharcterClicked.AddListener(OnCardSelected);
        }, isSync);
    }
    private void OnCardSelected(CharacterFactoryTeamCardData data)
    {
        if (selectedIndex.Count < allSelected.Length)
        {
            selectedIndex.Add(data);
            selectedList[data.id] = true;
            // 降序排序
            selectedIndex.Sort((a, b) => b.characterData.current_stats.stand_pos.CompareTo(a.characterData.current_stats.stand_pos));
            ce += data.characterData.current_stats.ce;
            ceCount.text = ce.ToString();
            Refresh();
        }
    }

    private void Update()
    {
        if (selectedIndex.Count > 0)
        {
            this.transform.Find("BeginBtn").GetComponent<Image>().color = Color.white;
            this.transform.Find("BeginBtn").GetComponent<Button>().interactable = true;
        }
        else
        {
            this.transform.Find("BeginBtn").GetComponent<Image>().color = Color.gray;
            this.transform.Find("BeginBtn").GetComponent<Button>().interactable = false;
        }
        for (int i = 0; i < selectedIndex.Count; i++)
        {
            allSelected[i].GetComponent<Image>().sprite = selectedIndex[i].characterSprite;
            allSelected[i].transform.Find("Rarity").GetComponent<Image>().sprite = selectedIndex[i].rankSprite;
        }
        for (int i = selectedIndex.Count; i < allSelected.Length; i++)
        {
            allSelected[i].GetComponent<Image>().sprite = defaultCharacterSprite;
            allSelected[i].transform.Find("Rarity").GetComponent<Image>().sprite = defaultRankSprite;
        }
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
            case "CharacterOne":
                if (selectedIndex.Count >= 1)
                {
                    SelectedIndexChange(0);
                }
                break;
            case "CharacterTwo":
                if (selectedIndex.Count >= 2)
                {
                    SelectedIndexChange(1);
                }
                break;
            case "CharacterThree":
                if (selectedIndex.Count >= 3)
                {
                    SelectedIndexChange(2);
                }
                break;
            case "CharacterFour":
                if (selectedIndex.Count >= 4)
                {
                    SelectedIndexChange(3);
                }
                break;
            case "CharacterFive":
                if (selectedIndex.Count >= 5)
                {
                    SelectedIndexChange(4);
                }
                break;
            case "BeginBtn":
                TeamSetting();
                break;
            case "CancelBtn":
                UIMgr.Instance.HidePanel<ADVArenaTeamSelectPanel>(true);
                break;
        }

    }

    private void SelectedIndexChange(int index)
    {
        ce -= selectedIndex[index].characterData.current_stats.ce;
        ceCount.text = ce.ToString();
        selectedList[selectedIndex[index].id] = false;
        selectedIndex.RemoveAt(index);
        Refresh();
    }

    private void TeamSetting()
    {
        ArenaConfig arenaList = new ArenaConfig() { arena_deploy = new List<int>() };
        for (int i = 0; i < selectedIndex.Count; i++)
        {
            arenaList.arena_deploy.Add(selectedIndex[i].characterData.character_id);
        }
        string json = JsonMapper.ToJson(arenaList);
        PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>() { { "arena_deploy", json } }, false);
        UIMgr.Instance.HidePanel<ADVArenaTeamSelectPanel>(true);
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

    }
}
