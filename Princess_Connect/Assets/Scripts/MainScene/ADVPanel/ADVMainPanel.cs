using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADVMainPanel : BasePanel
{


    protected override void ClickBtn(string btnName)
    {
        switch (btnName) 
        {
            case "StageOne":
                UIMgr.Instance.ShowPanel<ADVStageDetailsPanel>(E_UILayer.System, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                });
                break;
            case "StageTwo":
                break;
            case "StageThree":
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
