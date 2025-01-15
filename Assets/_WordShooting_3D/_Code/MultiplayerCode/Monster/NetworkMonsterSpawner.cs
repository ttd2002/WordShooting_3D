using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkMonsterSpawner : SingletonNetworkSpawner<NetworkMonsterSpawner>
{
    private float timer;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private TextFetchRandom _textFetchRandom;
    [SerializeField] private MeteoriteSpawnPoints _meteoriteSpawnPoints;
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
        if (_textQueue.Count >= 6) return;
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
        int randomChoice = Random.Range(0, 3);

        NetworkObject textObject;

        switch (randomChoice)
        {
            case 0:
                textObject = this.Spawn(randomPosition, Quaternion.identity);
                break;
            case 1:
                textObject = NetworkCyberSpawner.Instance.Spawn(randomPosition, Quaternion.identity);
                break;
            case 2:
                textObject = NetworkDrakonitSpawner.Instance.Spawn(randomPosition, Quaternion.identity);
                break;
            default:
                throw new System.Exception("Unexpected random choice value: " + randomChoice);
        }


        if (textObject != null)
        {
            string randomWord = _textFetchRandom.GetRandomWord();
            NetworkMonster networkMonster = textObject.GetComponent<NetworkMonster>();
            networkMonster.Initialize(randomWord);
            networkMonster.SetDestination();
            this.RPC_SetTextForObject(textObject, randomWord);
            this._textQueue.Enqueue(textObject);

            if (this._textQueue.Count == 1)
            {
                this.currentTarget = textObject;
                networkMonster.SetTargetMonster(true);
            }
        }
    }
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_SetTextForObject(NetworkObject textObject, string text)
    {
        NetworkMonster textNetworkObject = textObject.GetComponent<NetworkMonster>();
        if (textObject != null && textObject.IsValid)
        {
            textNetworkObject.Initialize(text);
            // textNetworkObject.SetDestination();
        }
    }

    public void OnTextCompleted()
    {
        if (Runner.IsServer && _textQueue.Count > 0)
        {
            _textQueue.Dequeue();
            // TextNetworkObject completedNetworkObject = completedObject.GetComponent<TextNetworkObject>();
            // completedNetworkObject.SetActiveReticle(false);

            if (_textQueue.Count > 0)
            {
                this.currentTarget = this._textQueue.Peek();
                NetworkMonster newNetworkObject = this.currentTarget.GetComponent<NetworkMonster>();
                newNetworkObject.SetTargetMonster(true);
                this.RPC_SetActiveReticle(this.currentTarget);
            }

        }

    }

    public void DespawnMonster(NetworkObject monster)
    {
        if (Runner.IsServer)
        {
            this.Despawn(monster);
        }
    }
    [Rpc(RpcSources.StateAuthority,RpcTargets.All)]
    public void RPC_SetActiveReticle(NetworkObject textObject)
    {
        NetworkMonster textNetworkObject = textObject.GetComponent<NetworkMonster>();
        if (textNetworkObject != null)
        {
            textNetworkObject.SetTargetMonster(true);
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
        this.LoadTextFetchRandom();
    }
    protected virtual void LoadMeteoriteSpawnPoint()
    {
        if (this._meteoriteSpawnPoints != null) return;
        this._meteoriteSpawnPoints = FindObjectOfType<MeteoriteSpawnPoints>();
        Debug.Log(transform.name + ": LoadMeteoriteSpawnPoint", gameObject);
    }

    protected virtual void LoadTextFetchRandom()
    {
        if (this._textFetchRandom != null) return;
        this._textFetchRandom = transform.GetComponent<TextFetchRandom>();
        Debug.Log(transform.name + ": LoadTextFetchRandom", gameObject);
    }

}
