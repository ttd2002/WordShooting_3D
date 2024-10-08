using UnityEngine;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
public class NetworkRandomMeteoriteSpawner : WNetworkBehaviour
{
    public float spawnInterval = 2f;
    private Coroutine spawnCoroutine;

    [SerializeField] protected MeteoriteSpawnPoints meteoriteSpawnPoints;
    [SerializeField] protected MeteoriteDespawnPoints meteoriteDespawnPoints;
    [SerializeField] protected TextFetchRandom textFetchRandom;

    public override void Spawned()
    {
        // Chỉ server thực hiện spawn
        if (Runner.IsServer)
        {
            StartCoroutine(this.FetchRandomWordsAndUseThem());
            spawnCoroutine = StartCoroutine(SpawnObjectRoutine());
        }
    }

    private IEnumerator SpawnObjectRoutine()
    {
        yield return new WaitForSeconds(2f);
        while (true)
        {
            SpawnObject();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnObject()
    {
        if (Runner.IsServer)
        {
            Vector3 randomPosition = GetRandomSpawnPosition();

            // Spawn meteorite bằng cách sử dụng Singleton Spawner và không cần playerRef
            NetworkObject meteorite = NetworkMeteoriteSpawner.Instance.Spawn(
                NetworkMeteoriteSpawner.networkMeteorite, randomPosition, Quaternion.identity);

            string randomWord = textFetchRandom.GetRandomWord();
            this.RPC_SetMeteoriteText(meteorite, randomWord);

            // Gọi RPC để thêm vào danh sách chung trên tất cả client
            this.RPC_AddMeteoriteToCommonList(meteorite);
            meteorite.gameObject.SetActive(true);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_SetMeteoriteText(NetworkObject meteorite, string randomWord)
    {
        TextMeshProUGUI tmp = meteorite.gameObject.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            tmp.text = randomWord;
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_AddMeteoriteToCommonList(NetworkObject meteorite)
    {
        NetworkMeteoriteSpawner.Instance.MarkObjectAsActive(meteorite.transform.GetComponent<NetworkObject>());
    }

    private Vector3 GetRandomSpawnPosition()
    {
        return this.meteoriteSpawnPoints.GetRandom().position;
    }

    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
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
        this.LoadMeteoriteSpawnPoint();
        this.LoadMeteoriteDespawnPoint();
        this.LoadTextFetchRandom();
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

    protected virtual void LoadTextFetchRandom()
    {
        if (this.textFetchRandom != null) return;
        this.textFetchRandom = transform.GetComponent<TextFetchRandom>();
        Debug.Log(transform.name + ": LoadTextFetchRandom", gameObject);
    }
}
