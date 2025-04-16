using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterPanel : BasePanel
{
    // 为每一个控件写入操作
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "Register":
                Resgister();
                break;
            case "BackToLogin":
                BackToLogin();
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    // 注册操作
    private void Resgister()
    {
        // 获取用户名
        string username = GetControl<TMP_InputField>("AccountInput").text;
        // 获取密码和确认密码
        string password = GetControl<TMP_InputField>("PasswordInput").text;
        string rePassword = GetControl<TMP_InputField>("RePasswordInput").text;

        // 用户名为空
        if (username == "")
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("用户名不能为空！");
            });
            return;
        }

        // 密码为空
        if (password == "")
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("密码不能为空！");
            });
            return;
        }

        // 确认密码为空
        if (rePassword == "")
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("请确认密码！");
            });
            return;
        }

        // 密码和确认密码不一致
        if (password!= rePassword)
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("两次输入的密码不一致！");
            });
            return;
        }
        // 调用数据库登录方法
        bool result = DatabaseMgr.Instance.RegisterUser(username, password);
        if (result)
        {
            Dictionary<string, object> columnUpdates = new Dictionary<string, object>
            {
                { "level", 1 },
                { "totalexp", 10 },{ "nowexp", 0 },
                { "totalab", 100 }, { "nowab", 100 },
                { "manacnt", 33333 }, { "diamondcnt", 33333 },
                { "missioncomplete", 0 }
            };
            DatabaseMgr.Instance.ModifyUserIntInfo(username, columnUpdates, false);
            // 关闭注册界面
            UIMgr.Instance.HidePanel<RegisterPanel>();
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("注册成功，请返回登录界面进行登录！");
            });
        }
        else
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("用户名已存在！");
            });
        }
    }

    // 返回登录界面
    private void BackToLogin()
    {
        UIMgr.Instance.HidePanel<RegisterPanel>();
        UIMgr.Instance.ShowPanel<LoginPanel>();
    }

    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
