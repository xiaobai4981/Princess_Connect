using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        bool result = PlayerDataMgr.Instance.RegisterUser(username, password);
        if (result)
        {
#region 新用户注册成功，将用户默认拥有的道具和角色自动添加
            // 体力更新时间设为新用户注册时间
            Dictionary<string, object> columnUpdates = new Dictionary<string, object>
            {
                { "last_stamina_update", System.DateTime.Now },
            };
            PlayerDataMgr.Instance.ModifyUserIntInfo(username, columnUpdates, false);
            // 添加默认道具
            string defaultInventory = File.ReadAllText("Assets/Resources/Configs/NewPlayerInventoryInit.json");
            string defaultCharacterEquipment = File.ReadAllText("Assets/Resources/Configs/NewPlayerCharacterEquipment.json");
            string defaultCharacterSkillLevel = File.ReadAllText("Assets/Resources/Configs/NewPlayerCharacterSkillLevel.json");
            GloryDataMgr.Instance.UpdateUserPlayerFactoryInfo(username, defaultInventory);
            // 添加默认角色
            AddDefaultRole(username, new List<int>{ 1002, 1011, 1058, 1059, 1060}, defaultCharacterEquipment, defaultCharacterSkillLevel);
            #endregion
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

    private void AddDefaultRole(string username, List<int> roleIds, string defaultCharacterEquipment, string defaultCharacterSkillLevel)
    {
        foreach (int roleId in roleIds)
        {
            CharacterDataMgr.Instance.AddUserPlayerInfo(username, roleId);
            CharacterDataMgr.Instance.InitPlayerCharacter(username, roleId, defaultCharacterEquipment, defaultCharacterSkillLevel);
        }
    }

    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
