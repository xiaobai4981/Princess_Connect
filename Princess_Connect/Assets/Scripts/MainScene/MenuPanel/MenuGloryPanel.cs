using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MenuGloryPanel : BasePanel
{
    #region 当前的称号显示
    // 当前的称号图片
    [SerializeField] private Image currentGloryImage;
    // 当前的称号描述
    [SerializeField] private TMP_Text NowDescribe;
    // 当前玩家数据
    PlayerInfo playerInfo;
    #endregion
    #region 外部要改变的组件
    // 图片及其编号
    [SerializeField] private Image externalDisplayImage;
    private int externalDisplayImageId;
    // 要更改的称号描述
    [SerializeField] private TMP_Text ChangeDescribe;
    // 要更改的称号获得条件
    [SerializeField] private TMP_Text GetContion;
    #endregion
    // Content
    public Transform contentParent;
    // 玩家拥有的卡牌ID集合
    private HashSet<int> ownedCardIds = new HashSet<int>();
    // 数据库中所有称号的id集合
    private List<int> allGloryIds = new List<int>();
    // 暂存按钮数据
    private List<GloryCardData> cardDatas = new List<GloryCardData>();
    // 生成卡面部分
    void Start()
    {
        externalDisplayImageId = -1;
        // 当前的称号展示
        string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
        playerInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath));
        // 获取玩家拥有的卡牌ID
        FetchPlayerOwnedCards(() => {
            PrepareCardDatas(playerInfo);
            GenerateAllCards();
        });
        allGloryIds = GloryDataMgr.Instance.GetAllGloryIds(); 
    }
    #region 处理卡面初始化, 处理卡面点击事件
    private void FetchPlayerOwnedCards(System.Action onComplete)
    {
        // 模拟网络请求延迟
        Invoke(() => {
            GetPlayerInfo();
            onComplete?.Invoke();
        }, 0.3f);
    }
    // 通过数据库连接找到玩家信息，并将拥有的卡牌ID集合设置为该玩家拥有的卡牌ID集合
    private void GetPlayerInfo()
    {
        // 得到当前玩家的仓库信息
        string playerInventory = GloryDataMgr.Instance.SearchUserPlayerFactoryInfo(nowPlayerName, "inventory");
        PlayerInventoryInfo inventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(playerInventory);
        // 将拥有的卡牌ID集合设置为该玩家拥有的卡牌ID集合
        ownedCardIds = new HashSet<int>(inventoryInfo.glory);
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
    private void PrepareCardDatas(PlayerInfo playerInfo, bool isSync = false)
    {
        cardDatas.Clear();
        foreach (int cardId in allGloryIds)
        {
            bool isOwned = ownedCardIds.Contains(cardId);
            if (cardId == playerInfo.now_emblem)
            {
                ABResMgr.Instance.LoadResAsync<Sprite>("emblem", $"icon_emblem_{cardId}", (res) =>
                {
                    currentGloryImage.sprite = res;
                }, isSync);
                NowDescribe.text = GloryDataMgr.Instance.GetGloryDescription(cardId)["description"];
            }
            Sprite cardSprite = null;
            ABResMgr.Instance.LoadResAsync<Sprite>("emblem", $"icon_emblem_{cardId}", (res) =>
            {
                cardSprite = res;
            }, isSync);
            cardDatas.Add(new GloryCardData(
                cardId,
                cardSprite,
                isOwned
            ));
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

    private void CreateCard(GloryCardData data, bool isSync = false)
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "MenuGloryCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            Transform cardBtn = cardObj.transform.Find("CardButton");
            MenuGloryCard card = cardObj.GetComponent<MenuGloryCard>();
            card.Initialize(data);
            card.OnCardClicked.AddListener(OnCardSelected);
        }, isSync);
    }
    private void OnCardSelected(GloryCardData data)
    {
        if (externalDisplayImage != null)
        {
            // 获取更改称号的描述和条件描述
            Dictionary<string, string> Description = new Dictionary<string, string>();
            Description = GloryDataMgr.Instance.GetGloryDescription(data.emblemId);
            // 更改图片
            if (data.isOwned)
            {
                externalDisplayImage.sprite = data.cardSprite;
                externalDisplayImageId = data.emblemId;
            }
            else
            {
                ABResMgr.Instance.LoadResAsync<Sprite>("emblem", "Locked", (res) =>
                {
                    externalDisplayImage.sprite = res;
                }, true);
            }
            // 更改描述
            ChangeDescribe.text = Description["description"];
            // 更改条件描述
            GetContion.text = Description["condition_desc"];
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
    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    // 重写ClickBtn方法
    protected override void ClickBtn(string btnName)
    {
        switch(btnName){
            case "BackToMenu":
                UIMgr.Instance.HidePanel<MenuGloryPanel>(true);
                UIMgr.Instance.ShowPanel<MenuPanel>();
                break;
            case "ChangeGloryBtn":
                if (externalDisplayImageId != -1 && externalDisplayImageId != playerInfo.now_emblem)
                {
                    // 修改当前的称号
                    ABResMgr.Instance.LoadResAsync<Sprite>("emblem", $"icon_emblem_{externalDisplayImageId}", (res) =>
                    {
                        currentGloryImage.sprite = res;
                    }, true);
                    NowDescribe.text = GloryDataMgr.Instance.GetGloryDescription(externalDisplayImageId)["description"];
                    // 修改玩家数据
                    playerInfo.now_emblem = externalDisplayImageId;
                    string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
                    File.WriteAllText(filePath, JsonMapper.ToJson(playerInfo));
                    GloryDataMgr.Instance.UpdateUserGlory(playerInfo.username, playerInfo.now_emblem);
                }
                break;
        }
    }

    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
