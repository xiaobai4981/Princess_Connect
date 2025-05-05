using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStarControl : BasePanel
{
    public GameObject[] starCount;
    public Sprite starSprite;

    public Image characterPiece;
    public TMP_Text characterPiecesDesc;
    public RectTransform piecesNeedFill;
    public TMP_Text piecesNeedText;

    public GameObject princessOrbNeed;
    public GameObject princessOrb;
    public TMP_Text princessOrbDesc;
    public RectTransform princessOrbNeedFill;
    public TMP_Text princessOrbNeedText;

    public TMP_Text manaNeedText;
    public Button starUpBtn;

    PlayerCharacterCollection playerCharacterCollection;
    PlayerCharacterData nowPlayerCharacterData;
    PlayerInventoryInfo playerInventoryInfo;
    PlayerInfo nowPlayerInfo;

    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(File.ReadAllText(filePath1));
        string filePath2 = Path.Combine(Application.persistentDataPath, "player_data.json");
        nowPlayerInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath2));
        UpdateBtns();
    }

    private void UpdateBtns()
    {
        int nowStarLevel = nowPlayerCharacterData.current_star;
        // 星星更新
        for (int i = 0; i < nowStarLevel; i++)
        {
            if (i < 5)
            {
                Image starImage = starCount[i].GetComponent<Image>();
                starImage.sprite = starSprite;
            }
            else
            {
                starCount[i].SetActive(true);
            }
        }
        #region 角色碎片更新
        // 获取角色碎片信息
        string piecesNum = nowCharacterId.ToString() + CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStarLevel].pieceMaterialNum.ToString();
        string piecesDesc = GloryDataMgr.Instance.GetItemDescription(int.Parse(piecesNum))["name"];
        int piecesNumNow = playerInventoryInfo.itemDic.ContainsKey(piecesNum) ? playerInventoryInfo.itemDic[piecesNum].num : 0;
        int piecesNumNeed = CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStarLevel].needPieceMaterial;
        // 更新
        ABResMgr.Instance.LoadResAsync<Sprite>("unit_material", "unit_material_no_frame_" + piecesNum, (res) => {
            characterPiece.sprite = res;
        }, true);
        characterPiecesDesc.text = piecesDesc;
        piecesNeedText.text = piecesNumNow + "/" + piecesNumNeed;
        piecesNeedFill.sizeDelta = piecesNumNow >= piecesNumNeed? new Vector2(220f, piecesNeedFill.sizeDelta.y) :
                                    new Vector2((float)piecesNumNow / (float)piecesNumNeed * 220f, piecesNeedFill.sizeDelta.y);
        #endregion
        // 公主宝珠更新
        if (CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStarLevel].isNeedOrd)
        {
            // 显示公主宝珠图片和进度条
            princessOrb.SetActive(true);
            princessOrbNeed.SetActive(true);
            // 获取公主宝珠信息
            string orbNum = "25001";
            string orbDesc = GloryDataMgr.Instance.GetItemDescription(int.Parse(orbNum))["name"];
            int orbNumNow = playerInventoryInfo.itemDic.ContainsKey(orbNum)? playerInventoryInfo.itemDic[orbNum].num : 0;
            int orbNumNeed = CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStarLevel].needOrdNum;
            // 更新
            princessOrbDesc.text = orbDesc;
            princessOrbNeedText.text = orbNumNow + "/" + orbNumNeed;
            princessOrbNeedFill.sizeDelta = orbNumNow >= orbNumNeed ? new Vector2(220f, piecesNeedFill.sizeDelta.y) :
                                        new Vector2((float)orbNumNow / (float)orbNumNeed * 220f, piecesNeedFill.sizeDelta.y);
        }
        // 所需Mana更新
        manaNeedText.text = CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStarLevel].needMana.ToString("N0");
    }

    private void Update()
    {
        if (princessOrbDesc.text == "公主宝珠")
        {
            if (piecesNeedFill.sizeDelta == new Vector2(220f, piecesNeedFill.sizeDelta.y) &&
                nowPlayerInfo.mana_cnt >= CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowPlayerCharacterData.current_star].needMana &&
                princessOrbNeedFill.sizeDelta == new Vector2(220f, princessOrbNeedFill.sizeDelta.y))
            {
                starUpBtn.interactable = true;
                starUpBtn.image.color = Color.white;
            }
            else
            {
                starUpBtn.interactable = false;
                starUpBtn.image.color = Color.gray;
            }
        }
        else
        {
            if (piecesNeedFill.sizeDelta == new Vector2(220f, piecesNeedFill.sizeDelta.y) &&
                nowPlayerInfo.mana_cnt >= CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowPlayerCharacterData.current_star].needMana)
            {
                starUpBtn.interactable = true;
                starUpBtn.image.color = Color.white;
            }
            else
            {
                starUpBtn.interactable = false;
                starUpBtn.image.color = Color.gray;
            }
        }
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "StarUpBtn":
                if (starUpBtn.interactable)
                {
                    StarUp();
                }
                break;
        }
    }

    private void StarUp()
    {
        int nowStar = nowPlayerCharacterData.current_star;
        string nowPieceNum = nowCharacterId.ToString() + CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStar].pieceMaterialNum.ToString();
        // 角色碎片消耗
        ItemInfo itemInfo = playerInventoryInfo.itemDic[nowPieceNum];
        itemInfo.num -= CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStar].needPieceMaterial;
        playerInventoryInfo.itemDic[nowPieceNum] = itemInfo;
        // 公主宝珠消耗
        if (CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStar].isNeedOrd)
        {
            ItemInfo ordItemInfo = playerInventoryInfo.itemDic["25001"];
            ordItemInfo.num -= CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStar].needPieceMaterial;
            playerInventoryInfo.itemDic["25001"] = ordItemInfo;
        }
        // 增加星星,并更新角色信息
        nowPlayerCharacterData.current_star++;
        int ce = 0;
        nowPlayerCharacterData.current_stats.hp += RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].hpGrowth;
        ce += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].hpGrowth * 0.1f);
        nowPlayerCharacterData.current_stats.atk += RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].atkGrowth;
        ce += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].atkGrowth);
        nowPlayerCharacterData.current_stats.physical_def += RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].defGrowth;
        nowPlayerCharacterData.current_stats.magical_def += RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].defGrowth;
        ce += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Star"][nowPlayerCharacterData.current_star].defGrowth * 9f);
        nowPlayerCharacterData.current_stats.ce += ce;
        // 更新玩家信息（玩家信息保存到本地和服务器）
        nowPlayerInfo.mana_cnt -= CharacterStarUpConfig.characterStarUpConfig.starUpConfigs[nowStar].needMana;
        string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
        File.WriteAllText(filePath, JsonMapper.ToJson(nowPlayerInfo));
        PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>() { {"mana_cnt", nowPlayerInfo.mana_cnt } }, false);
        // 文件更新和保存
        // player_character_data.json
        playerCharacterCollection.characters[nowCharacterId.ToString()] = nowPlayerCharacterData;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_character_data.json"), JsonMapper.ToJson(playerCharacterCollection));
        // player_inventory_data.json
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_inventory_data.json"), JsonMapper.ToJson(playerInventoryInfo));
        // 触发更新
        UpdateBtns();
        QuestListener.Instance.UpdateQuestProgress(nowPlayerName);
        EventCenter.Instance.EventTrigger<string>(E_EventType.E_Character_Develop_Update, "joy_long");
    }

    public override void UpdateCharacterId(int nowCharacterId)
    {
        this.nowCharacterId = nowCharacterId;
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
