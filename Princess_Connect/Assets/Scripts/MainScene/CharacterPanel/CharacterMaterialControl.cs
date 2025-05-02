using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaterialControl : BasePanel
{

    // 更新当前的人物id
    public override void UpdateCharacterId(int nowCharacterId)
    {
        this.nowCharacterId = nowCharacterId;
    }
    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }
    public override void HideMe()
    {

    }

    public override void ShowMe()
    {

    }
}
