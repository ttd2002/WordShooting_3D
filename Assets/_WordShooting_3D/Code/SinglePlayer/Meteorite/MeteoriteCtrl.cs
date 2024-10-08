using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteCtrl : SingletonAbstract<MeteoriteCtrl>
{
    [SerializeField] protected MeteoriteRandom meteoriteRandom;
    public MeteoriteRandom MeteoriteRandom => meteoriteRandom;
    [SerializeField] protected TextFetchRandom textFetchRandom;
    public TextFetchRandom TextFetchRandom => textFetchRandom;
    [SerializeField] protected MeteoriteSpawnPoints meteoriteSpawnPoints;
    public MeteoriteSpawnPoints MeteoriteSpawnPoints => meteoriteSpawnPoints;
    [SerializeField] protected MeteoriteDespawnPoints meteoriteDespawnPoints;
    public MeteoriteDespawnPoints MeteoriteDespawnPoints => meteoriteDespawnPoints;    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(this.FetchRandomWordsAndUseThem());
    }

    void Update()
    {
        this.meteoriteRandom.MeteoriteSpawning(this.textFetchRandom.GetRandomWord(), this.meteoriteSpawnPoints.GetRandom().position, meteoriteDespawnPoints.GetRandom());
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
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadMeteoriteRandom();
        this.LoadTextFetchRandom();
        this.LoadMeteoriteSpawnPoint();
        this.LoadMeteoriteDespawnPoint();
    }

    protected virtual void LoadMeteoriteRandom()
    {
        if (this.meteoriteRandom != null) return;
        this.meteoriteRandom = transform.GetComponent<MeteoriteRandom>();
        Debug.Log(transform.name + ": LoadMeteoriteRandom", gameObject);
    }
    protected virtual void LoadTextFetchRandom()
    {
        if (this.textFetchRandom != null) return;
        this.textFetchRandom = transform.GetComponent<TextFetchRandom>();
        Debug.Log(transform.name + ": LoadTextFetchRandom", gameObject);
    }
    protected virtual void LoadMeteoriteSpawnPoint()
    {
        if (this.meteoriteSpawnPoints != null) return;
        this.meteoriteSpawnPoints = FindObjectOfType<MeteoriteSpawnPoints>();
        Debug.Log(transform.name + ": LoadMeteoriteSpawnPoint", gameObject);
    }
    protected virtual void LoadMeteoriteDespawnPoint()
    {
        if (this.meteoriteDespawnPoints != null) return;
        this.meteoriteDespawnPoints = FindObjectOfType<MeteoriteDespawnPoints>();
        Debug.Log(transform.name + ": LoadMeteoriteDespawnPoint", gameObject);
    }
}
