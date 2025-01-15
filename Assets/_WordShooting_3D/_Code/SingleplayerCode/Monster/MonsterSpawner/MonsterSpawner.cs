using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : SingletonSpawner<MonsterSpawner>
{
    public static string demon_damaged = "Demon_damaged";
    public static string demon_default = "Demon_default";
    public static string cyber_Monsters = "Cyber_Monsters";
    public static string drakonit_monster_lod = "Drakonit_monster_lod";

    [SerializeField] private List<GameObject> _listMonster;
    public List<GameObject> ListMonster => _listMonster;

    [SerializeField] private Queue<GameObject> monsterQueue = new Queue<GameObject>();

    // protected override void Start()
    // {
    //     foreach (var monster in _listMonster)
    //     {
    //         monsterQueue.Enqueue(monster);
    //     }
    // }
    public void EnqueueMonster(GameObject monster)
    {
        monsterQueue.Enqueue(monster);
    }
    public GameObject PeekFirstMonster()
    {

        if (monsterQueue.Count <= 0) return null;
        return monsterQueue.Peek();

    }
    public GameObject DequeueNextMonster()
    {
        if (monsterQueue.Count <= 0) return null;
        return monsterQueue.Dequeue();
    }
}
