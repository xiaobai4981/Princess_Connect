using LitJson;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSkillControl : BasePanel
{
    // Ub技能的更新组件
    public Image ubSkillImage;
    public TMP_Text ubSkillName;
    public TMP_Text ubSkillDescription;
    public TMP_Text ubSkillEffect;
    public TMP_Text ubSkillLevelText;
    public TMP_Text ubSkillNeedManaText;
    public Button ubSkillLevelUpBtn;

    // 1技能的更新组件
    public Image skill1Image;
    public TMP_Text skill1Name;
    public TMP_Text skill1Description;
    public TMP_Text skill1Effect;
    public TMP_Text skill1LevelText;
    public TMP_Text skill1NeedManaText;
    public Button skill1LevelUpBtn;

    // 2技能的更新组件
    public Image skill2Image;
    public TMP_Text skill2Name;
    public TMP_Text skill2Description;
    public TMP_Text skill2Effect;
    public TMP_Text skill2LevelText;
    public TMP_Text skill2NeedManaText;
    public Button skill2LevelUpBtn;

    // ex技能的更新组件
    public Image exSkillImage;
    public TMP_Text exSkillName;
    public TMP_Text exSkillDescription;
    public TMP_Text exSkillEffect;
    public TMP_Text exSkillLevelText;
    public TMP_Text exSkillNeedManaText;
    public Button exSkillLevelUpBtn;

    PlayerCharacterCollection playerCharacterCollection;
    PlayerCharacterData nowPlayerCharacterData;
    PlayerInfo nowPlayerInfo;

    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        string filePath2 = Path.Combine(Application.persistentDataPath, "player_data.json");
        nowPlayerInfo = JsonMapper.ToObject<PlayerInfo>(File.ReadAllText(filePath2));
        UpdateBtns();
    }

    private void UpdateBtns()
    {
        string ub = nowPlayerCharacterData.current_star == 6? "s_ub_skill" : "n_ub_skill";
        string skill1 = "skill_1";
        string skill2 = "skill_2";
        string exSkill = "ex_skill";
        // 更新ub技能信息
        if (ub == "s_ub_skill")
        {
            ABResMgr.Instance.LoadResAsync<Sprite>("skill_icon", $"icon_skill_" + nowPlayerCharacterData.skills_config.s_ub_skill.id, (res) =>
            {
                ubSkillImage.sprite = res;
            }, true);
            ubSkillName.text = nowPlayerCharacterData.skills_config.s_ub_skill.name;
            ubSkillDescription.text = nowPlayerCharacterData.skills_config.s_ub_skill.description;
            ubSkillEffect.text = CharacterSkillEffectInter.SkillEffectInter(nowPlayerCharacterData, nowPlayerCharacterData.skills_config.s_ub_skill.effect, ub);
        }
        else
        {
            ABResMgr.Instance.LoadResAsync<Sprite>("skill_icon", $"icon_skill_" + nowPlayerCharacterData.skills_config.n_ub_skill.id, (res) =>
            {
                ubSkillImage.sprite = res;
            }, true);
            ubSkillName.text = nowPlayerCharacterData.skills_config.n_ub_skill.name;
            ubSkillDescription.text = nowPlayerCharacterData.skills_config.n_ub_skill.description;
            ubSkillEffect.text = CharacterSkillEffectInter.SkillEffectInter(nowPlayerCharacterData, nowPlayerCharacterData.skills_config.n_ub_skill.effect, ub);
        }
        ubSkillLevelText.text = nowPlayerCharacterData.skills_level[ub].ToString();
        ubSkillNeedManaText.text = (nowPlayerCharacterData.skills_level[ub] * CharacterSkillUpConfig.characterSkillUpConfig[ub]).ToString();

        // 更新1技能信息
        ABResMgr.Instance.LoadResAsync<Sprite>("skill_icon", $"icon_skill_" + nowPlayerCharacterData.skills_config.skill_1.id, (res) =>
        {
            skill1Image.sprite = res;
        }, true);
        skill1Name.text = nowPlayerCharacterData.skills_config.skill_1.name;
        skill1Description.text = nowPlayerCharacterData.skills_config.skill_1.description;
        skill1Effect.text = CharacterSkillEffectInter.SkillEffectInter(nowPlayerCharacterData, nowPlayerCharacterData.skills_config.skill_1.effect, skill1);
        skill1LevelText.text = nowPlayerCharacterData.skills_level[skill1].ToString();
        skill1NeedManaText.text = (nowPlayerCharacterData.skills_level[skill1] * CharacterSkillUpConfig.characterSkillUpConfig[skill1]).ToString();

        // 更新2技能信息
        ABResMgr.Instance.LoadResAsync<Sprite>("skill_icon", $"icon_skill_" + nowPlayerCharacterData.skills_config.skill_2.id, (res) =>
        {
            skill2Image.sprite = res;
        }, true);
        skill2Name.text = nowPlayerCharacterData.skills_config.skill_2.name;
        skill2Description.text = nowPlayerCharacterData.skills_config.skill_2.description;
        skill2Effect.text = CharacterSkillEffectInter.SkillEffectInter(nowPlayerCharacterData, nowPlayerCharacterData.skills_config.skill_2.effect, skill2);
        skill2LevelText.text = nowPlayerCharacterData.skills_level[skill2].ToString();
        skill2NeedManaText.text = (nowPlayerCharacterData.skills_level[skill2] * CharacterSkillUpConfig.characterSkillUpConfig[skill2]).ToString();

        // 更新ex技能信息
        ABResMgr.Instance.LoadResAsync<Sprite>("skill_icon", $"icon_skill_" + nowPlayerCharacterData.skills_config.ex_skill.id, (res) =>
        {
            exSkillImage.sprite = res;
        }, true);
        exSkillName.text = nowPlayerCharacterData.skills_config.ex_skill.name;
        exSkillDescription.text = nowPlayerCharacterData.skills_config.ex_skill.description;
        exSkillEffect.text = CharacterSkillEffectInter.SkillEffectInter(nowPlayerCharacterData, nowPlayerCharacterData.skills_config.ex_skill.effect, exSkill);
        exSkillLevelText.text = nowPlayerCharacterData.skills_level[exSkill].ToString();
        exSkillNeedManaText.text = (nowPlayerCharacterData.skills_level[exSkill] * CharacterSkillUpConfig.characterSkillUpConfig[exSkill]).ToString();
    }

    private void Update()
    {
        string nowSkillName = nowPlayerCharacterData.current_star == 6 ? "s_ub_skill" : "n_ub_skill";
        if (nowPlayerCharacterData.skills_level[nowSkillName] < nowPlayerCharacterData.level)
        {
            if (nowPlayerInfo.mana_cnt > int.Parse(ubSkillNeedManaText.text))
            {
                ubSkillLevelUpBtn.interactable = true;
                ubSkillLevelUpBtn.image.color = Color.white;
            }
            else
            {
                ubSkillLevelUpBtn.interactable = false;
                ubSkillLevelUpBtn.image.color = Color.gray;
            }
        }
        else
        {
            ubSkillLevelUpBtn.interactable = false;
            ubSkillLevelUpBtn.image.color = Color.gray;
        }
        
        if (nowPlayerCharacterData.skills_level["skill_1"] < nowPlayerCharacterData.level)
        {
            if (nowPlayerInfo.mana_cnt > int.Parse(skill1NeedManaText.text))
            {
                skill1LevelUpBtn.interactable = true;
                skill1LevelUpBtn.image.color = Color.white;
            }
            else
            {
                skill1LevelUpBtn.interactable = false;
                skill1LevelUpBtn.image.color = Color.gray;
            }
        }
        else
        {
            skill1LevelUpBtn.interactable = false;
            skill1LevelUpBtn.image.color = Color.gray;
        }
        
        if (nowPlayerCharacterData.skills_level["skill_2"] < nowPlayerCharacterData.level)
        {
            if (nowPlayerInfo.mana_cnt > int.Parse(skill2NeedManaText.text))
            {
                skill2LevelUpBtn.interactable = true;
                skill2LevelUpBtn.image.color = Color.white;
            }
            else
            {
                skill2LevelUpBtn.interactable = false;
                skill2LevelUpBtn.image.color = Color.gray;
            }
        }
        else
        {
            skill2LevelUpBtn.interactable = false;
            skill2LevelUpBtn.image.color = Color.gray;
        }
        
        if (nowPlayerCharacterData.skills_level["ex_skill"] < nowPlayerCharacterData.level)
        {
            if (nowPlayerInfo.mana_cnt > int.Parse(exSkillNeedManaText.text))
            {
                exSkillLevelUpBtn.interactable = true;
                exSkillLevelUpBtn.image.color = Color.white;
            }
            else
            {
                exSkillLevelUpBtn.interactable = false;
                exSkillLevelUpBtn.image.color = Color.gray;
            }
        }
        else
        {
            exSkillLevelUpBtn.interactable = false;
            exSkillLevelUpBtn.image.color = Color.gray;
        }
        
    }

    protected override void ClickBtn(string btnName)
    {
        string nowSkillName;
        switch (btnName)
        {
            case "UbSkillLevelUpBtn":
                nowSkillName = nowPlayerCharacterData.current_star == 6 ? "s_ub_skill" : "n_ub_skill";
                if (ubSkillLevelUpBtn.interactable)
                    SkillUp(nowSkillName);
                break;
            case "Skill1LevelUpBtn":
                nowSkillName = "skill_1";
                if (skill1LevelUpBtn.interactable)
                    SkillUp(nowSkillName);
                break;
            case "Skill2LevelUpBtn":
                nowSkillName = "skill_2";
                if (skill2LevelUpBtn.interactable)
                    SkillUp(nowSkillName);
                break;
            case "ExSkillLevelUpBtn":
                nowSkillName = "ex_skill";
                if (exSkillLevelUpBtn.interactable)
                    SkillUp(nowSkillName);
                break;
        }
    }

    private void SkillUp(string nowSkillName)
    {
        // 升级
        nowPlayerInfo.mana_cnt -= CharacterSkillUpConfig.characterSkillUpConfig[nowSkillName] * nowPlayerCharacterData.skills_level[nowSkillName];
        nowPlayerCharacterData.skills_level[nowSkillName] += 1;
        nowPlayerCharacterData.current_stats.ce += 10;
        // 本地保存数据
        string filePath = Path.Combine(Application.persistentDataPath, "player_data.json");
        File.WriteAllText(filePath, JsonMapper.ToJson(nowPlayerInfo));
        PlayerDataMgr.Instance.ModifyUserIntInfo(nowPlayerName, new Dictionary<string, object>() { { "mana_cnt", nowPlayerInfo.mana_cnt } }, false);
        playerCharacterCollection.characters[nowCharacterId.ToString()] = nowPlayerCharacterData;
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "player_character_data.json"), JsonMapper.ToJson(playerCharacterCollection));
        // 触发更新
        UpdateBtns();
        EventCenter.Instance.EventTrigger<string>(E_EventType.E_Character_Develop_Update, "joy_short");
    }

    // 更新当前的人物id
    public override void UpdateCharacterId(int nowCharacterId)
    {
        this.nowCharacterId = nowCharacterId;
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
