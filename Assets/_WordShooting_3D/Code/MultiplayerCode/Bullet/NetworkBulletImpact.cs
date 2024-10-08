using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public class NetworkBulletImpact : WNetworkBehaviour
{
    [SerializeField] protected SphereCollider sphereCollider;
    [SerializeField] protected Rigidbody _rigidbody;

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
    protected virtual void OnTriggerEnter(Collider other)
    {
        NetworkBulletSpawner.Instance.Despawn(transform.parent.GetComponent<NetworkObject>());
        Transform vfx_Impact = VFXSpawner.Instance.Spawn(VFXSpawner.muzzle, other.transform.position, other.transform.rotation);
        vfx_Impact.gameObject.SetActive(true);
        Transform score = ScorePopUpSpawner.Instance.Spawn(ScorePopUpSpawner.scorePopUp, transform.parent.position, transform.parent.rotation);
        score.gameObject.SetActive(true);
        UIGameplayManager.Instance.networkScore.AddScore();
        if (transform.parent.name == "NetworkBulletFinish")
        {
            UIGameplayManager.Instance.networkScore.AddBonusScore();

            Transform reticle = other.transform.parent.parent.Find("Canvas/Reticle");
            reticle.gameObject.SetActive(false);

            NetworkMeteoriteDepsawn meteoriteDepsawn = other.transform.parent.parent.GetComponentInChildren<NetworkMeteoriteDepsawn>();
            meteoriteDepsawn.SetCanDespawnIsTrue();
            Transform scoreBonus = ScorePopUpSpawner.Instance.Spawn(ScorePopUpSpawner.scoreBonusPopUp, transform.parent.position, transform.parent.rotation);
            scoreBonus.gameObject.SetActive(true);
            Transform vfx_Explosion = VFXSpawner.Instance.Spawn(VFXSpawner.explosion, other.transform.position, other.transform.rotation);
            vfx_Explosion.gameObject.SetActive(true);
        }
    }

}