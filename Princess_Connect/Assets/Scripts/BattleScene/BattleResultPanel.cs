using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPanel : BasePanel
{
    private int resultNum;
    public GameObject[] resultShow;
    public GameObject[] starShow;
    public Sprite star;
    private int aillyCount;
    private int enemyCount;
    private int stageNum;

    private void Start()
    {
        string fileName = Path.Combine(Application.persistentDataPath, "stage_config_data.json");
        StageDatas stageDatas = JsonMapper.ToObject<StageDatas>(File.ReadAllText(fileName));
        string fileName1 = Path.Combine(Application.persistentDataPath, "player_data.json");
        PlayerInfo playerInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(fileName1));

        // 显示结果图片
        for (int i = 0; i < resultShow.Length; i++)
        {
            if (i == resultNum)
            {
                resultShow[i].SetActive(true);
            }
            else
            {
                resultShow[i].SetActive(false);
            }
        }
        // 显示星星
        int starNum = 0;
        if (enemyCount <= 0)
        {
            if (aillyCount == GameBattleData.battleInitData.playerTeam.Count)
            {
                starNum = 3;
            }
            else if (aillyCount == GameBattleData.battleInitData.playerTeam.Count - 1)
            {
                starNum = 2;
            }
            else
            {
                starNum = 1;
            }
        }
        for (int i = 0; i < starNum; i++)
        {
            starShow[i].GetComponent<Image>().sprite = star;
        }
        // 修改本地和数据库文件
        if (stageNum == 2001)
        {
            int cost = stageDatas.stage_datas[stageNum.ToString()].config_data.stamina_cost;
            if (starNum != 0)
            {
                // 调查次数-1
                PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>()
                {
                    {"clear_stage_times", 1 },
                    {"serach_times", -1 }
                }, true);
            }
            else
            {
                PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>()
                {
                    {"current_stamina",  cost - 1},
                }, true);
            }
        }
        else if (stageNum != 0)
        {
            int cost = stageDatas.stage_datas[stageNum.ToString()].config_data.stamina_cost;
            if (starNum != 0)
            {
                PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>()
                {
                    {"clear_stage_times", 1 },
                }, true);
            }
            else
            {
                PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>()
                {
                    {"current_stamina",  cost - 1},
                }, true);
            }
        }
        int lastStarCnt = StageDataMgr.Instance.GetPlayerStageStar(nowPlayerName, stageNum);
        if (starNum > lastStarCnt && stageNum != 0)
        {
            StageDataMgr.Instance.UpdateStageStar(nowPlayerName, stageNum, starNum);
        }
        if (starNum != 0 && stageNum != 0)
        {
            // 获得玩家经验
            int exp = stageDatas.stage_datas[stageNum.ToString()].config_data.stamina_cost;
            int nowExp = playerInfo.current_exp;
            int level = playerInfo.level;
            int totalExp = PlayerDataMgr.Instance.SearchUserLevelConfig(level, "require_exp");
            
            nowExp += exp;
            
            while (nowExp >= totalExp && level != 20)
            {
                level++;
                nowExp -= totalExp;
                totalExp = PlayerDataMgr.Instance.SearchUserLevelConfig(level, "require_exp");
            }
            if (nowExp >= totalExp && level == 20)
            {
                nowExp = totalExp;
            }
            int stamina = PlayerDataMgr.Instance.SearchUserLevelConfig(level, "stamina_cap");
            // 保存在本地
            playerInfo.level = level;
            playerInfo.current_exp = nowExp;
            playerInfo.current_stamina += stamina;
            File.WriteAllText(fileName1, JsonMapper.ToJson(playerInfo));
            // 保存在数据库
            PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>() 
            {
                {"level", level },
                {"current_exp", nowExp },
                {"current_stamina", stamina }
            }, false);

        }
        QuestListener.Instance.UpdateQuestProgress(nowPlayerName);
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "BackToMainSceneBtn":
                BackToMainScene();
                break;
        }
    }
    private void BackToMainScene()
    {
        UIMgr.Instance.HidePanel<BattleMenuPanel>(true);
        UIMgr.Instance.HidePanel<BattleControlPanel>(true);
        UIMgr.Instance.HidePanel<BattleResultPanel>(true);
        SceneMgr.Instance.LoadSceneAsyn("MainScene");
        UIMgr.Instance.ShowPanel<ADVPanel>(E_UILayer.Middle, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            panel.UpdatePlayerInfo();
        });
        UIMgr.Instance.ShowPanel<BottomBtnPanel>();
    }

    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    public void InputResult(int result, int aillyCount, int enemyCount)
    {
        this.resultNum = result;
        this.aillyCount = aillyCount;
        this.enemyCount = enemyCount;
    }
    public void UpdateStageNum(int stageNum)
    {
        this.stageNum = stageNum;
    }

    public override void HideMe()
    {

    }

    public override void ShowMe()
    {

    }
}
