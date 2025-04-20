using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIMgr.Instance.ShowPanel<StartPanel>(E_UILayer.Bottom, (panel) =>
        {
            
        });
        GameObject dbManagerObj = new GameObject("DatabaseMgrWrapper");
        DatabaseManagerWrapper wrapper = dbManagerObj.AddComponent<DatabaseManagerWrapper>();
        DontDestroyOnLoad(dbManagerObj);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
