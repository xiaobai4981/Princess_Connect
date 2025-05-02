using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct EquipBtnInfo
{
    public bool isEquip;
    public Sprite validIcon;
    public Sprite invalidIcon;
    public int equipId;
    public int equipNum;
}
public class CharacterEquipControl : BasePanel
{
    public Button[] equipBtns;
    public Button equipArmBtn;
    public Button equipRankUp;
    private List<EquipBtnInfo> equipBtnInfoList;
    public Image nowEquipIcon;
    private int nowEquipNumber;
    public TMP_Text nowEquipName;
    public TMP_Text nowEquipNum;
    public TMP_Text nowEquipBuff;
    PlayerCharacterCollection playerCharacterCollection;
    PlayerCharacterData nowPlayerCharacterData;
    PlayerInventoryInfo playerInventoryInfo;
    #region 初始化页面
    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(File.ReadAllText(filePath1));
        equipBtnInfoList = new List<EquipBtnInfo>();
        nowEquipNumber = 0;
        UpdatequipBtns();
        UpdateNowEquip(nowEquipNumber);
    }
    private void UpdatequipBtns()
    {
        equipBtnInfoList.Clear();
        // 角色当前品级
        int current_rank = nowPlayerCharacterData.current_rank;
        // 角色当前装备
        List<int> equip_ids = new List<int>(nowPlayerCharacterData.rank_requirements.rank_requirements[current_rank - 1]);
        // 角色当前装备是否装备 0 未装备 1 装备
        List<int> equip_is_equips = new List<int>(nowPlayerCharacterData.equipment_slots.equipment_slot);
        // 角色当前装备数量
        List<int> equip_nums = new List<int>();
        foreach (int equip_id in equip_ids)
        {
            if (playerInventoryInfo.itemDic.ContainsKey(equip_id.ToString()))
            {
                equip_nums.Add(playerInventoryInfo.itemDic[equip_id.ToString()].num);
            }
            equip_nums.Add(0);
        }
        // 初始化按钮
        for (int i = 0; i < equipBtns.Length; i++)
        {
            EquipBtnInfo equipBtnInfo = new EquipBtnInfo();
            equipBtnInfo.isEquip = (equip_is_equips[i] == 1);
            equipBtnInfo.equipId = equip_ids[i];
            equipBtnInfo.equipNum = equip_nums[i];
            // 加载已装备按钮图片
            ABResMgr.Instance.LoadResAsync<Sprite>("equipment", $"icon_equipment_{equip_ids[i]}", (obj) =>
            {
                equipBtnInfo.validIcon = obj;
            }, true);
            // 加载未装备按钮图片
            ABResMgr.Instance.LoadResAsync<Sprite>("equipment", $"icon_equipment_invalid_{equip_ids[i]}", (obj) =>
            {
                equipBtnInfo.invalidIcon = obj;
            }, true);
            equipBtns[i].image.sprite = (equipBtnInfo.isEquip ? equipBtnInfo.validIcon : equipBtnInfo.invalidIcon);
            equipBtnInfoList.Add(equipBtnInfo);
        }
    }
    private void UpdateNowEquip(int equipNumber)
    {
        // 显示当前装备
        Dictionary<string, string> desc = GloryDataMgr.Instance.GetItemDescription(equipBtnInfoList[equipNumber].equipId);
        nowEquipName.text = desc["name"];
        nowEquipNum.text = "x" + equipBtnInfoList[equipNumber].equipNum.ToString();
        nowEquipIcon.sprite = equipBtnInfoList[equipNumber].validIcon;
        // 显示当前装备buff
        string buff_desc = GloryDataMgr.Instance.GetItemBuff(equipBtnInfoList[equipNumber].equipId);
        JsonData jsonData = JsonMapper.ToObject(buff_desc);
        JsonData buffData = jsonData["buff"];
        string buff_text = "";
        string descType = nowPlayerCharacterData.current_stats.attack_type == "physical" ? "物理" : "魔法";
        foreach (string propertyName in buffData.Keys)
        {
            string typeDesc = propertyName == "atk" || propertyName == "crit" ? descType : "";
            string temp = typeDesc + KeyWordMapConfig.characterKeyWordDicConfig.KeyWordDic[propertyName] + "  +" + buffData[propertyName].ToString() + "\n";
            buff_text += temp;
        }
        nowEquipBuff.text = buff_text;
    }
    #endregion
    private void Update()
    {
        equipArmBtn.interactable = int.Parse(nowEquipNum.text.Substring(1)) >= 1? true : false;
        equipArmBtn.image.color = int.Parse(nowEquipNum.text.Substring(1)) >= 1 ? Color.white : Color.gray;
        int cnt = 0;
        foreach (var info in equipBtnInfoList)
        {
            cnt += info.isEquip ? 1 : 0;
        }
        equipRankUp.interactable = cnt == 6 ? true : false;
        equipRankUp.image.color = cnt == 6 ? Color.white : Color.gray;
    }
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "Equipment1":
                nowEquipNumber = 0;
                UpdateNowEquip(nowEquipNumber);
                break;
            case "Equipment2":
                nowEquipNumber = 1;
                UpdateNowEquip(nowEquipNumber);
                break;
            case "Equipment3":
                nowEquipNumber = 2;
                UpdateNowEquip(nowEquipNumber);
                break;
            case "Equipment4":
                nowEquipNumber = 3;
                UpdateNowEquip(nowEquipNumber);
                break;
            case "Equipment5":
                nowEquipNumber = 4;
                UpdateNowEquip(nowEquipNumber);
                break;
            case "Equipment6":
                nowEquipNumber = 5;
                UpdateNowEquip(nowEquipNumber);
                break;
            case "EquipArmBtn":
                if (equipArmBtn.interactable && equipBtnInfoList[nowEquipNumber].isEquip == false)
                {
                    ArmEquip();
                }
                break;
            case "EquipRankUp":
                if (equipRankUp.interactable)
                {
                    RankUp();
                }
                break;
        }
    }
    
    // 装备装备
    private void ArmEquip()
    {
        // 改装备数量（仓库）
        ItemInfo itemInfo = playerInventoryInfo.itemDic[equipBtnInfoList[nowEquipNumber].equipId.ToString()];
        itemInfo.num -= 1;
        nowEquipNum.text = "x" + itemInfo.num.ToString();
        playerInventoryInfo.itemDic[equipBtnInfoList[nowEquipNumber].equipId.ToString()] = itemInfo;
        nowPlayerCharacterData.equipment_slots.equipment_slot[nowEquipNumber] = 1;
        // 改人物属性
        string buff_desc = GloryDataMgr.Instance.GetItemBuff(equipBtnInfoList[nowEquipNumber].equipId);
        JsonData jsonData = JsonMapper.ToObject(buff_desc);
        JsonData buffData = jsonData["buff"];
        int ceAdd = 0;
        foreach (string propertyName in buffData.Keys)
        {
            switch (propertyName)
            {
                case "atk":
                    nowPlayerCharacterData.current_stats.atk += int.Parse(buffData[propertyName].ToString());
                    ceAdd += int.Parse(buffData[propertyName].ToString());
                    break;
                case "physical_def":
                    nowPlayerCharacterData.current_stats.physical_def += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 4.5);
                    break;
                case "magical_def":
                    nowPlayerCharacterData.current_stats.magical_def += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 4.5);
                    break;
                case "hp":
                    nowPlayerCharacterData.current_stats.hp += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 0.1);
                    break;
                case "crit":
                    nowPlayerCharacterData.current_stats.crit += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 0.5);
                    break;
                case "dod":
                    nowPlayerCharacterData.current_stats.dod += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 6);
                    break;
                case "hp_recover_per_wave":
                    nowPlayerCharacterData.current_stats.hp_recover_per_wave += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 0.1);
                    break;
                case "tp_recover_per_wave":
                    nowPlayerCharacterData.current_stats.tp_recover_per_wave += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 0.3);
                    break;
                case "tp_get":
                    nowPlayerCharacterData.current_stats.tp_get += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 1.5);
                    break;
                case "hit_rate":
                    nowPlayerCharacterData.current_stats.hit_rate += int.Parse(buffData[propertyName].ToString());
                    ceAdd += (int)(int.Parse(buffData[propertyName].ToString()) * 2);
                    break;
            }
        }
        nowPlayerCharacterData.current_stats.ce += ceAdd;
        // 改本地文件 三个
        // player_character_data.json
        playerCharacterCollection.characters[nowCharacterId.ToString()] = nowPlayerCharacterData;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_character_data.json"), JsonMapper.ToJson(playerCharacterCollection));
        // player_inventory_data.json
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_inventory_data.json"), JsonMapper.ToJson(playerInventoryInfo));
        // 改当前页面
        EquipBtnInfo equipBtnInfo = equipBtnInfoList[nowEquipNumber];
        equipBtnInfo.isEquip = true;
        equipBtnInfo.equipNum -= 1;
        equipBtnInfoList[nowEquipNumber] = equipBtnInfo;
        equipBtns[nowEquipNumber].image.sprite = equipBtnInfoList[nowEquipNumber].validIcon;
        // 触发上传
        UpdatequipBtns();
        EventCenter.Instance.EventTrigger<string>(E_EventType.E_Character_Develop_Update, "joy_short");
    }

    // 升阶
    private void RankUp()
    {
        #region 本地数据更新
        // 角色部分
        nowPlayerCharacterData.current_rank += 1;
        int ceAdd = 0;
        nowPlayerCharacterData.current_stats.hp += RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].hpGrowth;
        ceAdd += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].hpGrowth * 0.1);
        nowPlayerCharacterData.current_stats.atk += RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].atkGrowth;
        ceAdd += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].atkGrowth);
        nowPlayerCharacterData.current_stats.physical_def += RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].defGrowth;
        ceAdd += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].defGrowth * 4.5);
        nowPlayerCharacterData.current_stats.magical_def += RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].defGrowth;
        ceAdd += (int)(RewardConfig.characterDevelopConfig.otherUpRewards["Rank"][nowPlayerCharacterData.current_rank].defGrowth * 4.5);
        nowPlayerCharacterData.current_stats.ce += ceAdd;
        for (int i = 0; i < nowPlayerCharacterData.equipment_slots.equipment_slot.Count; i++)
        {
            nowPlayerCharacterData.equipment_slots.equipment_slot[i] = 0;  // 直接修改每个元素
        }
        // 页面更新部分
        UpdatequipBtns();
        nowEquipNumber = 0;
        UpdateNowEquip(nowEquipNumber);
        // 文件更新部分
        playerCharacterCollection.characters[nowCharacterId.ToString()] = nowPlayerCharacterData;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_character_data.json"), JsonMapper.ToJson(playerCharacterCollection));
        #endregion
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
