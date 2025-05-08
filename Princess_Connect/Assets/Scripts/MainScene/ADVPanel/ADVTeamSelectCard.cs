using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterFactoryTeamCardData
{
    public int id;  // 卡牌序列号
    public int characterId;  // 角色ID
    public PlayerCharacterData characterData; // 角色位置
    public Sprite characterSprite;  // 角色图片
    public int rankLevel;   // 角色品级
    public Sprite rankSprite;  // 品级图片
    public bool isSelected;  // 是否已被选择

    public CharacterFactoryTeamCardData(int id, int characterId, PlayerCharacterData characterData, Sprite characterSprite, Sprite rankSprite, int rankLevel, bool isSelected)
    {
        this.id = id;
        this.characterId = characterId;
        this.characterData = characterData;
        this.characterSprite = characterSprite;
        this.rankSprite = rankSprite;
        this.rankLevel = rankLevel;
        this.isSelected = isSelected;
    }
}
public class ADVTeamSelectCard : MonoBehaviour
{
    [SerializeField] private Button characterButton;
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image rankBG;

    // 事件：点击时传递卡牌数据
    public UnityEvent<CharacterFactoryTeamCardData> OnCharcterClicked = new UnityEvent<CharacterFactoryTeamCardData>();
    private CharacterFactoryTeamCardData _characterFactoryTeamCardData;

    private void Awake()
    {
        if (characterButton == null) characterButton = this.transform.GetComponent<Button>();
        if (characterIcon == null) characterIcon = this.transform.GetComponent<Image>();
        if (rankBG == null) rankBG = this.transform.Find("Rarity").GetComponent<Image>();

        characterButton.onClick.AddListener(OnCardButtonClicked);
    }

    private void OnDestroy()
    {
        characterButton.onClick.RemoveListener(OnCardButtonClicked);
    }

    public void Initialize(CharacterFactoryTeamCardData data)
    {
        _characterFactoryTeamCardData = data;

        // 设置卡牌外观
        if (characterIcon != null)
        {
            characterIcon.sprite = data.characterSprite;
        }

        if (rankBG != null)
        {
            rankBG.sprite = data.rankSprite;
        }
    }
    // 由按钮点击事件调用
    public void OnCardButtonClicked()
    {
        OnCharcterClicked?.Invoke(_characterFactoryTeamCardData);
    }
}
