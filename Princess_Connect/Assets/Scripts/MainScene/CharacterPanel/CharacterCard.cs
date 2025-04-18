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
    public Color[] rarityColors; // ��Ӧ��ͬϡ�жȵı߿�ɫ
    public Sprite starSprite;
    //private CharacterData _data;
    public void Initialize()
    {
        // ���ñ߿���ɫ
        frameImage.color = rarityColors[3];

        // �����Ǽ�
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
