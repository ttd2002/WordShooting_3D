using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkBulletImpact : WNetworkBehaviour
{
    [SerializeField] private int ownerIndex;
    [SerializeField] private string ownerNamePlayer;
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected Rigidbody _rigidbody;

    public void Initialize(int playerIndex, string name)
    {
        this.ownerIndex = playerIndex;
        this.ownerNamePlayer = name;
    }

    // protected virtual void OnTriggerEnter(Collider other)
    // {
    //     NetworkBulletSpawner.Instance.Despawn(transform.parent.GetComponent<NetworkObject>());
    //     Transform score = ScorePopUpSpawner.Instance.Spawn(ScorePopUpSpawner.scorePopUp, transform.parent.position, transform.parent.rotation);
    //     score.gameObject.SetActive(true);
    //     int currentScore = scoreManager.PlayerInfos[ownerIndex].PlayerScore;
    //     scoreManager.UpdatePlayerScore(ownerIndex, currentScore + 5);
    //     scoreManager.UpdatePlayerName(ownerIndex, ownerNamePlayer);
    //     if (transform.parent.name == "NetworkBulletFinish")
    //     {
    //         scoreManager.UpdatePlayerScore(ownerIndex, currentScore + 20);
    //         scoreManager.UpdatePlayerName(ownerIndex, ownerNamePlayer);

    //         NetworkMeteoriteDepsawn meteoriteDepsawn = other.transform.parent.parent.GetComponentInChildren<NetworkMeteoriteDepsawn>();
    //         meteoriteDepsawn.SetCanDespawnIsTrue();
    //         Transform scoreBonus = ScorePopUpSpawner.Instance.Spawn(ScorePopUpSpawner.scoreBonusPopUp, transform.parent.position, transform.parent.rotation);
    //         scoreBonus.gameObject.SetActive(true);
    //         NetworkExplosionSpawner.Instance.Spawn(other.transform.position, other.transform.rotation);
    //     }
    // }
    protected virtual void OnTriggerEnter(Collider other)
    {
        NetworkBulletSpawner.Instance.Despawn(transform.parent.GetComponent<NetworkObject>());
        NetworkMonster monster = other.transform.GetComponent<NetworkMonster>();
        int currentScore = NetworkScore.Instance.PlayerInfos[ownerIndex].PlayerScore;
        NetworkScore.Instance.UpdatePlayerScore(ownerIndex, currentScore);
        NetworkScore.Instance.UpdatePlayerName(ownerIndex, ownerNamePlayer);
        monster.GetHit();
        if (transform.parent.name == "NetworkBulletFinish")
        {
            NetworkScore.Instance.UpdatePlayerScore(ownerIndex, currentScore);
            NetworkScore.Instance.UpdatePlayerBonusScore(ownerIndex, currentScore);
            NetworkScore.Instance.UpdatePlayerName(ownerIndex, ownerNamePlayer);
            monster.MonsterDead();
            NetworkExplosionSpawner.Instance.Spawn(other.transform.position, other.transform.rotation);
        }
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadCollider();
        this.LoadRigidbody();
    }
    protected virtual void LoadCollider()
    {
        if (sphereCollider != null) return;
        this.sphereCollider = GetComponent<SphereCollider>();
        this.sphereCollider.isTrigger = false;
        this.sphereCollider.radius = 0.25f;
        Debug.Log(transform.name + ": LoadCollider", gameObject);
    }
    protected virtual void LoadRigidbody()
    {
        if (_rigidbody != null) return;
        this._rigidbody = GetComponent<Rigidbody>();
        this._rigidbody.isKinematic = true;
        Debug.Log(transform.name + ": LoadRigidbody", gameObject);
    }


}