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
        var data = battleInitData.enemyTeam[index];
        if (isArena)
        {

        }
        else
        {
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

    private IEnumerator BattleLoop()
    {
        while (isBattleActive)
        {
            yield return ExecuteTurn();
            yield return null; // 每帧至少等待一帧
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
        yield return StartCoroutine(attacker.PerformAction(targets));
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
                break;

            case BattleResult.Lose:
                // 敌方存活单位保持idle
                foreach (var enemy in enemies)
                {
                    if (enemy.isAlive) enemy.PlayIdleAnim();
                }
                MusicMgr.Instance.PlayBKMusic("AttackLose");
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
                break;
        }
        Debug.Log($"Battle Ended: {result}");
    }
    private void OnDestroy()
    {
        if (characterPrefab) Destroy(characterPrefab);
        if (monsterPrefab) Destroy(monsterPrefab);
    }
    public enum BattleResult { Win, Lose, Timeout }
}
