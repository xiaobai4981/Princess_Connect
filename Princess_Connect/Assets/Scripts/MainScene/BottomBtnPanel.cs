using System;
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
    // 保存页面的类型
    private static readonly Dictionary<Type, Action> PanelHiders = new Dictionary<Type, Action>() 
    {
        { typeof(BeginPanel), () => UIMgr.Instance.HidePanel<BeginPanel>() },
        //{ typeof(CharacterPanel), () => UIMgr.Instance.HidePanel<CharacterPanel>() },
        { typeof(ADVPanel), () => UIMgr.Instance.HidePanel<ADVPanel>() },   
        { typeof(LotteryPanel), () => UIMgr.Instance.HidePanel<LotteryPanel>() },
        { typeof(MenuPanel), () => UIMgr.Instance.HidePanel<MenuPanel>() }
    };
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
    public override void UpdatePlayerName(string nowPlayerName)
    {
        this.nowPlayerName = nowPlayerName;
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
                SwitchPanel<BeginPanel>();
                break;
            case "Character":
                //SwitchPanel<CharacterPanel>();
                break;
            case "ADV":
                SwitchPanel<ADVPanel>();
                break;
            case "Lottery":
                SwitchPanel<LotteryPanel>();
                break;
            case "Menu":
                SwitchPanel<MenuPanel>();
                break;
        }
    }

    // 切换面板的操作
    private void SwitchPanel<T>() where T : BasePanel
    {
        foreach (var panel in PanelHiders)
        {
            if (panel.Key == typeof(T))
                continue;
            panel.Value.Invoke();
        }
        UIMgr.Instance.ShowPanel<T>(E_UILayer.Bottom, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            //if (typeof(T) != typeof(CharacterPanel))
            //{
                panel.UpdatePlayerInfo();
            //}
            
        });
    }

    public override void HideMe()
    {

    }

    public override void ShowMe()
    {

    }
}
