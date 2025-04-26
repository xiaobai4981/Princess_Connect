using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;


[System.Serializable]
public class GloryCardData
{
    public Sprite cardSprite;  // 按钮显示的图片
    public int emblemId;  // 徽章ID
    public bool isOwned;  // 是否拥有该卡牌

    public GloryCardData(int id, Sprite sprite, bool owned)
    {
        emblemId = id;
        cardSprite = sprite;
        isOwned = owned;
    }
}
public class MenuGloryCard : MonoBehaviour
{
    [SerializeField] private Button cardButton;
    [SerializeField] private Image cardImage;

    // 事件：点击时传递卡牌数据
    public UnityEvent<GloryCardData> OnCardClicked = new UnityEvent<GloryCardData>();

    private GloryCardData _cardData;

    private void Awake()
    {
        if (cardButton == null) cardButton = this.transform.Find("CardButton").GetComponent<Button>();
        if (cardImage == null) cardImage = GetComponent<Image>();

        cardButton.onClick.AddListener(OnCardButtonClicked);
    }
    private void OnDestroy()
    {
        cardButton.onClick.RemoveListener(OnCardButtonClicked);
    }
    public void Initialize(GloryCardData data)
    {
        _cardData = data;

        // 设置卡牌外观
        if (cardImage != null)
        {
            cardImage.sprite = data.cardSprite;
        }

    }

    // 由按钮点击事件调用
    public void OnCardButtonClicked()
    {
        OnCardClicked?.Invoke(_cardData);
    }
}
