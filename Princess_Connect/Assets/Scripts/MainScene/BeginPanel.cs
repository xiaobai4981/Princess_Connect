using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class BeginPanel : BasePanel
{
    private string nowPlayerName;
    public RectTransform Exp;
    public RectTransform AB;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    // 记录现在的玩家
    public void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
    }

    // 更新玩家信息
    public void UpdatePlayerInfo()
    {
        int level = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "level");
        int nowExp = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "nowexp");
        int totalExp = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "totalexp");
        int nowAB = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "nowab");
        int totalAB = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "totalab");
        int manaCnt = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "manacnt");
        int diamondCnt = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "diamondcnt");
        int missionCnt = DatabaseMgr.Instance.SearchUserIntInfo(nowPlayerName, "missioncomplete");
        // 等级更新
        TMP_Text levelText = GetControl<TMP_Text>("Level");
        levelText.text = level.ToString();
        // 经验更新
        Exp.sizeDelta = new Vector2((float)nowExp / (float)totalExp * 250f, Exp.sizeDelta.y);
        // 体力更新
        AB.sizeDelta = new Vector2((float)nowAB / (float)totalAB * 250f, Exp.sizeDelta.y);
        TMP_Text ABText = GetControl<TMP_Text>("ABText");
        ABText.text = nowAB.ToString() + "/" + totalAB.ToString();
        // 玛娜更新
        TMP_Text manaText = GetControl<TMP_Text>("ManaText");
        manaText.text = manaCnt.ToString("N0");
        // 钻石更新
        TMP_Text diamondText = GetControl<TMP_Text>("DiamondText");
        diamondText.text = diamondCnt.ToString("N0");
        // 任务完成数更新
        TMP_Text missionText = GetControl<TMP_Text>("CntNum");
        missionText.text = missionCnt.ToString();
    }

    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
