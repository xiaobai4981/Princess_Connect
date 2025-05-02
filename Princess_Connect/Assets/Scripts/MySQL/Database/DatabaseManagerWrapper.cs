using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseManagerWrapper : MonoBehaviour
{
    private void Awake()
    {
        // 初始化单例
        DatabaseMgr.Instance.Initialize();
    }

    private void OnApplicationQuit()
    {
        EventCenter.Instance.EventTrigger(E_EventType.E_UpLoadDataBeforeQuit);
        // 游戏退出时确保关闭
        DatabaseMgr.Instance.DatabaseClose();
    }
}
