using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMaterialControl : BasePanel
{
    public TMP_Text characterLevel;
    public RectTransform ExpFill;
    public TMP_Text EXPNum;
    public TMP_Text LikbNum;
    public TMP_Text NextLevel;
    public Button expArmBtn;
    public Button likbArmBtn;
    PlayerCharacterCollection playerCharacterCollection;
    PlayerCharacterData nowPlayerCharacterData;
    PlayerInventoryInfo playerInventoryInfo;
    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(File.ReadAllText(filePath1));
        UpdatequipBtns();
    }
    private void UpdatequipBtns()
    {
        characterLevel.text = nowPlayerCharacterData.level.ToString();
        EXPNum.text = "x" + (playerInventoryInfo.itemDic.ContainsKey("20001")? playerInventoryInfo.itemDic["20001"].num.ToString(): "0");
        LikbNum.text = "x" + (playerInventoryInfo.itemDic.ContainsKey("50003") ? playerInventoryInfo.itemDic["50003"].num.ToString() : "0");
        int totalExp = CharacterDataMgr.Instance.SearchCharacterLevelConfig(nowPlayerCharacterData.level);
        ExpFill.sizeDelta = new Vector2((float)nowPlayerCharacterData.current_exp / (float)totalExp * 400f, ExpFill.sizeDelta.y);
        NextLevel.text = (totalExp - nowPlayerCharacterData.current_exp).ToString();
    }
    private void Update()
    {
        int totalExp = CharacterDataMgr.Instance.SearchCharacterLevelConfig(nowPlayerCharacterData.level);
        if (playerInventoryInfo.itemDic.ContainsKey("20001") && playerInventoryInfo.itemDic["20001"].num > 0)
        {
            if (nowPlayerCharacterData.level == 20 &&  nowPlayerCharacterData.current_exp == totalExp)
            {
                expArmBtn.interactable = false;
                expArmBtn.image.color = Color.gray;
            }
            else
            {
                expArmBtn.interactable = true;
                expArmBtn.image.color = Color.white;
            }
        }
        else
        {
            expArmBtn.interactable = false;
            expArmBtn.image.color = Color.gray;
        }

        if (playerInventoryInfo.itemDic.ContainsKey("50003") && playerInventoryInfo.itemDic["50003"].num > 0)
        {
            if (nowPlayerCharacterData.current_likb == 10)
            {
                likbArmBtn.interactable = false;
                likbArmBtn.image.color = Color.gray;
            }
            else
            {
                likbArmBtn.interactable = true;
                likbArmBtn.image.color = Color.white;
            }
        }
        else
        {
            likbArmBtn.interactable = false;
            likbArmBtn.image.color = Color.gray;
        }

    }
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "EXPArmBtn":
                if (expArmBtn.interactable)
                {
                    ArmExp();
                }
                break;
            case "LikbArmBtn":
                if (likbArmBtn.interactable)
                {
                    ArmLikb();    
                }
                break;
        }
    }

    private void ArmExp()
    {
        int level = nowPlayerCharacterData.level;
        int totalExp = CharacterDataMgr.Instance.SearchCharacterLevelConfig(level);
        int currentExp = nowPlayerCharacterData.current_exp;
        int ce = 0;
        currentExp += 30;
        // 角色部分
        while (currentExp >= totalExp && level != 20)
        {
            ce += 125;
            nowPlayerCharacterData.current_stats.hp += RewardConfig.characterDevelopConfig.hpLevelGrowth;
            nowPlayerCharacterData.current_stats.atk += RewardConfig.characterDevelopConfig.atkLevelGrowth;
            nowPlayerCharacterData.current_stats.physical_def += RewardConfig.characterDevelopConfig.defLevelGrowth;
            nowPlayerCharacterData.current_stats.magical_def += RewardConfig.characterDevelopConfig.defLevelGrowth;
            level++;
            currentExp -= totalExp;
            totalExp = CharacterDataMgr.Instance.SearchCharacterLevelConfig(level);
        }
        if (currentExp >= totalExp && level == 20)
        {
            currentExp = totalExp;
        }
        nowPlayerCharacterData.current_stats.ce += ce;
        nowPlayerCharacterData.level = level;
        nowPlayerCharacterData.current_exp = currentExp;
        // 页面更新部分
        ItemInfo itemInfo = playerInventoryInfo.itemDic["20001"];
        itemInfo.num -= 1;
        playerInventoryInfo.itemDic["20001"] = itemInfo;
        UpdatequipBtns();
        // 文件更新部分
        // player_character_data.json
        playerCharacterCollection.characters[nowCharacterId.ToString()] = nowPlayerCharacterData;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_character_data.json"), JsonMapper.ToJson(playerCharacterCollection));
        // player_inventory_data.json
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_inventory_data.json"), JsonMapper.ToJson(playerInventoryInfo));
        // 触发上传
        if (ce != 0)
            EventCenter.Instance.EventTrigger<string>(E_EventType.E_Character_Develop_Update, "joy_short");
    }

    private void ArmLikb()
    {
        // 角色部分
        if (nowPlayerCharacterData.current_likb == 10)
        {
            return;
        }
        nowPlayerCharacterData.current_likb += 1;
        nowPlayerCharacterData.current_stats.hp += RewardConfig.characterDevelopConfig.otherUpRewards["Likb"][nowPlayerCharacterData.current_likb].hpGrowth;
        nowPlayerCharacterData.current_stats.ce += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Likb"][nowPlayerCharacterData.current_likb].hpGrowth * 0.1f);
        nowPlayerCharacterData.current_stats.atk += RewardConfig.characterDevelopConfig.otherUpRewards["Likb"][nowPlayerCharacterData.current_likb].atkGrowth;
        nowPlayerCharacterData.current_stats.ce += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Likb"][nowPlayerCharacterData.current_likb].atkGrowth);
        // 页面更新部分
        ItemInfo itemInfo = playerInventoryInfo.itemDic["50003"];
        itemInfo.num -= 1;
        playerInventoryInfo.itemDic["50003"] = itemInfo;
        UpdatequipBtns();
        // 文件更新部分
        // player_character_data.json
        playerCharacterCollection.characters[nowCharacterId.ToString()] = nowPlayerCharacterData;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_character_data.json"), JsonMapper.ToJson(playerCharacterCollection));
        // player_inventory_data.json
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_inventory_data.json"), JsonMapper.ToJson(playerInventoryInfo));
        // 触发上传
        EventCenter.Instance.EventTrigger<string>(E_EventType.E_Character_Develop_Update, "joy_long");
    }

    // 更新当前的人物id
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
