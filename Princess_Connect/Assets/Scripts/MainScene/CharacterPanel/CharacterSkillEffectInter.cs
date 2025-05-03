using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public static class CharacterSkillEffectInter
{
    public static string SkillEffectInter(PlayerCharacterData playerCharacterData, List<Effect> effects, string skillName)
    {
        CharacterStats stats = playerCharacterData.current_stats;
        Dictionary<string, int> variables = new Dictionary<string, int>();
        variables.Add("level", playerCharacterData.skills_level[skillName]);
        variables.Add("atk", playerCharacterData.current_stats.atk);
        variables.Add("physical_def", playerCharacterData.current_stats.physical_def);
        variables.Add("magic_def", playerCharacterData.current_stats.magical_def);
        variables.Add("hp", playerCharacterData.current_stats.hp);
        string skillEffectDes = "";
        foreach (Effect effect in effects)
        {
            string text = effect.text;
            string parsedFormula = effect.formula;
            foreach (var pair in variables)
            {
                parsedFormula = parsedFormula.Replace(pair.Key, pair.Value.ToString());
            }
            string result = Math.Ceiling(CalculateExpression(parsedFormula)).ToString();

            // 3. 替换文本中的 [value]
            string finalText = text.Replace("[value]", result);
            skillEffectDes += finalText + "\n";
        }
        return skillEffectDes;
    }
    private static float CalculateExpression(string formula)
    {
        try
        {
            var result = new DataTable().Compute(formula, null);
            return Convert.ToSingle(result);
        }
        catch (Exception e)
        {
            Debug.LogError($"计算表达式失败: {formula}\n错误: {e.Message}");
            return 0f;
        }
    }
}
