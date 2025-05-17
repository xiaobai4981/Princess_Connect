using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class BattleUnit : MonoBehaviour
{
    #region 血条相关
    public GameObject healthBarPrefab;
    public RectTransform healthUIAnchor;
    private Health healthComponent;
    private GameObject healthBarInstance;
    #endregion
    #region 动画名字相关
    private SkeletonGraphic skeleton;
    private Spine.AnimationState spineState;

    // 普通动画
    private string idleAnim;
    private string runAnim;
    private string attackAnim;
    private string damageAnim;
    private string deathAnim;
    private string winAnim;
    // 技能动画
    private string exSkillAnim;
    private string skill1Anim;
    private string skill2Anim;


    #endregion
    // 开始移动相关
    public float spawnOffsetX = 500f; // 生成时的X轴偏移
    public float moveDuration = 2f; // 移动耗时
    private Vector3 targetPosition;

    #region 技能打断控制
    private bool isSkillInterrupting = false;
    private int savedAttackCycleIndex = 0;
    private Coroutine currentAttackCoroutine;
    #endregion

    #region 战斗属性
    public int teamIndex;
    public bool isAlly;
    public bool isAlive => currentHp > 0;
    private int skillIndex;
    public float attackInterval;
    public float attackIntervalVariation = 0.15f;
    #endregion

    #region 游戏数据相关
    private CharacterFactoryTeamCardDataInBattle characterData;
    private MonsterData monsterData;
    public int maxHp;
    public int currentHp;
    public int currentTp;
    private int currentAtk;
    #endregion
    #region 攻击循环控制
    private int attackCycleIndex = 0; // 新增：攻击循环计数器
    private readonly int[] attackSequence = new int[] { 0, 0, 2, 0, 4 }; // 新增：攻击序列配置
    #endregion

    // 事件
    public Action onMoveComplete;

    public void Initialize(bool isAlly, CharacterFactoryTeamCardDataInBattle characterData)
    {
        teamIndex = characterData.characterFactoryTeamCardData.characterId;
        NormalSetupSpine(characterData.characterFactoryTeamCardData.characterData.character_id,
                 characterData.characterFactoryTeamCardData.characterData.current_star);
        // todo 还有技能动画的初始化
        this.isAlly = isAlly;
        this.characterData = characterData;
        this.attackInterval = characterData.characterFactoryTeamCardData.characterData.current_stats.atk_cap / 1000;
        maxHp = characterData.nowHp;
        currentHp = maxHp;
        currentTp = characterData.nowTp;
        currentAtk = characterData.characterFactoryTeamCardData.characterData.current_stats.atk;

        InitializeHealthBar();

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

        maxHp = monsterData.base_stats.hp;
        currentHp = maxHp;
        currentAtk = monsterData.base_stats.atk;

        InitializeHealthBar();

        InitializePosition();
        StartCoroutine(DelayedActivation());
    }

    // 血条初始化
    private void InitializeHealthBar()
    {
        healthComponent = GetComponent<Health>();
        if (healthComponent == null)
        {
            Debug.LogError("Health component not found on " + gameObject.name);
            return;
        }

        healthComponent.UpdateMaxHealth(currentHp);
        if (healthBarPrefab == null || healthUIAnchor == null) return;

        // 生成血条实例
        healthBarInstance = Instantiate(
            healthBarPrefab,
            healthUIAnchor.position,
            Quaternion.identity
        );
        healthBarInstance.transform.SetParent(healthUIAnchor.transform, false);

        // 获取血条控制器并初始化
        var healthBarController = healthBarInstance.GetComponent<HealthBarController>();
        if (healthBarController != null)
        {
            healthBarController.target = healthUIAnchor; // 绑定锚点
            healthBarController.Initialize(
                healthComponent.maxHealth,
                healthComponent.currentHealth
            );
        }

        // 绑定血量变化事件
        healthComponent.OnDamageTaken += UpdateHealthBar;
    }

    // 更新血条数值
    void UpdateHealthBar(int damage)
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.GetComponent<HealthBarController>()
                .Initialize(healthComponent.maxHealth, healthComponent.currentHealth);
        }
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
            exSkillAnim = $"{suffix}_skill0";
            skill1Anim = $"{suffix}_skill1";
            skill2Anim = $"{suffix}_skill2";
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

    public void PauseAnimations()
    {
        skeleton.AnimationState.TimeScale = 0; // Spine动画暂停
    }

    public void ResumeAnimations()
    {
        skeleton.AnimationState.TimeScale = 1; // Spine动画恢复
    }

    public IEnumerator PerformAction(List<BattleUnit> targets)
    {
        // 等待其他技能完成
        yield return new WaitWhile(() => isSkillInterrupting);

        BattleUnit target = targets[targets.Count - 1];
        if (target == null) yield break;
        // todo 播放攻击动画 包括普通攻击和技能攻击
        int currentStep = 0;
        if (characterData != null)
        {
            currentStep = attackSequence[attackCycleIndex % attackSequence.Length];
        }
        string currentAnim = GetAttackAnimation(currentStep);

        spineState.SetAnimation(0, currentAnim, false);
        attackCycleIndex++;
        // 保存当前协程引用
        currentAttackCoroutine = StartCoroutine(AttackProcess(target));
        yield return currentAttackCoroutine;
        
    }
    private IEnumerator AttackProcess(BattleUnit target)
    {
        float animStartTime = Time.time;

        // 等待攻击前摇（带暂停检查）
        while (Time.time - animStartTime < 0.3f)
        {
            if (BattleManager.Instance.isPaused) yield return null;
            else yield return new WaitForEndOfFrame();
        }
        // todo 计算伤害
        currentTp += 50;
        bool isCritical = UnityEngine.Random.value < 0.1f; // 10%暴击率
        int damage = isCritical ? currentAtk * 2 : currentAtk;
        target.TakeDamage(damage, isCritical);

        float postAnimStart = Time.time;
        while (Time.time - postAnimStart < 1f)
        {
            if (BattleManager.Instance.isPaused) yield return null;
            else yield return new WaitForEndOfFrame();
        }
        PlayIdleAnim();
    }

    #region 新增技能触发方法
    public void TriggerSkill2(List<BattleUnit> targets)
    {
        if (!isAlive || isSkillInterrupting) return;

        currentTp = 0;
        // 打断当前攻击
        if (currentAttackCoroutine != null)
        {
            StopCoroutine(currentAttackCoroutine);
            currentAttackCoroutine = null;
        }

        StartCoroutine(Skill2Process(targets));
    }

    private IEnumerator Skill2Process(List<BattleUnit> targets)
    {
        isSkillInterrupting = true;

        // 保存当前攻击序列状态
        savedAttackCycleIndex = attackCycleIndex;

        // 记录技能开始时间
        float skillStartTime = Time.time;

        // 播放技能动画
        var trackEntry = spineState.SetAnimation(1, skill2Anim, false);
        float animDuration = trackEntry.Animation.Duration;

        while (animDuration > 0)
        {
            if (!BattleManager.Instance.isPaused)
                animDuration -= Time.deltaTime;
            yield return null;
        }

        // 执行技能效果
        foreach (var target in targets)
        {
            if (target.isAlive)
                target.TakeDamage(currentAtk * 3);
        }

        // 恢复攻击序列
        attackCycleIndex = savedAttackCycleIndex;
        

        // 关键修改：更新攻击计时器
        float skillDuration = Time.time - skillStartTime;
        BattleManager.Instance.UpdateAttackTimer(this, skillDuration);

        // 继续攻击循环
        PlayIdleAnim();
        isSkillInterrupting = false;
    }

    public bool IsCastingSkill()
    {
        return isSkillInterrupting;
    }
    #endregion

    private string GetAttackAnimation(int step)
    {
        return step switch
        {
            2 => exSkillAnim,
            4 => skill1Anim,
            _ => attackAnim
        };
    }

    public void TakeDamage(int amount, bool isCritical = false)
    {
        currentHp = Mathf.Max(0, currentHp - amount);
        healthComponent.TakeDamage(amount);

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
    void OnDestroy()
    {
        // 销毁血条实例
        if (healthBarInstance != null) Destroy(healthBarInstance);

        // 解绑事件
        healthComponent.OnDamageTaken -= UpdateHealthBar;
    }

}