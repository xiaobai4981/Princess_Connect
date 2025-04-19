using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuPanel : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ÷ÿ–¥Click
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "Item":

                break;
            case "Setting":
                
                break;
            case "Designation":
                
                break;
            case "BackToStart":
                BackToStart();
                break;
        }
    }

    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    private void BackToStart()
    {
        DatabaseMgr.Instance.DatabaseClose();
        UIMgr.Instance.ShowPanel<StartPanel>(E_UILayer.Bottom, (panel) =>
        {

        });
        UIMgr.Instance.HidePanel<MenuPanel>(true);
        UIMgr.Instance.HidePanel<LotteryPanel>(true);
        UIMgr.Instance.HidePanel<BeginPanel>(true);
        UIMgr.Instance.HidePanel<CharacterPanel>(true);
        UIMgr.Instance.HidePanel<ADVPanel>(true);
        UIMgr.Instance.HidePanel<BottomBtnPanel>(true);
    }

    public override void HideMe()
    {

    }

    public override void ShowMe()
    {

    }

}
