using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkShooting : WNetworkBehaviour
{
    // [Networked, OnChangedRender(nameof(ColorChanged))]
    // public int state { get; set; }
    // void ColorChanged()
    // {
    //     Debug.Log("Aaaaaaaaaaaaaaaaa: " + state);
    //     SpawnBulletRPC();
    //     if (state == 100)
    //     {
    //         SpawnBulletFinishRPC();
    //     }
    // }
    private void OnEnable()
    {
        ShootingManager.OnResetAllTargets += ResetTarget;
    }

    private void OnDisable()
    {
        ShootingManager.OnResetAllTargets -= ResetTarget;
    }
    private int currentCharIndex = 0;
    private string currentTarget = "";
    [SerializeField] private TextMeshProUGUI targetTextComponent;
    [SerializeField] private NetworkGun networkGun;

    [SerializeField] NetworkObject bullet;
    [SerializeField] NetworkObject bulletFinish;
    public virtual void CheckKeyInput(NetworkObject targetTextTransform)
    {
        if (Time.timeScale > 0f && targetTextTransform != null)
        {
            if (this.targetTextComponent == null)
            {
                this.targetTextComponent = targetTextTransform.GetComponentInChildren<TextMeshProUGUI>();
                if (this.targetTextComponent != null)
                {
                    this.currentTarget = this.targetTextComponent.text.ToLower();
                    this.currentCharIndex = 0;
                }
            }
            SpawnBullet();

        }
    }
    public void SpawnBullet()
    {
        if (GetInput(out NetworkInputData data))
        {
            if (HasStateAuthority)
            {

                if (data.buttons.IsSet(NetworkInputData.KEY_TYPED))
                {
                    char typedChar = data.typedChar;
                    if (this.currentCharIndex < this.currentTarget.Length && typedChar == this.currentTarget[this.currentCharIndex])
                    {
                        this.currentCharIndex++;
                        RPC_UIHighlightTypedText(currentCharIndex, currentTarget);
                        if (this.currentCharIndex == this.currentTarget.Length)
                        {
                            NetworkObject bulletObject = Runner.Spawn(bulletFinish, this.networkGun.FirePoint.position, Quaternion.identity);
                            bulletObject.name = "NetworkBulletFinish";
                            bulletObject.GetComponent<NetworkBulletFly>().SetTarget(networkGun.currentTarget.transform);
                        }
                        else
                        {
                            NetworkObject bulletObject = Runner.Spawn(bullet, this.networkGun.FirePoint.position, Quaternion.identity);
                            bulletObject.GetComponent<NetworkBulletFly>().SetTarget(networkGun.currentTarget.transform);
                        }
                    }
                }
            }

        }
    }
    [Rpc(RpcSources.StateAuthority, targets: RpcTargets.InputAuthority)]
    public void RPC_UIHighlightTypedText(int currentCharIndex, string currentTarget)
    {
        this.currentCharIndex = currentCharIndex;
        this.currentTarget = currentTarget;
        this.HighlightTypedText(networkGun.currentTarget.GetComponentInChildren<TextMeshProUGUI>());
    }
    public virtual void HighlightTypedText(TextMeshProUGUI targetTextComponent)
    {
        targetTextComponent.text = "<color=green>" + this.currentTarget.Substring(0, this.currentCharIndex) + "</color>" + this.currentTarget.Substring(this.currentCharIndex);
    }
    public virtual void ResetTarget()
    {
        this.currentCharIndex = 0;
        this.targetTextComponent = null;
        this.currentTarget = "";
    }
    public void ShootingAtTarget(Transform targetTextComponent)
    {
        this.SpawnBullet(NetworkBulletSpawner.bullet, targetTextComponent);
        this.SpawnMuzzle();
    }
    public void FinishText(Transform targetTextComponent)
    {
        this.SpawnMuzzle();
        this.SpawnBullet(NetworkBulletFinishSpawner.bulletFinish, targetTextComponent);
        // this.networkGun.NetworkLookAtTarget.SwitchToNextTarget();
    }
    private void SpawnMuzzle()
    {
        Transform vfx_Muzzle = VFXSpawner.Instance.Spawn(VFXSpawner.muzzle, this.networkGun.FirePoint.position, this.networkGun.FirePoint.rotation);
        vfx_Muzzle.gameObject.SetActive(true);
    }
    private void SpawnBullet(string namePrefab, Transform targetTextComponent)
    {
        NetworkObject bullet = NetworkBulletSpawner.Instance.Spawn(namePrefab, this.networkGun.FirePoint.position, this.networkGun.FirePoint.rotation);
        if (namePrefab == NetworkBulletFinishSpawner.bulletFinish)
        {
            bullet = NetworkBulletFinishSpawner.Instance.Spawn(namePrefab, this.networkGun.FirePoint.position, this.networkGun.FirePoint.rotation);
        }
        if (bullet == null) return;
        NetworkBulletFly bulletFly = bullet.GetComponentInChildren<NetworkBulletFly>();
        bulletFly.SetTarget(targetTextComponent);

        bullet.gameObject.SetActive(true);
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadNetworkGun();
    }
    protected virtual void LoadNetworkGun()
    {
        if (this.networkGun != null) return;
        this.networkGun = transform.parent.GetComponent<NetworkGun>();
        Debug.Log(transform.name + ": LoadNetworkGun", gameObject);
    }
}
