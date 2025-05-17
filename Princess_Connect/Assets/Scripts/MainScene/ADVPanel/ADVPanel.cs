using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADVPanel : BasePanel
{
    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "MainBtn":
                UIMgr.Instance.HidePanel<ADVPanel>();
                UIMgr.Instance.ShowPanel<ADVMainPanel>(E_UILayer.Middle, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                    panel.UpdatePlayerInfo();
                });
                break;
            case "SearchBtn":
                UIMgr.Instance.HidePanel<ADVPanel>();
                UIMgr.Instance.ShowPanel<ADVSearchPanel>(E_UILayer.Middle, (panel) =>
                {
                    panel.UpdatePlayerName(nowPlayerName);
                    panel.UpdatePlayerInfo();
                });
                break;
            case "ArenaBtn":
                UIMgr.Instance.HidePanel<ADVPanel>();
                UIMgr.Instance.ShowPanel<ADVArenaPanel>(E_UILayer.Middle, (panel) =>
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
        if (MusicMgr.Instance.GetNowBKMusicName() != "CharacterBG")
        {
            MusicMgr.Instance.PlayBKMusic("CharacterBG");
        }
    }
}
