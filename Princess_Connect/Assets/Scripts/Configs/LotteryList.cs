using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public static class LotteryList
{
    public static readonly Dictionary<int, List<int>> normalLotteryList = new Dictionary<int, List<int>>
    {
        {1, new List<int>() { 20001, 25001, 50003} },
        {2, new List<int>() { 101011, 101131, 101251,101281,101371,101401,101521, 101551, 101611} },
        {3, new List<int>() { 104012, 104013, 104014, 104133, 104134, 104222, 104223, 104224, 104343, 104402, 104432, 104462, 104551, 104552, 104611, 104612 } },
        {4, new List<int>() { 105011, 105012, 105013, 105131, 105132, 105133, 105221, 105222, 105223, 105282, 105372, 105551, 105552, 105611, 105612 } },
        {5, new List<int>() { 108011, 108012, 108013, 108131, 108132, 108133, 108221, 108222, 108223, 108342, 108462, 108551, 108552, 108611, 108612 } },
        {6, new List<int>() { 109012, 109013, 109014, 109132, 109133, 109134, 109222, 109223, 109224, 109343, 109463, 109493, 109552, 109553, 109612, 109613 } },
    };

    public static readonly Dictionary<int, List<int>> characterLotteryList = new Dictionary<int, List<int>>
    {
        {1, new List<int>(){1002 }},
        {2, new List<int>(){1011 }},
        {3, new List<int>(){1053 }},
        {4, new List<int>(){1057 }},
        {5, new List<int>(){1058 }},
        {6, new List<int>(){1059 }},
        {7, new List<int>(){1060 }}
    };

    private static readonly Dictionary<int, int> normalLotteryWeights = new Dictionary<int, int>
    {
        {1, 10},
        {2, 20},
        {3, 30},
        {4, 20},
        {5, 15},
        {6, 5}
    };

    private static readonly Dictionary<int, int> characterLotteryWeights = new Dictionary<int, int>
    {
        {1, 10},
        {2, 20},
        {3, 5},
        {4, 5},
        {5, 20},
        {6, 20},
        {7, 20}
    };
    public static int GetRandomLotteryItem(bool isCharacter)
    {
        // 1. 根据权重随机选择1-6中的一个键
        int selectedKey = GetRandomKeyWithWeight(isCharacter);

        // 2. 从选中的键对应的列表中随机选择一个值
        List<int> items = isCharacter? characterLotteryList[selectedKey] : normalLotteryList[selectedKey];
        int randomIndex = items.Count == 1? 0: Random.Range(0, items.Count);
        return items[randomIndex];
    }
    private static int GetRandomKeyWithWeight(bool isCharacter)
    {
        // 计算总权重
        int totalWeight = 0;
        if (isCharacter)
        {
            foreach (var item in characterLotteryWeights.Keys)
            {
                totalWeight += characterLotteryWeights[item];
            }
        }
        else
        {
            foreach (var item in normalLotteryWeights.Keys)
            {
                totalWeight += normalLotteryWeights[item];
            }
        }
        // 生成一个随机数
        int randomNumber = Random.Range(0, totalWeight);

        // 根据权重选择key
        int accumulatedWeight = 0;
        if (isCharacter)
        {
            foreach (var pair in characterLotteryWeights)
            {
                accumulatedWeight += pair.Value;
                if (randomNumber < accumulatedWeight)
                {
                    return pair.Key;
                }
            }
            return 3;
        }
        else
        {
            foreach (var pair in normalLotteryWeights)
            {
                accumulatedWeight += pair.Value;
                if (randomNumber < accumulatedWeight)
                {
                    return pair.Key;
                }
            }
            return 7;
        }
    }
}
