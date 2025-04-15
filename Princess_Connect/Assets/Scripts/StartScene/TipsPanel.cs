using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TipsPanel : BasePanel
{
    public TMP_Text tipsText;
    // Update is called once per frame
    void Update()
    {
        
    }
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "SureBtn":
                BackToLogin();
                break;
        }
    }

    // ��ʾ��ʾ��Ϣ
    public void ShowTips(string tips)
    {
        tipsText.text = tips;
    }

    // ���ص�¼����
    public void BackToLogin()
    {
        UIMgr.Instance.HidePanel<TipsPanel>();
        UIMgr.Instance.ShowPanel<LoginPanel>();
    }

    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
