using UnityEngine;
using TMPro;
using Fusion;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Pool;

public class NetworkMeteoriteSpawner : SingletonNetworkSpawner<NetworkMeteoriteSpawner>
{


    private float timer;
    private bool spawnStarted = false;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private TextFetchRandom _textFetchRandom;
    [SerializeField] private MeteoriteSpawnPoints _meteoriteSpawnPoints;
    [SerializeField] private MeteoriteDespawnPoints _meteoriteDespawnPoints;
    [SerializeField] private Queue<NetworkObject> _textQueue = new Queue<NetworkObject>();

    [Networked]
    [OnChangedRender(nameof(OnTargetChanged))]
    public NetworkObject currentTarget { get; set; }
    protected override void Awake()
    {
        StartCoroutine(this.FetchRandomWordsAndUseThem());
    }
    public override void FixedUpdateNetwork()
    {
        if (NetworkTimer.Instance.GetIsPaused()) return;
        if (Runner.IsServer)
        {
            this.timer += Runner.DeltaTime;
            if (timer >= _spawnInterval)
            {
                this.timer = 0f;
                SpawnTextObject();
            }
        }
    }
    private void SpawnTextObject()
    {
        Vector3 randomPosition = GetRandomSpawnPosition();
        NetworkObject textObject = this.Spawn(randomPosition, Quaternion.identity);
        if (textObject != null)
        {
            string randomWord = _textFetchRandom.GetRandomWord();
            TextNetworkObject networkObject = textObject.GetComponent<TextNetworkObject>();
            networkObject.Initialize(randomWord);
            this.RPC_SetTextForObject(textObject, randomWord);
            this._textQueue.Enqueue(textObject);

            if (this._textQueue.Count == 1)
            {
                this.currentTarget = textObject;
                networkObject.SetActiveReticle(true);
            }
        }
    }
    [Rpc]
    public void RPC_SetTextForObject(NetworkObject textObject, string text)
    {
        TextNetworkObject textNetworkObject = textObject.GetComponent<TextNetworkObject>();
        if (textNetworkObject != null)
        {
            textNetworkObject.Initialize(text);
        }
    }

    public void OnTextCompleted()
    {
        if (Runner.IsServer && _textQueue.Count > 0)
        {
            NetworkObject completedObject = _textQueue.Dequeue();
            TextNetworkObject completedNetworkObject = completedObject.GetComponent<TextNetworkObject>();
            completedNetworkObject.SetActiveReticle(false);
            this.Despawn(completedObject);

            if (_textQueue.Count > 0)
            {
                this.currentTarget = this._textQueue.Peek();
                TextNetworkObject newNetworkObject = this.currentTarget.GetComponent<TextNetworkObject>();
                newNetworkObject.SetActiveReticle(true);
                this.RPC_SetActiveReticle(this.currentTarget);
            }

        }

    }
    [Rpc]
    public void RPC_SetActiveReticle(NetworkObject textObject)
    {
        TextNetworkObject textNetworkObject = textObject.GetComponent<TextNetworkObject>();
        if (textNetworkObject != null)
        {
            textNetworkObject.SetActiveReticle(true);
        }
    }
    public void OnTargetChanged()
    {
        if (this.currentTarget != null)
        {
            this.currentTarget = this.currentTarget;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return this._meteoriteSpawnPoints.GetRandom().position;
    }

    private IEnumerator FetchRandomWordsAndUseThem()
    {
        yield return this._textFetchRandom.FetchRandomWordsAndParagraphsOnce(OnRandomWordsReceived, OnParagraphsReceived, 100);
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
        this.LoadMeteoriteSpawnPoint();
        this.LoadMeteoriteDespawnPoint();
        this.LoadTextFetchRandom();
    }
    protected virtual void LoadMeteoriteSpawnPoint()
    {
        if (this._meteoriteSpawnPoints != null) return;
        this._meteoriteSpawnPoints = FindObjectOfType<MeteoriteSpawnPoints>();
        Debug.Log(transform.name + ": LoadMeteoriteSpawnPoint", gameObject);
    }

    protected virtual void LoadMeteoriteDespawnPoint()
    {
        if (this._meteoriteDespawnPoints != null) return;
        this._meteoriteDespawnPoints = FindObjectOfType<MeteoriteDespawnPoints>();
        Debug.Log(transform.name + ": LoadMeteoriteDespawnPoint", gameObject);
    }

    protected virtual void LoadTextFetchRandom()
    {
        if (this._textFetchRandom != null) return;
        this._textFetchRandom = transform.GetComponent<TextFetchRandom>();
        Debug.Log(transform.name + ": LoadTextFetchRandom", gameObject);
    }
}
