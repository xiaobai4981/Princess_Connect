using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ADVMainPanel : BasePanel
{
    public GameObject[] stageOneStarBtn1;
    public GameObject[] stageOneStarBtn2;
    public GameObject[] stageOneStarBtn3;
    public Sprite star;
    void Start()
    {
        int starCountNum = StageDataMgr.Instance.GetPlayerStageStar(nowPlayerName, 1001);
        for (int i = 0; i < starCountNum; i++)
        {
            stageOneStarBtn1[i].GetComponent<Image>().sprite = star;
        }
        starCountNum = StageDataMgr.Instance.GetPlayerStageStar(nowPlayerName, 1002);
        for (int i = 0; i < starCountNum; i++)
        {
            stageOneStarBtn2[i].GetComponent<Image>().sprite = star;
        }
        starCountNum = StageDataMgr.Instance.GetPlayerStageStar(nowPlayerName, 1003);
        for (int i = 0; i < starCountNum; i++)
        {
            stageOneStarBtn3[i].GetComponent<Image>().sprite = star;
        }
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName) 
        {
            case "StageOne":
                UIMgr.Instance.ShowPanel<ADVStageDetailsPanel>(E_UILayer.System, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                    panel.UpdateNowStageIndex(1001);
                });
                break;
            case "StageTwo":
                UIMgr.Instance.ShowPanel<ADVStageDetailsPanel>(E_UILayer.System, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                    panel.UpdateNowStageIndex(1002);
                });
                break;
            case "StageThree":
                UIMgr.Instance.ShowPanel<ADVStageDetailsPanel>(E_UILayer.System, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                    panel.UpdateNowStageIndex(1003);
                });
                break;
            case "BackToADV":
                UIMgr.Instance.HidePanel<ADVMainPanel>(true);
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
