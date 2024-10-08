using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MeteoriteRandom : WMonoBehaviour
{
    [SerializeField] protected float spawnDelay = 2f;
    [SerializeField] protected float spawnTimer = 2f;
    
    public void MeteoriteSpawning(string randomText, Vector3 randomPosition, Transform newTarget)
    {
        this.spawnTimer += Time.fixedDeltaTime;
        if (this.spawnTimer < this.spawnDelay) return;
        this.spawnTimer = 0;
        Transform meteorite = MeteoriteSpawner.Instance.Spawn(MeteoriteSpawner.meteorite, randomPosition, Quaternion.identity);
        meteorite.transform.localScale = new Vector3(1, 1, 1);

        TextMeshProUGUI tmp = meteorite.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = randomText;

        MeteoriteFly meteoriteFly = meteorite.GetComponentInChildren<MeteoriteFly>();
        meteoriteFly.SetNewTarget(newTarget);
        

        meteorite.gameObject.SetActive(true);
    }
}
