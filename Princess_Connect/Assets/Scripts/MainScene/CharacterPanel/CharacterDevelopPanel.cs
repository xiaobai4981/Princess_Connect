using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDevelopPanel : BasePanel
{
    public GameObject[] characterDetailPanel;
    // 保存页面的类型
    private static readonly Dictionary<Type, Action> PanelHiders = new Dictionary<Type, Action>()
    {
        // todo 每个面板的隐藏操作，有些面板里面的小面板需要移除
        { typeof(CharacterEquipControl), () => UIMgr.Instance.HidePanel<CharacterEquipControl>(true) },
        { typeof(CharacterMaterialControl), () => UIMgr.Instance.HidePanel<CharacterMaterialControl>(true) },
        //{ typeof(CharacterSkillControl), () => UIMgr.Instance.HidePanel<CharacterSkillControl>(true) },
        //{ typeof(CharacterStarControl), () => UIMgr.Instance.HidePanel<CharacterStarControl>(true) },
        //{ typeof(CharacterDetailControl), () => UIMgr.Instance.HidePanel<CharacterDetailControl>(true) }
    };
    #region 初始化按钮
    // 按钮背景的字典
    private Dictionary<string, Button> buttonDataDict;
    private Dictionary<Button, Sprite> originalSprites = new Dictionary<Button, Sprite>();
    private bool isFirst = true;
    // Start is called before the first frame update
    void OnEnable()
    {
        InitializeButtons("EquipmentBtn");
    }

    private void InitializeButtons(string defaultBtnName)
    {
        Transform ModeBtn = transform.Find("ConsoleBtn");
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
                if (child.GetComponent<Button>().transition == Selectable.Transition.SpriteSwap)
                    buttonDataDict[child.name] = child.GetComponent<Button>();
            }
            isFirst = false;
        }
        else
        {
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
    }


    #endregion
    // 重写Click方法
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "EquipmentBtn":
                SetActiveButton("EquipmentBtn");
                break;
            case "MaterialBtn":
                SetActiveButton("MaterialBtn");
                break;
            case "SkillBtn":
                SetActiveButton("SkillBtn");
                break;
            case "StarBtn":
                SetActiveButton("StarBtn");
                break;
            case "DetailBtn":
                SetActiveButton("DetailBtn");
                break;
            case "NextCharacter":

                break;
            case "LastCharacter":

                break;
            case "BackToCharacterMenu":
                UIMgr.Instance.HidePanel<CharacterDevelopPanel>(true);
                UIMgr.Instance.ShowPanel<CharacterPanel>();
                break;
        }
    }
    public void SetActiveButton(string btnName)
    {
        if (!buttonDataDict.TryGetValue(btnName, out var activeData)) return;
        Transform ModeBtn = transform.Find("ConsoleBtn");
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
        switch (btnName)
        {
            case "EquipmentBtn":
                SwitchPanel<CharacterEquipControl>();
                break;
            case "MaterialBtn":
                SwitchPanel<CharacterMaterialControl>();
                break;
            //case "SkillBtn":
            //    SwitchPanel<CharacterSkillControl>();
            //    break;
            //case "StarBtn":
            //    SwitchPanel<CharacterStarControl>();
            //    break;
            //case "DetailBtn":
            //    SwitchPanel<CharacterDetailontrol>();
            //    break;
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
        if (typeof(T) == typeof(CharacterEquipControl))
        {
            UIMgr.Instance.ShowPanel<T>(E_UILayer.Bottom, (panel) =>
            {
                panel.UpdatePlayerName(nowPlayerName);
                panel.UpdateCharacterId(nowCharacterId);
            });
        }
    }

    // 更新当前的人物id
    public override void UpdateCharacterId(int id)
    {
        nowCharacterId = id;
    }
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }
}
