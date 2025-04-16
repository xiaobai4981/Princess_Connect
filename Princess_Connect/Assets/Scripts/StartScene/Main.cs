using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIMgr.Instance.ShowPanel<StartPanel>(E_UILayer.Bottom, (panel) =>
        {
            
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
