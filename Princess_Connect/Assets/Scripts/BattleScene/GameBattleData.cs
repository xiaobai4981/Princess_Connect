using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameBattleData 
{
    public static BattleInitData battleInitData;
}
public class BattleInitData
{
    public List<CharacterFactoryTeamCardDataInBattle> playerTeam;
    public List<MonsterData> enemyTeam;
    public List<CharacterFactoryTeamCardDataInBattle> arenaEnemyTeam;
    public StageData stageData;
    public bool isArena;
}
