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

    // Ϊÿһ���ؼ�д�����
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

    // ��¼�߼�
    private void Login()
    {
        // ��ȡ�û���
        string username = GetControl<TMP_InputField>("AccountInput").text;
        // ��ȡ����
        string password = GetControl<TMP_InputField>("PasswordInput").text;
        // �������ݿ��¼����
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
                panel.ShowTips("�û������������");
            });
        }
    }
    // ע���߼�
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
