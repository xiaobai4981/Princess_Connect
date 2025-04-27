using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class ItemCardData
{
    public int itemId;  // 徽章ID
    public int itemNum;  // 数量
    public Sprite itemIcon;  // 图标

    public ItemCardData(int itemId, int itemNum, Sprite itemIcon)
    {
        this.itemId = itemId;
        this.itemNum = itemNum;
        this.itemIcon = itemIcon;
    }
}

public class MenuItemCard : MonoBehaviour
{
    [SerializeField] private Button itemButton;
    [SerializeField] private Image itemIcon;
    // 事件：点击时传递卡牌数据
    public UnityEvent<ItemCardData> OnItemClicked = new UnityEvent<ItemCardData>();

    private ItemCardData _itemData;
    private void Awake()
    {
        if (itemButton == null) itemButton = this.transform.Find("ItemBtn").GetComponent<Button>();
        if (itemIcon == null) itemIcon = this.transform.Find("ItemBtn").GetComponent<Image>();

        itemButton.onClick.AddListener(OnCardButtonClicked);
    }
    private void OnDestroy()
    {
        itemButton.onClick.RemoveListener(OnCardButtonClicked);
    }
    public void Initialize(ItemCardData data)
    {
        _itemData = data;

        // 设置卡牌外观
        if (itemIcon != null)
        {
            itemIcon.sprite = data.itemIcon;
        }

    }

    // 由按钮点击事件调用
    public void OnCardButtonClicked()
    {
        OnItemClicked?.Invoke(_itemData);
    }

}
