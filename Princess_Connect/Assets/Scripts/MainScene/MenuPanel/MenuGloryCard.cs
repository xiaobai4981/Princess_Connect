using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Unity.VisualScripting;


[System.Serializable]
public class ButtonData
{
    public Sprite buttonSprite;  // 按钮显示的图片
    public int emblemId;  // 徽章ID

    public ButtonData(int id, Sprite image)
    {
        emblemId = id;
        buttonSprite = image;
    }
}
public class MenuGloryCard : MonoBehaviour
{
    public UnityEvent<ButtonData> OnButtonClicked;

    [SerializeField] private Button button;
    [SerializeField] private Image buttonImage;

    private ButtonData _currentData;

    // 初始化按钮方法
    public void Awake()
    {
        if (button == null) button = GetComponentInChildren<Button>();
        if (buttonImage == null && button != null)
            buttonImage = button.GetComponent<Image>();

        button?.onClick.AddListener(OnClick);
    }

    // 初始化Panel按钮
    public void Initialize(ButtonData data)
    {
        _currentData = data;

        if (buttonImage != null)
        {
            buttonImage.sprite = data.buttonSprite;
        }
    }

    private void OnClick()
    {
        OnButtonClicked?.Invoke(_currentData);
    }
}
