using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ADVArenaEnemyCard : MonoBehaviour
{
    public TMP_Text playerName;
    public TMP_Text playerLevel;
    public TMP_Text playerCE;
    public GameObject[] teamBlock;
    private Button characterButton;
    public UnityEvent<List<CharacterFactoryTeamCardData>> OnCharcterClicked = new UnityEvent<List<CharacterFactoryTeamCardData>>();
    private List<CharacterFactoryTeamCardData> _arenaEnemyTeamCardData;

    private void Awake()
    {
        if (characterButton == null) characterButton = this.transform.GetComponent<Button>();

        characterButton.onClick.AddListener(OnCardButtonClicked);
    }

    public void Initialize(string playerName, int playerLevel, List<CharacterFactoryTeamCardData> data)
    {
        _arenaEnemyTeamCardData = data;
        int ce = 0;
        for (int i = 0; i < data.Count; i++)
        {
            ce += data[i].characterData.current_stats.ce;
            teamBlock[i].GetComponent<Image>().sprite = data[i].characterSprite;
            teamBlock[i].transform.Find("Rarity").GetComponent<Image>().sprite = data[i].rankSprite;
        }
        this.playerName.text = playerName;
        this.playerLevel.text = playerLevel.ToString();
        playerCE.text = ce.ToString();
    }
    private void OnDestroy()
    {
        characterButton.onClick.RemoveListener(OnCardButtonClicked);
    }
    // 由按钮点击事件调用
    public void OnCardButtonClicked()
    {
        OnCharcterClicked?.Invoke(_arenaEnemyTeamCardData);
    }
}
