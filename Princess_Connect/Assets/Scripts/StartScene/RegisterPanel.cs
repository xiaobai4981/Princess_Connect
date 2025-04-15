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
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
