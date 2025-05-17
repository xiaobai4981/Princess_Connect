using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class LotteryResultPanel : BasePanel
{
    public GameObject[] lotteryResultItems;
    public GameObject timeLineDirector;
    private List<int> resultList;
    private bool lotteryMode;

    PlayerInventoryInfo playerInventoryInfo;

    private void Start()
    {
        if (resultList.Count == 1)
        {
            timeLineDirector.SetActive(false);
            for (int i = 1; i < lotteryResultItems.Length; i++)
            {
                lotteryResultItems[i].SetActive(false);
            }
        }
        // 普通抽奖
        if (lotteryMode == false)
        {
            for (int i = 0; i < resultList.Count; i++)
            {
                if (resultList[i] < 100000)
                {
                    ABResMgr.Instance.LoadResAsync<Sprite>("item", $"icon_item_{resultList[i]}", (res) =>
                    {
                        lotteryResultItems[i].GetComponent<Image>().sprite = res;
                    });
                }
                else
                {
                    ABResMgr.Instance.LoadResAsync<Sprite>("equipment", $"icon_equipment_{resultList[i]}", (res) =>
                    {
                        lotteryResultItems[i].GetComponent<Image>().sprite = res;
                    });
                }
            }
            SaveItemData();
        }
        else
        {
            string defaultCharacterEquipment = File.ReadAllText("Assets/Resources/Configs/NewPlayerCharacterEquipment.json");
            string defaultCharacterSkillLevel = File.ReadAllText("Assets/Resources/Configs/NewPlayerCharacterSkillLevel.json");
            for (int i = 0; i < resultList.Count; i++)
            {
                if (CharacterDataMgr.Instance.SearchCharacterInPlayer(nowPlayerName, resultList[i]))
                {
                    ABResMgr.Instance.LoadResAsync<Sprite>("unit_material", $"unit_material_no_frame_{resultList[i]}11", (res) =>
                    {
                        lotteryResultItems[i].GetComponent<Image>().sprite = res;
                    });
                }
                else
                {
                    // 未获得的角色
                    CharacterDataMgr.Instance.AddUserPlayerInfo(nowPlayerName, resultList[i]);
                    CharacterDataMgr.Instance.InitPlayerCharacter(nowPlayerName, resultList[i], defaultCharacterEquipment, defaultCharacterSkillLevel);
                    CharacterDataMgr.Instance.InitUserCharacterData(nowPlayerName);

                    ABResMgr.Instance.LoadResAsync<Sprite>("character_icon", $"icon_unit_{resultList[i]}11", (res) =>
                    {
                        lotteryResultItems[i].GetComponent<Image>().sprite = res;
                    });
                }
            }
            SaveCharacterData();
        }
    }

    private void SaveItemData()
    {
        foreach (int item in resultList)
        {
            if (playerInventoryInfo.itemDic.ContainsKey(item.ToString()))
            {
                ItemInfo itemInfo = playerInventoryInfo.itemDic[item.ToString()];
                itemInfo.num += 1;
                playerInventoryInfo.itemDic[item.ToString()] = itemInfo;
            }
            else
            {
                ItemInfo itemInfo = new ItemInfo();
                itemInfo.num = 1;
                itemInfo.type = item < 100000? "item": "equipment";
                playerInventoryInfo.itemDic.Add(item.ToString(), itemInfo);
                
            }
        }
        string filePath = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        File.WriteAllText(filePath, JsonMapper.ToJson(playerInventoryInfo));
        string inventoryData = File.ReadAllText(filePath);
        GloryDataMgr.Instance.UpdateUserPlayerFactoryInfo(nowPlayerName, inventoryData);
    }

    private void SaveCharacterData()
    {
        string defaultCharacterEquipment = File.ReadAllText("Assets/Resources/Configs/NewPlayerCharacterEquipment.json");
        string defaultCharacterSkillLevel = File.ReadAllText("Assets/Resources/Configs/NewPlayerCharacterSkillLevel.json");

        foreach (int characterItem in resultList)
        {
            if (CharacterDataMgr.Instance.SearchCharacterInPlayer(nowPlayerName, characterItem))
            {
                DateTime now = DateTime.Now;
                int totalSeconds = now.Second;
                string pieces = totalSeconds % 2 == 0? characterItem + "11" : characterItem + "61";
                if (playerInventoryInfo.itemDic.ContainsKey(pieces))
                {
                    ItemInfo itemInfo = playerInventoryInfo.itemDic[pieces];
                    itemInfo.num += 1;
                    playerInventoryInfo.itemDic[pieces] = itemInfo;
                }
                else
                {
                    ItemInfo itemInfo = new ItemInfo();
                    itemInfo.num = 1;
                    itemInfo.type = "unit_material";
                    playerInventoryInfo.itemDic.Add(pieces, itemInfo);
                }
            }
        }
        string filePath = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        File.WriteAllText(filePath, JsonMapper.ToJson(playerInventoryInfo));
        string inventoryData = File.ReadAllText(filePath);
        GloryDataMgr.Instance.UpdateUserPlayerFactoryInfo(nowPlayerName, inventoryData);
        QuestListener.Instance.UpdateQuestProgress(nowPlayerName);
    }


    protected override void ClickBtn(string buttonName)
    {
        switch (buttonName)
        {
            case "ConfirmBtn":
                UIMgr.Instance.HidePanel<LotteryResultPanel>(true);
                UIMgr.Instance.ShowPanel<LotteryPanel>();
                break;
        }
    }
    public void UpdateLotteryResult(List<int> result, bool lotteryMode)
    {
        resultList = result;
        this.lotteryMode = lotteryMode;
    }

    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
        string filePath = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(File.ReadAllText(filePath));
    }
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
