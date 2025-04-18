using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryPanel : BasePanel
{
    public GameObject CharacterLot;
    public GameObject NormalLot;
    public GameObject defaultBtn;
    private Sprite defaltNormalState;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = defaultBtn.GetComponent<Button>();
        SpriteState spriteState = btn.spriteState;
        Image btnImage = btn.GetComponent<Image>();
        defaltNormalState = btnImage.sprite;
        btnImage.sprite = spriteState.pressedSprite;
        CharacterLot.SetActive(true);
        NormalLot.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ÷ÿ–¥Click∑Ω∑®
    protected override void ClickBtn(string btnName)
    {
        switch (btnName)
        {
            case "CharacterLotBtn":
                CharacterLot.SetActive(true);
                NormalLot.SetActive(false);
                break;
            case "NormalLotBtn":
                Button btn = defaultBtn.GetComponent<Button>();
                Image btnImage = btn.GetComponent<Image>();
                btnImage.sprite = defaltNormalState;
                CharacterLot.SetActive(false);
                NormalLot.SetActive(true);
                break;
            case "NormalBtn":

                break;
            case "OneTime":

                break;
            case "TenTime":

                break;
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
