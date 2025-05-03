using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUpConfig
{
    public string pieceMaterialNum;
    public int needPieceMaterial;
    public bool isNeedOrd;
    public int needOrdNum;
    public int needMana;
}

public class CharacterStarUp
{
    public Dictionary<int, StarUpConfig> starUpConfigs;
}

public static class CharacterStarUpConfig
{
    public static readonly CharacterStarUp characterStarUpConfig = new CharacterStarUp()
    {
        starUpConfigs = new Dictionary<int, StarUpConfig>()
        {
            {1, new StarUpConfig() { pieceMaterialNum = "11", needPieceMaterial = 10, isNeedOrd = false, needOrdNum = 0, needMana = 10000} },
            {2, new StarUpConfig() { pieceMaterialNum = "11", needPieceMaterial = 15, isNeedOrd = false, needOrdNum = 0, needMana = 15000 } },
            {3, new StarUpConfig() { pieceMaterialNum = "11", needPieceMaterial = 25, isNeedOrd = false, needOrdNum = 0, needMana = 25000 } },
            {4, new StarUpConfig() { pieceMaterialNum = "11", needPieceMaterial = 40, isNeedOrd = false, needOrdNum = 0, needMana = 40000 } },
            {5, new StarUpConfig() { pieceMaterialNum = "61", needPieceMaterial = 50, isNeedOrd = true, needOrdNum = 50, needMana = 50000 } }
        }
    };
}
