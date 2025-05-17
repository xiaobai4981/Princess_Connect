using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ArenaCardData
{
    public string playerName;
    public int playerLevel;
    public List<CharacterFactoryTeamCardData> teamCardDatas;
}

public class ADVArenaPanel : BasePanel
{
    private Dictionary<string, List<int>> otherPlayerArenaConfig;
    private void Start()
    {
        otherPlayerArenaConfig = PlayerDataMgr.Instance.SearchOtherArenaDeployInfo(nowPlayerName);
        Refresh();
    }
    // Content
    public Transform contentParent;
    // 暂存按钮数据
    private List<ArenaCardData> cardDatas = new List<ArenaCardData>();
    // 类型不同的道具仓库卡牌初始化
    private void PrepareCardDatas(bool isSync = true)
    {
        cardDatas.Clear();
        foreach (string userName in otherPlayerArenaConfig.Keys)
        {
            int level = PlayerDataMgr.Instance.SearchUserIntInfo(userName, "level");
            List<CharacterFactoryTeamCardData> arenaCardDatas = new List<CharacterFactoryTeamCardData>();
            for (int i = 0; i < otherPlayerArenaConfig[userName].Count; i++)
            {
                PlayerCharacterData data = CharacterDataMgr.Instance.GetUserCharacterData(userName, otherPlayerArenaConfig[userName][i]);
                Sprite characterSprite = null;
                Sprite rankSprite = null;
                string rankNum;
                if (data.current_star <= 3)
                {
                    rankNum = "11";
                }
                else if (data.current_star <= 5)
                {
                    rankNum = "31";
                }
                else
                {
                    rankNum = "61";
                }
                string characterSpriteFileName = $"icon_unit_{data.character_id}" + rankNum;
                // 加载角色卡片图片
                ABResMgr.Instance.LoadResAsync<Sprite>("character_icon", characterSpriteFileName, (res) =>
                {
                    characterSprite = res;
                }, isSync);
                // 加载边框
                ABResMgr.Instance.LoadResAsync<Sprite>("rank_frame", $"character_rank_{data.current_rank}", (res) =>
                {
                    rankSprite = res;
                }, isSync);
                arenaCardDatas.Add(new CharacterFactoryTeamCardData(
                    1,
                    data.character_id,
                    data,
                    characterSprite,
                    rankSprite,
                    data.current_rank,
                    false
                ));
            }
            cardDatas.Add(new ArenaCardData()
            {
                playerName = userName,
                playerLevel = level,
                teamCardDatas = arenaCardDatas
            });
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

    private void CreateCard(ArenaCardData data, bool isSync = false)
    {
        ABResMgr.Instance.LoadResAsync<GameObject>("ui", "ADVArenaEnemyCard", (res) =>
        {
            GameObject cardObj = GameObject.Instantiate(res, contentParent, false);
            // 按钮绑定事件
            ADVArenaEnemyCard card = cardObj.GetComponent<ADVArenaEnemyCard>();
            card.Initialize(data.playerName, data.playerLevel, data.teamCardDatas);
            card.OnCharcterClicked.AddListener(OnCardSelected);
        }, isSync);
    }
    private void OnCardSelected(List<CharacterFactoryTeamCardData> data)
    {
        UIMgr.Instance.ShowPanel<ADVTeamSelectPanel>(E_UILayer.System, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            panel.UpdateArenaInfo(data);
        });
    }
    private void ClearAllCards()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
    }
    private void Refresh()
    {
        PrepareCardDatas();
        GenerateAllCards();
    }
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "TeamSettingBtn":
                UIMgr.Instance.ShowPanel<ADVArenaTeamSelectPanel>(E_UILayer.System, (panel) => {
                    panel.UpdatePlayerName(nowPlayerName);
                });
                break;
            case "BackToADV":
                UIMgr.Instance.HidePanel<ADVArenaPanel>(true);
                UIMgr.Instance.ShowPanel<ADVPanel>(E_UILayer.Bottom, (panel) =>
                {
                    panel.UpdatePlayerInfo();
                });
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
