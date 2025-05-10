using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpineInfo
{
    public List<string> skeletonDataAssetPath;
    public List<string> normalSpineNum;
    public List<string> fightSpineNum;
    public string eventSpineNum;
}


public static class CharacterSpineFileConfig
{
    public static readonly Dictionary<int, CharacterSpineInfo> charactersSpineInfo = new Dictionary<int, CharacterSpineInfo>()
    {
        {1002, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/YY/100261_SkeletonData"
            },
            normalSpineNum = new List<string>(){"07"},
            fightSpineNum = new List<string>(){"100201"},
            eventSpineNum = "000000"
        } },
        {1011, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/LN/101161_SkeletonData"
            },
            normalSpineNum = new List<string>(){"08"},
            fightSpineNum = new List<string>(){"101101"},
            eventSpineNum = "000000"
        } },
        {1053, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/MNK/MNKNormal/105311_SkeletonData", "SpineAnim/MNK/MNKSpecial/105361_SkeletonData"
            },
            normalSpineNum = new List<string>(){"04"},
            fightSpineNum = new List<string>(){"105301"},
            eventSpineNum = "000000"
        } },
        {1057, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/JT/JTNormal/105731_SkeletonData", "SpineAnim/JT/JTSpecial/170161_SkeletonData"
            },
            normalSpineNum = new List<string>(){"04", "34"},
            fightSpineNum = new List<string>(){"105701", "170101"},
            eventSpineNum = "000000"
        } },
        {1058, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/PKLM/105861_SkeletonData"
            },
            normalSpineNum = new List<string>(){"05"},
            fightSpineNum = new List<string>(){"105801"},
            eventSpineNum = "000000"
        } },
        {1059, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/KKL/105961_SkeletonData"
            },
            normalSpineNum = new List<string>(){"06"},
            fightSpineNum = new List<string>(){"105901"},
            eventSpineNum = "000000"
        } },
        {1060, new CharacterSpineInfo(){
            skeletonDataAssetPath = new List<string>(){
                "SpineAnim/KL/106061_SkeletonData"
            },
            normalSpineNum = new List<string>(){"07"},
            fightSpineNum = new List<string>(){"106001"},
            eventSpineNum = "000000"
        } },
        {200400, new CharacterSpineInfo(){
            normalSpineNum = new List<string>(){"200400"},
        } },
        {200500, new CharacterSpineInfo(){
            normalSpineNum = new List<string>(){"200500"},
        } },
        {200501, new CharacterSpineInfo(){
            normalSpineNum = new List<string>(){"200501"},
        } },
        {300800, new CharacterSpineInfo(){
            normalSpineNum = new List<string>(){"300800"},
        } }
    };
}
