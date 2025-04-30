using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterKeyWordDic
{
    public Dictionary<string, string> KeyWordDic;
}
public static class KeyWordMapConfig
{
    public static readonly CharacterKeyWordDic characterKeyWordDicConfig = new CharacterKeyWordDic()
    {
        KeyWordDic = new Dictionary<string, string>()
        {
            {"hp", "生命值"},
            {"atk", "攻击力"},
            {"physical_def", "物理防御"},
            {"magical_def", "魔法防御"},
            {"crit", "暴击率"},
            {"dod", "闪避"},
            {"hp_recover_per_wave", "HP自动回复"},
            {"tp_recover_per_wave", "TP自动回复"},
            {"tp_get", "技能值上升"},
            {"hit_rate", "命中"}
        }
    };
}
