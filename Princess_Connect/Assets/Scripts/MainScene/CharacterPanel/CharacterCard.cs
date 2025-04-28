using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class CharacterFactoryCardData
{
    public int characterId;  // 角色ID
    public Sprite characterSprite;  // 角色图片
    public int characterStar; // 角色星级
    public int rankLevel;   // 角色品级
    public Sprite rankSprite;  // 品级图片

    public CharacterFactoryCardData(int characterId, Sprite characterSprite, int characterStar, Sprite rankSprite, int rankLevel)
    {
        this.characterId = characterId;
        this.characterSprite = characterSprite;
        this.characterStar = characterStar;
        this.rankSprite = rankSprite;
        this.rankLevel = rankLevel;
    }
}

public class CharacterCard : MonoBehaviour
{
    public GameObject[] stars;
    public Sprite starSprite;
    [SerializeField] private Button characterButton;
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image rankBG;
    // 事件：点击时传递卡牌数据
    public UnityEvent<CharacterFactoryCardData> OnCharcterClicked = new UnityEvent<CharacterFactoryCardData>();

    private CharacterFactoryCardData _characterFactoryCardData;
    private void Awake()
    {
        if (characterButton == null) characterButton = this.transform.Find("CharacterBtn").GetComponent<Button>();
        if (characterIcon == null) characterIcon = this.transform.Find("CharacterBtn").GetComponent<Image>();
        if (rankBG == null) rankBG = this.transform.Find("CharacterBtn").Find("Rarity").GetComponent<Image>();

        characterButton.onClick.AddListener(OnCardButtonClicked);
    }
    private void OnDestroy()
    {
        characterButton.onClick.RemoveListener(OnCardButtonClicked);
    }
    public void Initialize(CharacterFactoryCardData data)
    {
        _characterFactoryCardData = data;

        // 设置卡牌外观
        if (characterIcon != null)
        {
            characterIcon.sprite = data.characterSprite;
        }

        if (rankBG != null)
        {
            rankBG.sprite = data.rankSprite;
        }

        // 设置星级
        for (int i = 0; i < data.characterStar; i++)
        {
            if (i < 5)
            {
                Image starImage = stars[i].GetComponent<Image>();
                starImage.sprite = starSprite;
            }
            else
            {
                stars[i].SetActive(true);
            }
        }

    }

    // 由按钮点击事件调用
    public void OnCardButtonClicked()
    {
        OnCharcterClicked?.Invoke(_characterFactoryCardData);
    }
}
