using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BattleUnit : MonoBehaviour
{
    #region 动画名字相关
    private SkeletonGraphic skeleton;
    private Spine.AnimationState spineState;
    private string characterNormalSpineNum;
    private string characterSkillSpineNum;
    private string monsterSpineNum;

    // 普通动画
    private string idleAnim;
    private string runAnim;
    private string attackAnim;
    private string damageAnim;
    private string deathAnim;
    private string winAnim;
    // 技能动画


    // 庆祝动画

    #endregion
    // 开始移动相关
    public float spawnOffsetX = 500f; // 生成时的X轴偏移
    public float moveDuration = 2f; // 移动耗时
    private Vector3 targetPosition;

    #region 战斗属性
    public bool isAlly;
    public bool isAlive => currentHp > 0;
    private int skillIndex;
    public float attackInterval;
    public float attackIntervalVariation = 0.15f;
    #endregion

    #region 游戏数据相关
    private CharacterFactoryTeamCardDataInBattle characterData;
    private MonsterData monsterData;
    private int currentHp;
    private int currentTp;
    private int currentAtk;
    #endregion
    // 事件
    public Action onMoveComplete;

    public void Initialize(bool isAlly, CharacterFactoryTeamCardDataInBattle characterData)
    {
        NormalSetupSpine(characterData.characterFactoryTeamCardData.characterData.character_id,
                 characterData.characterFactoryTeamCardData.characterData.current_star);
        // todo 还有技能动画的初始化
        this.isAlly = isAlly;
        this.characterData = characterData;
        this.attackInterval = characterData.characterFactoryTeamCardData.characterData.current_stats.atk_cap / 1000;
        currentHp = characterData.nowHp;
        currentTp = characterData.nowTp;
        currentAtk = characterData.characterFactoryTeamCardData.characterData.current_stats.atk;

        InitializePosition();
        StartCoroutine(DelayedActivation());
    }

    public void InitializeMonster(bool isAlly, MonsterData monsterData)
    {
        NormalSetupSpine(monsterData.monster_id, 0);
        // todo 还有技能动画的初始化（如果有）
        this.isAlly = isAlly;
        this.monsterData = monsterData;
        this.attackInterval = monsterData.base_stats.atk_cap / 1000;
        currentHp = monsterData.base_stats.hp;
        currentAtk = monsterData.base_stats.atk;

        InitializePosition();
        StartCoroutine(DelayedActivation());
    }

    private void InitializePosition()
    {
        targetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        transform.position = isAlly ?
            new Vector3(transform.position.x - spawnOffsetX, transform.position.y, 0) :
            new Vector3(transform.position.x + spawnOffsetX, transform.position.y, 0);
    }

    private void NormalSetupSpine(int id, int star)
    {
        skeleton = GetComponent<SkeletonGraphic>();
        spineState = skeleton.AnimationState;

        var spineInfo = CharacterSpineFileConfig.charactersSpineInfo[id];
        int spineNum = (star == 6 && spineInfo.normalSpineNum.Count > 1) ? 1 : 0;

        string prefix = spineInfo.normalSpineNum[spineNum];
        
        idleAnim = $"{prefix}_idle";
        runAnim = $"{prefix}_run";
        attackAnim = $"{prefix}_attack";
        damageAnim = $"{prefix}_damage";
        deathAnim = $"{prefix}_die";
        if (star != 0)
        {
            string suffix = spineInfo.fightSpineNum[spineNum];
            winAnim = $"{suffix}_joyResult";
        }
    }

    private IEnumerator DelayedActivation()
    {
        gameObject.SetActive(true);
        yield return null; // 确保一帧更新
        StartCoroutine(EnterBattlefield());
    }

    private IEnumerator EnterBattlefield()
    {
        // 播放跑步动画
        spineState.SetAnimation(0, runAnim, true);

        // 移动过程
        float elapsed = 0;
        Vector3 startPos = transform.position;
        while (elapsed < moveDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPosition, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        // 切换到待机状态
        PlayIdleAnim();
        onMoveComplete?.Invoke();
    }
    public void PlayIdleAnim()
    {
        spineState.ClearTracks();

        // 播放idle动画并循环
        spineState.SetAnimation(0, idleAnim, true);
    }
    public void PlayWinAnim()
    {
        spineState.ClearTracks();

        // 播放idle动画并循环
        spineState.SetAnimation(0, winAnim, false);
    }

    public IEnumerator PerformAction(List<BattleUnit> targets)
    {
        BattleUnit target = targets[targets.Count - 1];
        if (target == null) yield break;
        // todo 播放攻击动画 包括普通攻击和技能攻击
        spineState.SetAnimation(0, attackAnim, false);
        yield return new WaitForSeconds(0.3f); // 攻击前摇
        // todo 计算伤害
        bool isCritical = UnityEngine.Random.value < 0.1f; // 10%暴击率
        int damage = isCritical ? currentAtk * 2 : currentAtk;
        target.TakeDamage(damage, isCritical);

        yield return new WaitForSeconds(1f); // 攻击后摇
        PlayIdleAnim();
    }

    public void TakeDamage(int amount, bool isCritical = false)
    {
        currentHp = Mathf.Max(0, currentHp - amount);

        Vector3 displayPos = transform.position + Vector3.up * 1.5f;
        //DamageNumberController.Instance.ShowDamage(displayPos, amount, isCritical);

        spineState.SetAnimation(1, damageAnim, false);

        if (currentHp <= 0) StartCoroutine(Die());
    }


    // 死亡动画
    private IEnumerator Die()
    {
        spineState.ClearTracks();
        spineState.SetAnimation(0, deathAnim, false);
        BattleManager.Instance.RemoveUnit(this);
        yield return new WaitForSeconds(0.5f);

        
        yield return StartCoroutine(FadeToGray());

        gameObject.SetActive(false);

    }

    public float GetActualAttackInterval()
    {
        return attackInterval * UnityEngine.Random.Range(
            1 - attackIntervalVariation,
            1 + attackIntervalVariation
        );
    }

    private IEnumerator FadeToGray()
    {
        float elapsed = 0f;

        Color originalColor = skeleton.color;
        if (originalColor == default) originalColor = Color.white;

        while (elapsed < 1.5f)
        {
            float t = elapsed / 1.5f;

            // 颜色插值
            Color lerpedColor = Color.Lerp(originalColor, Color.gray, t);

            // 透明度插值
            lerpedColor.a = Mathf.Lerp(1f, 0f, t);

            // 应用颜色
            skeleton.color = lerpedColor;

            elapsed += Time.deltaTime;
            yield return null;
        }

        skeleton.color = Color.gray;
    }


}