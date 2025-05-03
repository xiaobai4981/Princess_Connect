using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class CharacterDetailControl : BasePanel
{
    // 需要修改的文本
    public TMP_Text ceText;
    public TMP_Text physicAtkText;
    public TMP_Text magicalAtkText;
    public TMP_Text physicDefText;
    public TMP_Text magicalDefText;
    public TMP_Text physicCritText;
    public TMP_Text magicalCritText;
    public TMP_Text hpText;
    public TMP_Text dodText;
    public TMP_Text hpRecoverText;
    public TMP_Text tpRecoverText;
    public TMP_Text tpGetText;
    public TMP_Text hitRateText;

    PlayerCharacterCollection playerCharacterCollection;
    PlayerCharacterData nowPlayerCharacterData;
    PlayerInventoryInfo playerInventoryInfo;

    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        UpdatequipBtns();
    }
    private void UpdatequipBtns()
    {
        ceText.text = nowPlayerCharacterData.current_stats.ce.ToString();
        physicAtkText.text = nowPlayerCharacterData.current_stats.attack_type == "physical" ?nowPlayerCharacterData.current_stats.atk.ToString() : "0";
        magicalAtkText.text = nowPlayerCharacterData.current_stats.attack_type == "magical" ? nowPlayerCharacterData.current_stats.atk.ToString() : "0";
        physicDefText.text = nowPlayerCharacterData.current_stats.physical_def.ToString();
        magicalDefText.text = nowPlayerCharacterData.current_stats.magical_def.ToString();
        physicCritText.text = nowPlayerCharacterData.current_stats.attack_type == "physical" ? nowPlayerCharacterData.current_stats.crit.ToString() : "0";
        magicalCritText.text = nowPlayerCharacterData.current_stats.attack_type == "magical" ? nowPlayerCharacterData.current_stats.crit.ToString() : "0";
        hpText.text = nowPlayerCharacterData.current_stats.hp.ToString();
        dodText.text = nowPlayerCharacterData.current_stats.dod.ToString();
        hpRecoverText.text = nowPlayerCharacterData.current_stats.hp_recover_per_wave.ToString();
        tpRecoverText.text = nowPlayerCharacterData.current_stats.tp_recover_per_wave.ToString();
        tpGetText.text = nowPlayerCharacterData.current_stats.tp_get.ToString();
        hitRateText.text = nowPlayerCharacterData.current_stats.hit_rate.ToString();
    }


    // 更新当前的人物id
    public override void UpdateCharacterId(int id)
    {
        this.nowCharacterId = id;
    }

    // 更新当前的玩家
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
