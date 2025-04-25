using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;


public class MenuGloryPanel : BasePanel
{
    #region 处理卡面初始化
    // Content
    public Transform contentParent;
    // 玩家拥有的卡牌ID集合
    private HashSet<int> ownedCardIds = new HashSet<int>();
    // 数据库中所有称号的id集合
    private List<int> allGloryIds = new List<int>();
    // 生成卡面部分
    void Start()
    {
        // 模拟获取玩家拥有的卡牌ID（实际应从服务器获取）
        FetchPlayerOwnedCards(() => {
            GenerateAllCards();
        });
        allGloryIds = GloryDataMgr.Instance.GetAllGloryIds();
    }
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
    private void GenerateAllCards()
    {
        // 清除现有卡牌（如果有）
        ClearAllCards();

        // 生成所有卡牌
        for (int cardId = 0; cardId < allGloryIds.Count; cardId++)
        {
            CreateCard(allGloryIds[cardId]);
        }
    }

    private void CreateCard(int cardId, bool isSync = false)
    {
        bool isOwned = ownedCardIds.Contains(cardId);
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "MenuGloryCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            Transform cardBtn = cardObj.transform.Find("CardButton");
            if (isOwned)
            {
                cardBtn.GetComponent<Button>().image.sprite = Resources.Load<Sprite>("Textures/Emblem/" + "icon_emblem_" + cardId.ToString());
            }
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
    #region 按钮点击事件

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
