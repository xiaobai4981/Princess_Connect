using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLevelUpReward
{
    public int hpLevelGrowth;
    public int atkLevelGrowth;
    public int defLevelGrowth;
    public Dictionary<string, Dictionary<int, CharacterUpReward>> otherUpRewards;
}
public class CharacterUpReward
{
    public int hpGrowth;
    public int atkGrowth;
    public int defGrowth;
}

public static class RewardConfig
{
    public static readonly CharacterLevelUpReward characterDevelopConfig = new CharacterLevelUpReward()
    {
        hpLevelGrowth = 50,
        atkLevelGrowth = 30,
        defLevelGrowth = 20,
        otherUpRewards = new Dictionary<string, Dictionary<int, CharacterUpReward>>()
        {
            // 星级提升奖励
            {"Star", 
                new Dictionary<int, CharacterUpReward>()
                {
                    {2, new CharacterUpReward() {hpGrowth = 4 * 50 , atkGrowth = 4 * 30, defGrowth = 4 * 20}},
                    {3, new CharacterUpReward() {hpGrowth = 9 * 50 , atkGrowth = 9 * 30, defGrowth = 9 * 20}},
                    {4, new CharacterUpReward() {hpGrowth = 16 * 50 , atkGrowth = 16 * 30, defGrowth = 16 * 20}},
                    {5, new CharacterUpReward() {hpGrowth = 25 * 50 , atkGrowth = 25 * 30, defGrowth = 25 * 20}},
                    {6, new CharacterUpReward() {hpGrowth = 36 * 50 , atkGrowth = 36 * 30, defGrowth = 36 * 20}}
                }
            },
            // 品级提升奖励
            {"Rank",
                new Dictionary<int, CharacterUpReward>()
                {
                    {2, new CharacterUpReward() {hpGrowth = 4 * 50 , atkGrowth = 4 * 30, defGrowth = 4 * 20}},
                    {3, new CharacterUpReward() {hpGrowth = 6 * 50 , atkGrowth = 6 * 30, defGrowth = 6 * 20}},
                    {4, new CharacterUpReward() {hpGrowth = 8 * 50 , atkGrowth = 8 * 30, defGrowth = 8 * 20}},
                    {5, new CharacterUpReward() {hpGrowth = 10 * 50 , atkGrowth = 10 * 30, defGrowth = 10 * 20}},
                }
            },
            // 好感度提升奖励
            {"Likb",
                new Dictionary<int, CharacterUpReward>()
                {
                    {2, new CharacterUpReward() {hpGrowth = 2 * 10 , atkGrowth = 2 * 10, defGrowth = 0}},
                    {3, new CharacterUpReward() {hpGrowth = 3 * 10 , atkGrowth = 3 * 10, defGrowth = 0}},
                    {4, new CharacterUpReward() {hpGrowth = 4 * 10 , atkGrowth = 4 * 10, defGrowth = 0}},
                    {5, new CharacterUpReward() {hpGrowth = 5 * 10 , atkGrowth = 5 * 10, defGrowth = 0}},
                    {6, new CharacterUpReward() {hpGrowth = 6 * 10 , atkGrowth = 6 * 10, defGrowth = 0}},
                    {7, new CharacterUpReward() {hpGrowth = 7 * 10 , atkGrowth = 7 * 10, defGrowth = 0}},
                    {8, new CharacterUpReward() {hpGrowth = 8 * 10 , atkGrowth = 8 * 10, defGrowth = 0}},
                    {9, new CharacterUpReward() {hpGrowth = 9 * 10 , atkGrowth = 9 * 10, defGrowth = 0}},
                    {10, new CharacterUpReward() {hpGrowth = 10 * 10 , atkGrowth = 10 * 10, defGrowth = 0}},
                }
            }
        }
    };
}
