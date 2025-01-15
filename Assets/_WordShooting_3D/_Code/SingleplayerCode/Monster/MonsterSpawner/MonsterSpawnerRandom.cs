using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MonsterSpawnerRandom : MonoBehaviour
{
    [SerializeField] protected MonsterSpawnerCtrl monsterSpawnerCtrl;

    [SerializeField] protected float spawnDelay = 4f;
    [SerializeField] protected float spawnTimer = 2f;
    [SerializeField] protected float spawnLimit = 6f;

    public void MonsterSpawning(string randomText, Vector3 randomPosition)
    {
        if (this.RandomReachLimit()) return;

        this.spawnTimer += Time.fixedDeltaTime;
        if (this.spawnTimer < this.spawnDelay) return;
        this.spawnTimer = 0;

        Transform prefab = MonsterSpawner.Instance.GetRandomPrefab();
        Transform monster = MonsterSpawner.Instance.Spawn(prefab, randomPosition, Quaternion.identity);
        MonsterSpawner.Instance.EnqueueMonster(monster.gameObject);

        Monster monsterScript = monster.GetComponent<Monster>();
        monster.gameObject.SetActive(true);
        if (monsterScript != null) monsterScript.SetRespawnMonster(randomText);


    }


    private bool RandomReachLimit()
    {
        int currentMonster = this.monsterSpawnerCtrl.MonsterSpawner.SpawnedCount;
        return currentMonster >= this.spawnLimit;
    }
}
