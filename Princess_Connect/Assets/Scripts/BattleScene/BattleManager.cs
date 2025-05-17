using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    // 战斗初始化数据
    private BattleInitData battleInitData;

    // 角色预制体父节点
    private GameObject characterPrefab;
    private GameObject monsterPrefab;
    public Vector3[] allyPositions; // 友方单位生成位置
    public Vector3[] enemyPositions; // 敌方单位生成位置
    
    // 战斗时间控制
    public float battleDuration = 90f; // 战斗总时长(秒)
    public float battleTimer;
    public  bool isBattleActive;
    private int unitsMovingCount;

    public List<BattleUnit> allies = new List<BattleUnit>();
    public List<BattleUnit> enemies = new List<BattleUnit>();

    private Dictionary<BattleUnit, float> nextAttackTimes = new Dictionary<BattleUnit, float>();

    // 用于停止战斗
    public bool isPaused;
    private float pausedTimeRemaining; // 暂停时剩余的战斗时间
    private Dictionary<BattleUnit, Coroutine> pausedCoroutines = new Dictionary<BattleUnit, Coroutine>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        var allGameObjects = new List<GameObject>();
        allGameObjects.AddRange(FindObjectsOfType<GameObject>());
        Transform parent = null;
        foreach (var item in allGameObjects)
        {
            if (item.name == "Top")
            {
                parent = item.transform;
                break;
            }
        }
        characterPrefab = new GameObject("CharacterPrefab");
        monsterPrefab = new GameObject("MonsterPrefab");
        characterPrefab.transform.SetParent(parent);
        characterPrefab.transform.localPosition = Vector3.zero;
        monsterPrefab.transform.SetParent(parent);
        monsterPrefab.transform.localPosition = Vector3.zero;
        // 直接从静态类获取数据
        if (GameBattleData.battleInitData == null)
        {
            Debug.LogError("BattleInitData未设置！");
            return;
        }
        battleInitData = GameBattleData.battleInitData;
    }
    private void Start()
    {
        InitializeBattle();
        StartCoroutine(StartBattleSequence());
    }
    private IEnumerator StartBattleSequence()
    {
        yield return new WaitUntil(() => unitsMovingCount == 0);
        //yield return BattleStartUIController.Instance.ShowBattleStart();
        foreach (var unit in allies.Concat(enemies))
        {
            nextAttackTimes[unit] = Time.time;
        }
        isBattleActive = true;

        StartCoroutine(BattleLoop());
    }

    private void Update()
    {
        if (isBattleActive)
        {
            battleTimer += Time.deltaTime;

            // 检查战斗是否超时
            if (battleTimer >= battleDuration)
            {
                EndBattle(BattleResult.Timeout);
            }
        }
    }

    // 初始化战斗
    private void InitializeBattle()
    {
        // 从游戏数据加载战斗单位信息
        // 普通模式
        // 生成友方单位
        for (int i = 0; i < battleInitData.playerTeam.Count; i++)
        {
            CreateAlly(i);
        }
        if (battleInitData.isArena == false)
        {
            // 生成敌方单位
            for (int i = 0; i < battleInitData.enemyTeam.Count; i++)
            {
                CreateEnemy(i, battleInitData.isArena);
            }
        }
        // 竞技场模式
        else
        {
            // 生成敌方单位
            for (int i = 0; i < battleInitData.arenaEnemyTeam.Count; i++)
            {
                CreateEnemy(i, battleInitData.isArena);
            }
        }
    }

    private void CreateAlly(int index)
    {
        var data = battleInitData.playerTeam[index];
        string resName = GetCharacterPrefabName(data);
        ABResMgr.Instance.LoadResAsync<GameObject>("character_prefab", resName, (obj) =>
        {
            GameObject characterInstance = Instantiate(obj);
            characterInstance.transform.SetParent(characterPrefab.transform);
            characterInstance.transform.localPosition = allyPositions[index];
            BattleUnit unit = characterInstance.GetComponent<BattleUnit>();
            unit.onMoveComplete += () => unitsMovingCount--;
            unitsMovingCount++;
            unit.Initialize(true, data);
            allies.Add(unit);
        }, true);
    }

    private string GetCharacterPrefabName(CharacterFactoryTeamCardDataInBattle data)
    {
        int charId = data.characterFactoryTeamCardData.characterId;
        if (charId == 1053 || charId == 1057)
        {
            return data.characterFactoryTeamCardData.characterData.current_star == 6 ?
                $"{charId}61" : charId.ToString();
        }
        return charId.ToString();
    }

    private void CreateEnemy(int index, bool isArena)
    {
        
        if (isArena)
        {
            var arenaDatga = battleInitData.arenaEnemyTeam[index];
            string resName = GetCharacterPrefabName(arenaDatga);
            ABResMgr.Instance.LoadResAsync<GameObject>("character_prefab", resName, (obj) =>
            {
                GameObject characterInstance = Instantiate(obj);
                characterInstance.transform.SetParent(monsterPrefab.transform);
                characterInstance.transform.localPosition = enemyPositions[index];
                characterInstance.transform.localScale = new Vector3(-0.7f, 0.7f, 1);
                // 其他组件保持正常方向
                Transform uiRoot = characterInstance.transform.Find("HeadUIAnchor");
                if (uiRoot != null)
                {
                    uiRoot.localScale = new Vector3(-0.7f, 0.7f, 1);
                }
                BattleUnit unit = characterInstance.GetComponent<BattleUnit>();
                unit.onMoveComplete += () => unitsMovingCount--;
                unitsMovingCount++;
                unit.Initialize(false, arenaDatga);
                enemies.Add(unit);
            }, true);
        }
        else
        {
            var data = battleInitData.enemyTeam[index];
            ABResMgr.Instance.LoadResAsync<GameObject>("monster_prefab", battleInitData.enemyTeam[index].monster_id.ToString(), (obj) =>
            {
                GameObject characterInstance = Instantiate(obj);
                characterInstance.transform.SetParent(monsterPrefab.transform);
                characterInstance.transform.localPosition = enemyPositions[index];
                BattleUnit unit = characterInstance.GetComponent<BattleUnit>();
                unit.onMoveComplete += () => unitsMovingCount--;
                unitsMovingCount++;
                unit.InitializeMonster(false, data);
                enemies.Add(unit);
            }, true);
        }
       
    }


    public void RemoveUnit(BattleUnit unit)
    {
        if (unit.isAlly) allies.Remove(unit);
        else enemies.Remove(unit);
        nextAttackTimes.Remove(unit);
    }
    #region 战斗暂停有关
    public void PauseBattle()
    {
        if (!isBattleActive) return;

        isPaused = true;
        isBattleActive = false;
        pausedTimeRemaining = battleDuration - battleTimer;

        // 暂停所有单位的动画
        foreach (var unit in GetAllUnits())
        {
            if (unit.isAlive)
            {
                unit.PauseAnimations();
                // 暂停正在进行的攻击协程
                pausedCoroutines[unit] = StartCoroutine(PauseUnitAction(unit));
            }
        }

        Time.timeScale = 0; // 暂停游戏时间
    }

    public void ResumeBattle()
    {
        if (!isPaused) return;

        isPaused = false;
        isBattleActive = true;
        battleTimer = battleDuration - pausedTimeRemaining;

        Time.timeScale = 1; // 恢复游戏时间

        // 恢复所有单位的动画
        foreach (var unit in GetAllUnits())
        {
            if (unit.isAlive)
            {
                unit.ResumeAnimations();
                // 恢复协程
                if (pausedCoroutines.TryGetValue(unit, out var coroutine))
                {
                    StopCoroutine(coroutine);
                    StartCoroutine(ResumeUnitAction(unit));
                }
            }
        }
        pausedCoroutines.Clear();

        // 重新启动战斗循环
        StartCoroutine(BattleLoop());
    }

    private IEnumerator PauseUnitAction(BattleUnit unit)
    {
        yield return new WaitWhile(() => isPaused); // 等待暂停结束
    }

    private IEnumerator ResumeUnitAction(BattleUnit unit)
    {
        yield return null; // 等待一帧确保状态恢复
        if (unit.isAlive)
        {
            unit.PlayIdleAnim(); // 重置到待机状态
            nextAttackTimes[unit] = Time.time + unit.GetActualAttackInterval();
        }
    }

    public void UpdateAttackTimer(BattleUnit unit, float elapsedTime)
    {
        if (nextAttackTimes.ContainsKey(unit))
        {
            float realElapsed = elapsedTime * Time.timeScale;
            nextAttackTimes[unit] = Mathf.Max(
                Time.time,
                nextAttackTimes[unit] + realElapsed
            );
        }
    }


    private List<BattleUnit> GetAllUnits()
    {
        return allies.Concat(enemies).ToList();
    }
    #endregion
    private IEnumerator BattleLoop()
    {
        while (isBattleActive)
        {
            yield return ExecuteTurn();
            yield return null;
            CheckBattleEnd();
        }
    }


    private IEnumerator ExecuteTurn()
    {
        List<Coroutine> animations = new List<Coroutine>();
        float currentTime = Time.time;

        // 友方行动
        foreach (var ally in allies.Where(u => u.isAlive))
        {
            if (currentTime >= nextAttackTimes[ally])
            {
                animations.Add(StartCoroutine(ExecuteAttack(ally, enemies)));
                nextAttackTimes[ally] = currentTime + ally.GetActualAttackInterval();
            }
        }

        // 敌方行动
        foreach (var enemy in enemies.Where(u => u.isAlive))
        {
            if (currentTime >= nextAttackTimes[enemy])
            {
                animations.Add(StartCoroutine(ExecuteAttack(enemy, allies)));
                nextAttackTimes[enemy] = currentTime + enemy.GetActualAttackInterval();
            }
        }

        foreach (var anim in animations) yield return anim;
    }
    private IEnumerator ExecuteAttack(BattleUnit attacker, List<BattleUnit> targets)
    {
        // 添加状态检查
        if (attacker.IsCastingSkill()) yield break;

        yield return StartCoroutine(attacker.PerformAction(targets));

        // 强制刷新攻击计时器
        if (nextAttackTimes.ContainsKey(attacker))
        {
            nextAttackTimes[attacker] = Time.time + attacker.GetActualAttackInterval();
        }
    }


    private void CheckBattleEnd()
    {
        if (allies.TrueForAll(u => !u.isAlive)) EndBattle(BattleResult.Lose);
        if (enemies.TrueForAll(u => !u.isAlive)) EndBattle(BattleResult.Win);
    }

    private void EndBattle(BattleResult result)
    {
        isBattleActive = false;
        StopAllCoroutines();
        switch (result)
        {
            case BattleResult.Win:
                // 友方存活单位做出win动画
                foreach (var ally in allies)
                {
                    if (ally.isAlive) ally.PlayWinAnim();
                }
                MusicMgr.Instance.PlayBKMusic("AttackWin");
                UIMgr.Instance.ShowPanel<BattleResultPanel>(E_UILayer.System, (panel) => {
                    panel.UpdatePlayerName(GameBattleData.nowPlayerName);
                    panel.InputResult(1, allies.Count, enemies.Count);
                    if (!GameBattleData.battleInitData.isArena)
                    {
                        panel.UpdateStageNum(GameBattleData.battleInitData.stageId);
                    }
                });
                break;

            case BattleResult.Lose:
                // 敌方存活单位保持idle
                foreach (var enemy in enemies)
                {
                    if (enemy.isAlive) enemy.PlayIdleAnim();
                }
                MusicMgr.Instance.PlayBKMusic("AttackLose");
                UIMgr.Instance.ShowPanel<BattleResultPanel>(E_UILayer.System, (panel) => {
                    panel.UpdatePlayerName(GameBattleData.nowPlayerName);
                    panel.InputResult(0, allies.Count, enemies.Count);
                    if (!GameBattleData.battleInitData.isArena)
                    {
                        panel.UpdateStageNum(GameBattleData.battleInitData.stageId);
                    }
                });
                break;

            case BattleResult.Timeout:
                // 双方存活单位都保持idle
                foreach (var ally in allies)
                {
                    if (ally.isAlive) ally.PlayIdleAnim();
                }
                foreach (var enemy in enemies)
                {
                    if (enemy.isAlive) enemy.PlayIdleAnim();
                }
                UIMgr.Instance.ShowPanel<BattleResultPanel>(E_UILayer.System, (panel) => {
                    panel.UpdatePlayerName(GameBattleData.nowPlayerName);
                    panel.InputResult(2, allies.Count, enemies.Count);
                    if (!GameBattleData.battleInitData.isArena)
                    {
                        panel.UpdateStageNum(GameBattleData.battleInitData.stageId);
                    }
                });
                break;
        }
        Debug.Log($"Battle Ended: {result}");
    }

    public void OnSkill2ButtonClicked(int allyIndex)
    {
        if (allies.Count > allyIndex && allies[allyIndex].isAlive)
        {
            List<BattleUnit> targets = enemies.Where(e => e.isAlive).ToList();
            allies[allyIndex].TriggerSkill2(targets);
        }
    }
    private void OnDestroy()
    {
        if (characterPrefab) Destroy(characterPrefab);
        if (monsterPrefab) Destroy(monsterPrefab);
    }
    public enum BattleResult { Win, Lose, Timeout }
}
