using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMenuPanel : BasePanel
{


    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "BackToBattleBtn":
                BackToBattle();
                break;
            case "BackToMainSceneBtn":
                BackToMainScene();
                break;
        }
    }

    private void BackToBattle()
    {
        // todo 重新开始计时和角色打斗
        BattleManager.Instance.ResumeBattle();
        // 关闭自己
        UIMgr.Instance.HidePanel<BattleMenuPanel>(true);
    }

    private void BackToMainScene()
    {
        UIMgr.Instance.HidePanel<BattleMenuPanel>(true);
        UIMgr.Instance.HidePanel<BattleControlPanel>(true);
        SceneMgr.Instance.LoadSceneAsyn("MainScene");
        UIMgr.Instance.ShowPanel<ADVPanel>(E_UILayer.Middle, (panel) =>
        {
            panel.UpdatePlayerInfo();
        });
        UIMgr.Instance.ShowPanel<BottomBtnPanel>();
    }


    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
