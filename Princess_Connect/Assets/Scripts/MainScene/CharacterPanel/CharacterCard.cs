using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCard : MonoBehaviour
{
    public Image frameImage;
    public Image characterImage;
    public GameObject[] stars;
    public Color[] rarityColors; // 对应不同稀有度的边框色
    public Sprite starSprite;
    //private CharacterData _data;
    public void Initialize()
    {
        // 设置边框颜色
        frameImage.color = rarityColors[3];

        // 设置星级
        for (int i = 0; i < stars.Length; i++)
        {
            if (i < 5)
            {
                Image starImage = stars[i].GetComponent<Image>();
                starImage.sprite = starSprite;
            }
            else
            {
                stars[i].SetActive(i < 6);
            }
        }
    }
}
