using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    // 记录现在的玩家
    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    // 更新玩家信息
    public override void UpdatePlayerInfo()
    {
        UpdateBeginPanelInfo();
    }

    public void UpdateBeginPanelInfo()
    {
        int level = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "level");
        int nowExp = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "current_exp");
        int totalExp = PlayerDataMgr.Instance.SearchUserLevelConfig(level, "require_exp");
        int nowAB = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "current_stamina");
        int totalAB = PlayerDataMgr.Instance.SearchUserLevelConfig(level, "stamina_cap");
        int manaCnt = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "mana_cnt");
        int diamondCnt = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "diamond_cnt");
        // 等级更新
        TMP_Text levelText = GetControl<TMP_Text>("Level");
        levelText.text = level.ToString();
        // 经验更新
        Exp.sizeDelta = new Vector2((float)nowExp / (float)totalExp * 250f, Exp.sizeDelta.y);
        // 体力更新
        int addAB = PlayerDataMgr.Instance.SearchUserLastStaminaUpdateInfo(nowPlayerName, "last_stamina_update") * 2;
        nowAB = nowAB + addAB > totalAB ? totalAB : nowAB + addAB;
        AB.sizeDelta = new Vector2((float)nowAB / (float)totalAB * 250f, AB.sizeDelta.y);
        TMP_Text ABText = GetControl<TMP_Text>("ABText");
        ABText.text = nowAB.ToString() + "/" + totalAB.ToString();
        // 玛娜更新
        TMP_Text manaText = GetControl<TMP_Text>("ManaText");
        manaText.text = manaCnt.ToString("N0");
        // 钻石更新
        TMP_Text diamondText = GetControl<TMP_Text>("DiamondText");
        diamondText.text = diamondCnt.ToString("N0");
        // 任务完成数更新
        int missionCnt = MissionDataMgr.Instance.GetMissionAchieveCount(nowPlayerName);
        TMP_Text missionText = GetControl<TMP_Text>("CntNum");
        missionText.text = missionCnt > 99 ? "99+" : missionCnt.ToString();
        #region 称号展示
        Transform Glory = this.transform.Find("PlayerData/Glory");
        // 路径
        string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
        PlayerInfo playInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath));
        ABResMgr.Instance.LoadResAsync<Sprite>("emblem", $"icon_emblem_{playInfo.now_emblem}", (res) =>
        {
            Glory.GetComponent<Image>().sprite = res;
        }, true);
        #endregion
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "MissonBtn":
                UIMgr.Instance.HidePanel<BeginPanel>();
                UIMgr.Instance.ShowPanel<BeginQuestPanel>(E_UILayer.Bottom, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                });
                break;
        }
    }

    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        if (MusicMgr.Instance.GetNowBKMusicName() != "NormalBG")
        {
            MusicMgr.Instance.PlayBKMusic("NormalBG");
        }
    }
}
