using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RegisterPanel : BasePanel
{
    // Ϊÿһ���ؼ�д�����
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

    // ע�����
    private void Resgister()
    {
        // ��ȡ�û���
        string username = GetControl<TMP_InputField>("AccountInput").text;
        // ��ȡ�����ȷ������
        string password = GetControl<TMP_InputField>("PasswordInput").text;
        string rePassword = GetControl<TMP_InputField>("RePasswordInput").text;

        // �û���Ϊ��
        if (username == "")
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("�û�������Ϊ�գ�");
            });
            return;
        }

        // ����Ϊ��
        if (password == "")
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("���벻��Ϊ�գ�");
            });
            return;
        }

        // ȷ������Ϊ��
        if (rePassword == "")
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("��ȷ�����룡");
            });
            return;
        }

        // �����ȷ�����벻һ��
        if (password!= rePassword)
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("������������벻һ�£�");
            });
            return;
        }
        // �������ݿ��¼����
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
            // �ر�ע�����
            UIMgr.Instance.HidePanel<RegisterPanel>();
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("ע��ɹ����뷵�ص�¼������е�¼��");
            });
        }
        else
        {
            UIMgr.Instance.ShowPanel<TipsPanel>(E_UILayer.Top, (panel) =>
            {
                panel.ShowTips("�û����Ѵ��ڣ�");
            });
        }
    }

    // ���ص�¼����
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
