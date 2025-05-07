using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADVTeamSelectPanel : BasePanel
{
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "CancelBtn":
                UIMgr.Instance.HidePanel<ADVTeamSelectPanel>(true);
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
