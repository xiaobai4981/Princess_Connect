using LitJson;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDevelopPanel : BasePanel
{
    #region 初始化角色信息
    // 刚开养成界面时，更新当前角色信息并向事件中心注册更新角色信息的事件
    // 需要更新的信息
    public TMP_Text characterNameText;
    public TMP_Text manaText;
    public Image characterIcon;
    public Image rarityFrame;
    public GameObject[] starCount;
    public Sprite starSprite;
    public TMP_Text LikbNum;
    public TMP_Text LevelNum;
    public TMP_Text CENum;
    public TMP_Text RankNum;
    public SkeletonGraphic characterSkel;

    PlayerInfo nowPlayerInfo;
    PlayerCharacterData nowPlayerCharacterData;

    private void Start()
    {
        SwitchPanel<CharacterEquipControl>();
        UpdateCharacterInfo();
        //EventCenter.Instance.AddEventListener("UpdateCharacterInfo", UpdateCharacterInfo);
    }
    public void UpdateCharacterInfo()
    {
        RefreshPanel();
        PlayCharacterAnim();
    }
    private void RefreshPanel()
    {
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        string filePath2 = Path.Combine(Application.persistentDataPath, "player_data.json");
        nowPlayerInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath2));
        PlayerCharacterCollection playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath1));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        // 更新角色信息
        characterNameText.text = nowPlayerCharacterData.character_name;
        manaText.text = nowPlayerInfo.mana_cnt.ToString("N0");
        LikbNum.text = nowPlayerCharacterData.current_likb.ToString();
        LevelNum.text = nowPlayerCharacterData.level.ToString();
        CENum.text = nowPlayerCharacterData.current_stats.ce.ToString();
        RankNum.text = nowPlayerCharacterData.current_rank.ToString();
        for (int i = 0; i < nowPlayerCharacterData.current_star; i++)
        {
            if (i < 5)
            {
                Image starImage = starCount[i].GetComponent<Image>();
                starImage.sprite = starSprite;
            }
            else
            {
                starCount[i].SetActive(true);
            }
        }
        // 更新角色头像和框架
        // 加载角色卡片图片
        string rankNum;
        if (nowPlayerCharacterData.current_star <= 3)
        {
            rankNum = "11";
        }
        else if (nowPlayerCharacterData.current_star <= 5)
        {
            rankNum = "31";
        }
        else
        {
            rankNum = "61";
        }
        string characterSpriteFileName = $"icon_unit_{nowCharacterId}" + rankNum;
        ABResMgr.Instance.LoadResAsync<Sprite>("character_icon", characterSpriteFileName, (res) =>
        {
            characterIcon.sprite = res;
        }, true);
        // 加载边框
        ABResMgr.Instance.LoadResAsync<Sprite>("rank_frame", $"character_rank_{nowPlayerCharacterData.current_rank}", (res) =>
        {
            rarityFrame.sprite = res;
        }, true);
    }

    private void PlayCharacterAnim(string animName = "init")
    {
        string spineType;
        int spineNum = nowPlayerCharacterData.current_star == 6 && CharacterSpineFileConfig.charactersSpineInfo[nowCharacterId].normalSpineNum.Count > 1 ? 1 : 0;
        string spineAssetPath = CharacterSpineFileConfig.charactersSpineInfo[nowCharacterId].skeletonDataAssetPath[spineNum];
        string normalSpineNum = CharacterSpineFileConfig.charactersSpineInfo[nowCharacterId].normalSpineNum[spineNum];
        characterSkel.Clear();
        characterSkel.skeletonDataAsset = Resources.Load<SkeletonDataAsset>(spineAssetPath);
        characterSkel.Initialize(true);

        if (animName == "init") 
        {
            spineType = "_landing";
        }
        else
        {
            spineType = "_" + animName;
        }
        // 播放land动画（不循环）
        var landTrack = characterSkel.AnimationState.SetAnimation(0, normalSpineNum + spineType, false);

        // land动画结束后播放idle动画（循环）
        characterSkel.AnimationState.AddAnimation(0, normalSpineNum + "_idle", true, landTrack.AnimationEnd);
    }
    #endregion

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
            case "BackToCharacterMenu":
                foreach (var panel in PanelHiders)
                {
                    panel.Value.Invoke();
                }
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

        UIMgr.Instance.ShowPanel<T>(E_UILayer.Middle, (panel) =>
        {
            panel.UpdatePlayerName(nowPlayerName);
            panel.UpdateCharacterId(nowCharacterId);
        });
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
