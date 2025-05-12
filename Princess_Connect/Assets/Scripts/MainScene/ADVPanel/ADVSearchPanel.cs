using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ADVSearchPanel : BasePanel
{
    public GameObject[] starCount;
    public Sprite starSprite;
    public TMP_Text restTimes;
    public GameObject searchBtn;
    private int restSearchTimes;
    private void Start()
    {
        int starNum = StageDataMgr.Instance.GetPlayerStageStar(nowPlayerName, 2001);
        restSearchTimes = PlayerDataMgr.Instance.SearchUserIntInfo(nowPlayerName, "search_times");
        for (int i = 0; i < starNum; i++)
        {
            starCount[i].GetComponent<Image>().sprite = starSprite;
        }
        restTimes.text = restSearchTimes.ToString() + "/5";
    }
    private void Update()
    {
        if (restSearchTimes <= 0)
        {
            searchBtn.GetComponent<Button>().interactable = false;
            searchBtn.GetComponent<Image>().color = Color.gray;
        }
    }
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "SearchADV":
                if (restSearchTimes > 0)
                {
                    UIMgr.Instance.ShowPanel<ADVStageDetailsPanel>(E_UILayer.System, (panel) =>
                    {
                        panel.UpdatePlayerName(nowPlayerName);
                        panel.UpdateNowStageIndex(2001);
                    });
                }
                break;
            case "BackToADV":
                UIMgr.Instance.HidePanel<ADVSearchPanel>(true);
                UIMgr.Instance.ShowPanel<ADVPanel>(E_UILayer.Bottom, (panel) =>
                {
                    panel.UpdatePlayerInfo();
                });
                break;
        }

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
