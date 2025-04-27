using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuItemPanel : BasePanel
{
    // 当前玩家数据
    PlayerInventoryInfo playerInventoryInfo;
#region 控制按钮初始化
    // 按钮背景的字典
    private Dictionary<string, Button> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    private bool isFirst = true;
    void OnEnable()
    {
        InitializeButtons("ItemBtn");
    }
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
    #region 外部控制组件
    // 道具图片
    [SerializeField] private Image externalDisplayImage;
    [SerializeField] private TMP_Text externalImageNum;
    // 道具的名字
    [SerializeField] private TMP_Text itemName;
    // 道具的描述
    [SerializeField] private TMP_Text itemDescription;
#endregion
#region 道具卡牌初始化
    // Content
    public Transform contentParent;
    // 记录所有的道具
    private Dictionary<string, ItemInfo> allItems = new Dictionary<string, ItemInfo>();
    // 暂存按钮数据
    private List<ItemCardData> cardDatas = new List<ItemCardData>();
    void Start()
    {
        // 得到当前玩家的仓库信息
        string playerInventory = GloryDataMgr.Instance.SearchUserPlayerFactoryInfo(nowPlayerName, "inventory");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(playerInventory);
        // 读取道具数据
        foreach (string itemIdStr in playerInventoryInfo.itemDic.Keys)
        {
            allItems.Add(itemIdStr, playerInventoryInfo.itemDic[itemIdStr]);
        }
        FetchPlayerOwnedCards(() => {
            PrepareCardDatas("item");
            GenerateAllCards();
        });
    }
    private void FetchPlayerOwnedCards(System.Action onComplete)
    {
        // 模拟网络请求延迟
        Invoke(() => {
            onComplete?.Invoke();
        }, 0.3f);
    }
    // 辅助方法：模拟延迟回调
    private void Invoke(System.Action action, float delay)
    {
        StartCoroutine(InvokeRoutine(action, delay));
    }

    private System.Collections.IEnumerator InvokeRoutine(System.Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
    // 类型不同的道具仓库卡牌初始化
    private void PrepareCardDatas(string nowItemType, bool isSync = false)
    {
        cardDatas.Clear();
        foreach (string cardIdStr in allItems.Keys)
        {
            int itemId = int.Parse(cardIdStr);
            // 每个dic有两条，第一条是道具类型，第二条是道具数量
            string itemType = allItems[cardIdStr].type;
            int itemNum = allItems[cardIdStr].num;
            if (itemType == nowItemType)
            {
                Sprite itemSprite = null;
                string fileName = nowItemType != "unit_material"? $"icon_{nowItemType}_{itemId}" : $"{nowItemType}_no_frame_{itemId}";
                ABResMgr.Instance.LoadResAsync<Sprite>(nowItemType, fileName, (res) =>
                {
                    itemSprite = res;
                }, isSync);
                cardDatas.Add(new ItemCardData(
                    itemId,
                    itemNum,
                    itemSprite
                ));
            }
            if (cardDatas.Count == 1)
            {
                externalDisplayImage.sprite = cardDatas[0].itemIcon;
                externalImageNum.text = "x" + cardDatas[0].itemNum.ToString();
                Dictionary<string, string> description = GloryDataMgr.Instance.GetItemDescription(itemId);
                itemName.text = description["name"];
                itemDescription.text = description["description"];
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

    private void CreateCard(ItemCardData data, bool isSync = false)
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "MenuItemCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            // 按钮初始化
            Transform cardBtn = cardObj.transform.Find("ItemBtn");
            // 按钮绑定图片
            Button btn = cardBtn.GetComponent<Button>();
            btn.image.sprite = data.itemIcon;
            // 按钮绑定文字
            TMP_Text text = cardBtn.GetComponentInChildren<TMP_Text>();
            text.text = "x" + data.itemNum.ToString();
            // 按钮绑定事件
            MenuItemCard card = cardObj.GetComponent<MenuItemCard>();
            card.Initialize(data);
            card.OnItemClicked.AddListener(OnCardSelected);
        }, isSync);
    }
    private void OnCardSelected(ItemCardData data)
    {
        if (externalDisplayImage != null)
        {
            // 获取更改道具的描述和条件描述
            Dictionary<string, string> descriptions = GloryDataMgr.Instance.GetItemDescription(data.itemId);
            // 更改外部控制量信息
            externalDisplayImage.sprite = data.itemIcon;
            externalImageNum.text = "x" + data.itemNum.ToString();
            itemName.text = descriptions["name"];
            itemDescription.text = descriptions["description"];
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
    // 改写Button的OnClick方法
    protected override void ClickBtn(string buttonName)
    {
        switch (buttonName)
        {
            case "ItemBtn":
                SetActiveButton("ItemBtn");
                FetchPlayerOwnedCards(() => {
                    PrepareCardDatas("item");
                    GenerateAllCards();
                });
                break;
            case "EquipmentBtn":
                SetActiveButton("EquipmentBtn");
                FetchPlayerOwnedCards(() => {
                    PrepareCardDatas("equipment");
                    GenerateAllCards();
                });
                break;
            case "UnitMaterialBtn":
                SetActiveButton("UnitMaterialBtn");
                FetchPlayerOwnedCards(() => {
                    PrepareCardDatas("unit_material");
                    GenerateAllCards();
                });
                break;
            case "BackToMenu":
                UIMgr.Instance.HidePanel<MenuItemPanel>(true);
                UIMgr.Instance.ShowPanel<MenuPanel>();
                break;
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
        
    }
}
