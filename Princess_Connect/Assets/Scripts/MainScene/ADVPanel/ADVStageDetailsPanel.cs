using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADVStageDetailsPanel : BasePanel
{
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "BeginBtn":
                UIMgr.Instance.ShowPanel<ADVTeamSelectPanel>(E_UILayer.System, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                });
                break;
            case "CancelBtn":
                UIMgr.Instance.HidePanel<ADVStageDetailsPanel>(true);
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
