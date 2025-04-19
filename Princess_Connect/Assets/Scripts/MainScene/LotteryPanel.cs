using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryPanel : BasePanel
{
    // 按钮背景的字典
    private Dictionary<string, Button> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    // 两种扭蛋的页面
    public GameObject CharacterLot;
    public GameObject NormalLot;
    private bool isFirst = true;
    // Start is called before the first frame update
    void OnEnable()
    {
        InitializeButtons("CharacterLotBtn");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 初始化按钮
    private void InitializeButtons(string defaultBtnName)
    {
        Transform ModeBtn = transform.Find("LotteryPart/LotteryModeBtn");
        if (isFirst)
        {
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
            // 保存按钮数据
            buttonDataDict = new Dictionary<string, Button>();
            foreach (Transform child in ModeBtn)
            {
                buttonDataDict[child.name] = child.GetComponent<Button>();
            }
            isFirst = false;
        }
        else
        {
            Debug.Log(buttonDataDict);
            foreach (var pair in buttonDataDict)
            {
                Button btn = ModeBtn.Find(pair.Key).GetComponent<Button>();
                Image btnImg = btn.GetComponent<Image>();
                btnImg.sprite = originalSprites[btn];
            }
        }
        // 设置默认按钮为按下状态
        Transform defaultBtn = ModeBtn.Find(defaultBtnName);
        if (defaultBtn != null)
        {
            Button btn = defaultBtn.GetComponent<Button>();
            if (btn != null)
            {
                SpriteState spriteState = btn.spriteState;
                Image btnImage = btn.GetComponent<Image>();
                btnImage.sprite = spriteState.pressedSprite;
            }
        }
        CharacterLot.SetActive(true);
        NormalLot.SetActive(false);
    }

    // 重写Click方法
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "CharacterLotBtn":
                SetActiveButton("CharacterLotBtn");
                break;
            case "NormalLotBtn":
                SetActiveButton("NormalLotBtn");
                break;
            case "NormalBtn":
                
                break;
            case "OneTime":
                
                break;
            case "TenTime":
                
                break;
        }
    }

    public void SetActiveButton(string btnName)
    {
        if (btnName == "CharacterLotBtn")
        {
            CharacterLot.SetActive(true);
            NormalLot.SetActive(false);
        }
        else
        {
            CharacterLot.SetActive(false);
            NormalLot.SetActive(true);
        }
        if (!buttonDataDict.TryGetValue(btnName, out var activeData)) return;
        Transform ModeBtn = transform.Find("LotteryPart/LotteryModeBtn");
        foreach (var pair in buttonDataDict)
        {
            bool isActive = pair.Key == btnName;
            // 手动设置 Sprite（不依赖 Button 组件）
            Image btnImage = pair.Value.GetComponent<Image>();
            btnImage.sprite = isActive ?
            pair.Value.spriteState.pressedSprite :
            originalSprites[pair.Value];
            if (!isActive)
            {
                Button btn = ModeBtn.Find(pair.Key).GetComponent<Button>();
                Image btnImg = btn.GetComponent<Image>();
                btnImg.sprite = originalSprites[btn];
            }
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
