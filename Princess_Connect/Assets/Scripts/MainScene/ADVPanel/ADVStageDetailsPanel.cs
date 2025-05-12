using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ADVStageDetailsPanel : BasePanel
{
    private int nowStageIndex;
    private PlayerInfo nowPlayerInfo;
    private StageDatas stageDatas;

    private Dictionary<int, string> stageName = new Dictionary<int, string>()
    {
        {1001, "主线关卡 1 - 1"},
        {1002, "主线关卡 1 - 2"},
        {1003, "主线关卡 1 - 3"},
        {2001, "调查关卡"}
    };
    // 需要更新的ui
    public TMP_Text stageNameText;
    public TMP_Text nowStamina;
    public TMP_Text costStamina;

    public GameObject[] starCount;
    public Sprite star;
    public GameObject[] monsterBlock;
    private List<int> monsterList = new List<int>();
    public GameObject[] itemBlock;
    public GameObject[] fullRFewardBlock;


    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
        string jsonStr = File.ReadAllText(filePath);
        nowPlayerInfo = JsonMapper.ToObject<PlayerInfo>(jsonStr);

        string filePath1 = Path.Combine(Application.persistentDataPath, "stage_config_data.json");
        string jsonStr1 = File.ReadAllText(filePath1);
        stageDatas = JsonMapper.ToObject<StageDatas>(jsonStr1);

        UpdateUI();
    }

    private void UpdateUI()
    {
        StageData stageData = stageDatas.stage_datas[nowStageIndex.ToString()];
        // 文字更新
        stageNameText.text = stageName[nowStageIndex];
        costStamina.text = stageData.config_data.stamina_cost.ToString();
        int totalAB = PlayerDataMgr.Instance.SearchUserLevelConfig(nowPlayerInfo.level, "stamina_cap");
        nowStamina.text = nowPlayerInfo.current_stamina.ToString() + "/" + totalAB.ToString();
        // 星星更新
        int starCountNum = StageDataMgr.Instance.GetPlayerStageStar(nowPlayerName, nowStageIndex);
        for (int i = 0; i < starCountNum; i++)
        {
            starCount[i].GetComponent<Image>().sprite = star;
        }
        // 怪物信息更新
        for (int i = 0; i < stageData.config_data.monsters.Count; i++)
        {
            int rankNum = stageData.config_data.monsters[i].rank;
            int monsterNum = stageData.config_data.monsters[i].monster_id;
            monsterList.Add(monsterNum);
            ABResMgr.Instance.LoadResAsync<Sprite>("monster", $"icon_unit_{monsterNum}", (res)=>{
                monsterBlock[i].GetComponent<Image>().sprite = res;
            } , true);
            ABResMgr.Instance.LoadResAsync<Sprite>("rank_frame", $"character_rank_{rankNum}", (res) => {
                monsterBlock[i].transform.Find("Rarity").GetComponent<Image>().sprite = res;
            }, true);
        }
        // 掉落道具信息更新
        // 普通
        if (stageData.config_data.reward.normal_drop.item != null)
        {
            for (int i = 0; i < stageData.config_data.reward.normal_drop.item.Count; i++)
            {
                ABResMgr.Instance.LoadResAsync<Sprite>("item", $"icon_item_{stageData.config_data.reward.normal_drop.item[i]}", (res) => {
                    itemBlock[i].GetComponent<Image>().sprite = res;
                }, true);
            }
        }
        else
        {
            for (int i = 0; i < stageData.config_data.reward.normal_drop.equipment.Count; i++)
            {
                ABResMgr.Instance.LoadResAsync<Sprite>("equipment", $"icon_equipment_{stageData.config_data.reward.normal_drop.equipment[i]}", (res) => {
                    itemBlock[i].GetComponent<Image>().sprite = res;
                }, true);
            }
        }
        // 第一次
        if (stageData.config_data.reward.first_perfect_clear.item != null)
        {
            for (int i = 0; i < stageData.config_data.reward.first_perfect_clear.item.Count; i++)
            {
                ABResMgr.Instance.LoadResAsync<Sprite>("item", $"icon_item_{stageData.config_data.reward.first_perfect_clear.item[i]}", (res) => {
                    fullRFewardBlock[i].GetComponent<Image>().sprite = res;
                }, true);
            }
        }
        else
        {
            for (int i = 0; i < stageData.config_data.reward.first_perfect_clear.equipment.Count; i++)
            {
                ABResMgr.Instance.LoadResAsync<Sprite>("equipment", $"icon_equipment_{stageData.config_data.reward.first_perfect_clear.equipment[i]}", (res) => {
                    fullRFewardBlock[i].GetComponent<Image>().sprite = res;
                }, true);
            }
        }
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "BeginBtn":
                UIMgr.Instance.ShowPanel<ADVTeamSelectPanel>(E_UILayer.System, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                    panel.UpdateInfo(monsterList, stageDatas.stage_datas[nowStageIndex.ToString()]);
                    panel.UpdateNowStageIndex(nowStageIndex);
                });
                break;
            case "CancelBtn":
                UIMgr.Instance.HidePanel<ADVStageDetailsPanel>(true);
                break;
        }

    }

    public void UpdateNowStageIndex(int nowStageIndex)
    {
        this.nowStageIndex = nowStageIndex;
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
