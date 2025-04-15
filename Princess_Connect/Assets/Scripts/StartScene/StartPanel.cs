using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StartPanel : BasePanel
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UIMgr.Instance.ShowPanel<LoginPanel>(E_UILayer.Middle);
        }

    }

    public void TestFun()
    {
        
    }

    public override void ShowMe()
    {
        
    }

    public override void HideMe()
    {
        
    }
}
