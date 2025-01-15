using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnerCtrl : WMonoBehaviour
{
    [SerializeField] protected MonsterSpawnerRandom monsterSpawnerRandom;
    [SerializeField] protected TextFetchRandom textFetchRandom;
    [SerializeField] protected MeteoriteSpawnPoints meteoriteSpawnPoints;
    [SerializeField] protected MonsterSpawner monsterSpawner;
    public MonsterSpawner MonsterSpawner => monsterSpawner;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(this.FetchRandomWordsAndUseThem());
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        this.monsterSpawnerRandom.MonsterSpawning(this.textFetchRandom.GetRandomWord(), this.meteoriteSpawnPoints.GetRandom().position);
        // this.textRandom.TextSpawning(this.textFetchRandom.GetRandomWord());
    }
    private IEnumerator FetchRandomWordsAndUseThem()
    {
        yield return this.textFetchRandom.FetchRandomWordsAndParagraphsOnce(OnRandomWordsReceived, OnParagraphsReceived, 100);
    }

    private void OnRandomWordsReceived(string[] words)
    {
        if (words == null) return;
        Debug.Log("Fetched " + words.Length + " random words.");
    }

    private void OnParagraphsReceived(List<string> paragraphs)
    {
        if (paragraphs == null) return;
        Debug.Log("Fetched " + paragraphs.Count + " paragraphs.");
    }

}
