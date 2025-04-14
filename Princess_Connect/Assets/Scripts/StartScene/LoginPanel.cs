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
        }
        else
        {
            //UIMgr.Instance.ShowPanel<TipsPanel>();
        }
    }
    // ע���߼�
    private void Resgister()
    {
        UIMgr.Instance.HidePanel<LoginPanel>();
        // UIMgr.Instance.ShowPanel<RegisterPanel>();
    }
    public override void HideMe()
    {
        throw new System.NotImplementedException();
    }

    public override void ShowMe()
    {
        throw new System.NotImplementedException();
    }
}
