using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuSettingPanel : BasePanel
{
    public TextMeshProUGUI BGValueNum;
    public TextMeshProUGUI SoundValueNum;
    public TextMeshProUGUI CVValueNum;
    public override void HideMe()
    {
        
    }

    public override void ShowMe()
    {
        
    }

    // 改写Button的OnClick方法
    protected override void ClickBtn(string buttonName)
    {
        switch (buttonName)
        {
            case "BackToMenu":
                UIMgr.Instance.HidePanel<MenuSettingPanel>();
                break;
        }
    }

    // 改写Slider的ValueChange方法
    protected override void SliderValueChange(string sliderName, float value)
    {
        switch (sliderName)
        {
            case "BGSlider":
                MusicMgr.Instance.ChangeBKMusicValue((value * 1.0f) / 10);
                BGValueNum.text = value.ToString();
                break;
            case "SoundSlider":

                SoundValueNum.text = value.ToString();
                break;
            case "CVSlider":

                CVValueNum.text = value.ToString();
                break;
        }
    }
}
