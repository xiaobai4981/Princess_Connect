using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterFactoryTeamCardDataInBattle
{
    public CharacterFactoryTeamCardData characterFactoryTeamCardData;
    public int nowHp;
    public int nowTp;
}

public class BattleControlPanel : BasePanel
{
    // 战斗场景的背景图
    private Sprite BG;

    public GameObject[] characterUBControls;
    public Sprite star;
    public TMP_Text timeText;
    private List<CharacterFactoryTeamCardDataInBattle> characterTeamCardDataInBattle = new List<CharacterFactoryTeamCardDataInBattle>();

    private void Start()
    {
        for (int i = 0; i < characterTeamCardDataInBattle.Count; i++)
        {
            // 更新角色图片
            characterUBControls[i].GetComponent<Image>().sprite = characterTeamCardDataInBattle[i].characterFactoryTeamCardData.characterSprite;
            // 更新角色边框
            characterUBControls[i].transform.Find("Rarity").GetComponent<Image>().sprite = characterTeamCardDataInBattle[i].characterFactoryTeamCardData.rankSprite;
            // 更新Hp
            RectTransform hp = characterUBControls[i].transform.Find("Hp/HpFill").gameObject.GetComponent<RectTransform>();
            hp.sizeDelta = new Vector2((float)characterTeamCardDataInBattle[i].nowHp / (float)characterTeamCardDataInBattle[i].characterFactoryTeamCardData.characterData.current_stats.hp * 180f, 
                hp.sizeDelta.y);
            // 更新TP
            RectTransform tp = characterUBControls[i].transform.Find("Tp/TpFill").gameObject.GetComponent<RectTransform>();
            tp.sizeDelta = new Vector2(0f, hp.sizeDelta.y);
            // 更新星星
            for (int j = 0; j < characterTeamCardDataInBattle[i].characterFactoryTeamCardData.characterData.current_star; j++)
            {
                if (j < 5)
                {
                    characterUBControls[i].transform.Find($"StarCount/Star{j + 1}").GetComponent<Image>().sprite = star;
                }
                else
                {
                    characterUBControls[i].transform.Find($"StarCount/Star{j + 1}").gameObject.SetActive(false);
                }
            }
        }
        for (int i = characterTeamCardDataInBattle.Count; i < characterUBControls.Length; i++)
        {
            characterUBControls[i].SetActive(false);
        }
    }

    private void Update()
    {
        // 更新时间
        if (BattleManager.Instance != null)
        {
            int min = (int)((BattleManager.Instance.battleDuration - BattleManager.Instance.battleTimer) / 60);
            int sec = (int)((BattleManager.Instance.battleDuration - BattleManager.Instance.battleTimer) % 60);
            timeText.text = $"{min}" + ":" + $"{sec}";
        }
        
        // todo 更新HP和TP

    }

    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "MenuBtn":
                // todo 停止计时和角色打斗
                BattleManager.Instance.isBattleActive = false;
                // 打开面板
                UIMgr.Instance.ShowPanel<BattleMenuPanel>(E_UILayer.System);
                break;
        }
    }
    public void UpdateCharacterTeamCardData(List<CharacterFactoryTeamCardDataInBattle> characterTeamCardData)
    {
        this.characterTeamCardDataInBattle = characterTeamCardData;
    }
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        if (MusicMgr.Instance.GetNowBKMusicName() != "WhileAttack")
        {
            MusicMgr.Instance.PlayBKMusic("WhileAttack");
        }
    }
}
