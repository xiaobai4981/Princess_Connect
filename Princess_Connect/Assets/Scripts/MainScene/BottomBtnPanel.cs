using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BottomBtnPanel : BasePanel
{
    // 控制按钮动画的字典
    private Dictionary<string, ButtonData> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    private class ButtonData
    {
        public Button button;
        public Animator animator;
    }

    private void InitializeButtons(string defaultBtnName)
    {
        // 停止所有按钮动画
        foreach (Transform child in transform)
        {
            Animator anim = child.Find(child.name + "Img")?.GetComponent<Animator>();
            if (anim != null)
            {
                anim.SetBool("isPressed", false);
            }
        }
        // 保存按钮的原始图片
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            if (btn.transition == Selectable.Transition.SpriteSwap)
            {
                Image img = btn.GetComponent<Image>();
                if (img != null)
                {
                    originalSprites[btn] = img.sprite;
                }
            }
        }
        
        // 设置默认按钮为按下状态
        Transform defaultBtn = transform.Find(defaultBtnName);
        if (defaultBtn != null)
        {
            Animator defaultAnim = defaultBtn.Find(defaultBtnName + "Img")?.GetComponent<Animator>();
            if (defaultAnim != null)
            {
                defaultAnim.SetBool("isPressed", true);

                Button btn = defaultBtn.GetComponent<Button>();
                if (btn != null)
                {
                    SpriteState spriteState = btn.spriteState;
                    Image btnImage = btn.GetComponent<Image>();
                    btnImage.sprite = spriteState.pressedSprite;
                }
            }
        }

        buttonDataDict = new Dictionary<string, ButtonData>();
        foreach (Transform child in transform)
        {
            var animator = child.GetComponentInChildren<Animator>();
            if (animator != null)
            {
                buttonDataDict[child.name] = new ButtonData
                {
                    button = child.GetComponent<Button>(),
                    animator = animator
                };
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        InitializeButtons("Home");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 为每一个控件写入操作
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "Home":
                SetActiveButton("Home");
                break;
            case "Character":
                SetActiveButton("Character");
                break;
            case "ADV":
                SetActiveButton("ADV");
                break;
            case "Lottery":
                SetActiveButton("Lottery");
                break;
            case "Menu":
                SetActiveButton("Menu");
                break;
        }
    }

    // 设置动画，并设置不同的界面
    public void SetActiveButton(string btnName)
    {
        if (!buttonDataDict.TryGetValue(btnName, out var activeData)) return;

        foreach (var pair in buttonDataDict)
        {
            bool isActive = pair.Key == btnName;
            pair.Value.animator.SetBool("isPressed", isActive);
            if (!isActive)
            {
                Button btn = this.transform.Find(pair.Key).GetComponent<Button>();
                Image btnImage = btn.GetComponent<Image>();
                btnImage.sprite = originalSprites[btn];
            }
        }
        switch (btnName)
        {
            case "Home":
                UIMgr.Instance.ShowPanel<BeginPanel>(E_UILayer.Bottom);
                //UIMgr.Instance.HidePanel<CharacterPanel>(E_UILayer.Bottom);
                //UIMgr.Instance.HidePanel<ADVPanel>(E_UILayer.Bottom);
                //UIMgr.Instance.HidePanel<LotteryPanel>(E_UILayer.Bottom);
                UIMgr.Instance.HidePanel<MenuPanel>();
                break;
            //case "Character":
            //    UIMgr.Instance.HidePanel<BeginPanel>();
            //    //UIMgr.Instance.ShowPanel<CharacterPanel>(E_UILayer.Bottom);
            //    //UIMgr.Instance.HidePanel<ADVPanel>();
            //    //UIMgr.Instance.HidePanel<LotteryPanel>();
            //    UIMgr.Instance.HidePanel<MenuPanel>();
            //    break;
            //case "ADV":
            //    UIMgr.Instance.HidePanel<BeginPanel>();
            //    //UIMgr.Instance.HidePanel<CharacterPanel>();
            //    //UIMgr.Instance.ShowPanel<ADVPanel>(E_UILayer.Bottom);
            //    //UIMgr.Instance.HidePanel<LotteryPanel>();
            //    UIMgr.Instance.HidePanel<MenuPanel>();
            //    break;
            //case "Lottery":
            //    UIMgr.Instance.HidePanel<BeginPanel>();
            //    //UIMgr.Instance.HidePanel<CharacterPanel>();
            //    //UIMgr.Instance.HidePanel<ADVPanel>();
            //    //UIMgr.Instance.ShowPanel<LotteryPanel>(E_UILayer.Bottom);
            //    UIMgr.Instance.HidePanel<MenuPanel>();
            //    break;
            case "Menu":
                UIMgr.Instance.HidePanel<BeginPanel>();
                //UIMgr.Instance.HidePanel<CharacterPanel>();
                //UIMgr.Instance.HidePanel<ADVPanel>();
                //UIMgr.Instance.HidePanel<LotteryPanel>();
                UIMgr.Instance.ShowPanel<MenuPanel>(E_UILayer.Bottom);
                break;
        }
    }


    public override void HideMe()
    {

    }

    public override void ShowMe()
    {

    }
}
