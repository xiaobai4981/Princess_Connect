using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADVArenaPanel : BasePanel
{
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "BackToADV":
                UIMgr.Instance.HidePanel<ADVArenaPanel>(true);
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
