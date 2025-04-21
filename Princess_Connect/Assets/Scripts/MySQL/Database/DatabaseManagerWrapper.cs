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
        // 游戏退出时确保关闭
        DatabaseMgr.Instance.DatabaseClose();
    }
}
