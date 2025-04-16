using I18N.Common;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private void Start()
    {
        DatabaseMgr.Instance.Initialize();
    }

    // 为每一个控件写入操作
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "Login":
                Login();
                break;
            case "Register":
                Resgister();
                break;
        }
    }

    // 登录逻辑
    private void Login()
    {
        // 获取用户名
        string username = GetControl<TMP_InputField>("AccountInput").text;
        // 获取密码
        string password = GetControl<TMP_InputField>("PasswordInput").text;
        // 调用数据库登录方法
        bool result = DatabaseMgr.Instance.LoginUser(username, password);
        if (result)
        {
            SceneMgr.Instance.LoadSceneAsyn("MainScene");
            UIMgr.Instance.HidePanel<LoginPanel>(true);
            UIMgr.Instance.HidePanel<TipsPanel>(true);
            UIMgr.Instance.HidePanel<RegisterPanel>(true);
            UIMgr.Instance.HidePanel<StartPanel>(true);
            UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Bottom, (panel) =>
            {
                panel.UpdatePlayerName(username);
                panel.UpdatePlayerInfo();
            });
            UIMgr.Instance.ShowPanel<BottomBtnPanel>(E_UILayer.Top, (panel) =>
            {
                
            });
        }
        else
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("用户名或密码错误！");
            });
        }
    }
    // 注册逻辑
    private void Resgister()
    {
        UIMgr.Instance.HidePanel<LoginPanel>();
        UIMgr.Instance.ShowPanel<RegisterPanel>(E_UILayer.Middle, (panel) =>
        {

        });
    }
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
