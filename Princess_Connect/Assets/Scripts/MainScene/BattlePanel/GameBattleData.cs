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

    public StageData stageData;
    public List<int> arenaPlayerTeam;
    public string arenaPlayerName;
    public bool isArena;
}
