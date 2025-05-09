using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    private Transform characterPrefab;
    private Transform monsterPrefab;
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
        GameObject obj1 = new GameObject("CharacterPrefab");
        GameObject obj2 = new GameObject("MonsterPrefab");
        characterPrefab = Instantiate(obj1, parent, false).transform;
        monsterPrefab = Instantiate(obj2, parent, false).transform;

    }
    private void Start()
    {
        // 直接从静态类获取数据
        if (GameBattleData.battleInitData == null)
        {
            Debug.LogError("BattleInitData未设置！");
            return;
        }

    }

    private void OnDestroy()
    {
        Destroy(characterPrefab.gameObject);
        Destroy(monsterPrefab.gameObject);
    }

}
