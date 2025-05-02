using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStarControl : BasePanel
{
    public GameObject[] starCount;

    public Image characterPiece;
    public TMP_Text characterPiecesDesc;
    public Transform piecesNeedFill;
    public TMP_Text PiecesNeedText;

    public GameObject princessOrbNeed;
    public Image princessOrb;
    public TMP_Text princessOrbDesc;
    public Transform princessOrbNeedFill;
    public TMP_Text princessOrbNeedText;

    public Button starUpBtn;

    PlayerCharacterCollection playerCharacterCollection;
    PlayerCharacterData nowPlayerCharacterData;
    PlayerInventoryInfo playerInventoryInfo;

    private void Start()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "player_character_data.json");
        playerCharacterCollection = JsonMapper.ToObject<PlayerCharacterCollection>(File.ReadAllText(filePath));
        nowPlayerCharacterData = playerCharacterCollection.characters[nowCharacterId.ToString()];
        string filePath1 = Path.Combine(Application.persistentDataPath, "player_inventory_data.json");
        playerInventoryInfo = JsonMapper.ToObject<PlayerInventoryInfo>(File.ReadAllText(filePath1));
        UpdatequipBtns();
    }

    private void UpdatequipBtns()
    {
        
    }

    private void Update()
    {
        
    }

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
