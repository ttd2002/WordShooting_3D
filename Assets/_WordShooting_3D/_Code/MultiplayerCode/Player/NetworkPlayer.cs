using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NetworkPlayer : WNetworkBehaviour
{
    private Vector3 direction;
    private Quaternion rotation;
    private int playerHealth = 20;
    private int playerMaxHealth = 20;
    private bool reachPos;
    [SerializeField] private GameObject model;
    [SerializeField] private NetworkPlayerShooting networkPlayerShooting;
    [SerializeField] private Animator animator;
    [SerializeField] private UIBarScript HPBarScript;
    [SerializeField] public NetworkObject currentTarget;
    [SerializeField] public bool isDeath;
    public Animator PlayerAnimator => animator;

    protected override void Start()
    {
        this.animator.SetFloat("speed", 1.5f);
        this.reachPos = false;
        this.HPBarScript = Transform.FindAnyObjectByType<UIBarScript>();
        this.HPBarScript.UpdateValue(playerHealth, playerMaxHealth);

    }
    public override void FixedUpdateNetwork()
    {
        if (isDeath) return;
        if (transform.position.z >= 1f)
        {
            this.animator.SetFloat("speed", 0);
            this.animator.SetBool("isAiming", true);
            this.reachPos = true;
            this.RPC_CheckReachPos();
        }
        if (!reachPos) return;
        currentTarget = NetworkMonsterSpawner.Instance.currentTarget;
        this.RPC_SetCurrentTarget(currentTarget);
        this.GetTargetToShoot(currentTarget);
        this.networkPlayerShooting.CheckKeyInput(currentTarget);
        if (HasInputAuthority && Input.GetKeyDown(KeyCode.F1))
        {
            RPC_ToggleAutoTyping();
        }
    }
    public void TakeDamage(int HP)
    {
        this.playerHealth -= HP;
        this.HPBarScript.UpdateValue(this.playerHealth, this.playerMaxHealth);
        if (playerHealth <= 0)
        {
            this.IsDeath();
            this.RPC_PlayerDead();
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ToggleAutoTyping()
    {
        networkPlayerShooting.isAutoTyping = !networkPlayerShooting.isAutoTyping;

        if (networkPlayerShooting.isAutoTyping)
        {
            networkPlayerShooting.StartAutoTyping();
        }
        else
        {
            networkPlayerShooting.StopAutoTyping();
        }
    }
    [Rpc]
    public void RPC_SetCurrentTarget(NetworkObject target)
    {
        this.currentTarget = target;
        this.GetTargetToShoot(target);

    }
    [Rpc]
    public void RPC_PlayerDead()
    {
        this.IsDeath();
    }
    [Rpc]
    public void RPC_CheckReachPos()
    {
        this.animator.SetFloat("speed", 0);
        this.animator.SetBool("isAiming", true);
        this.reachPos = true;
    }
    private void GetTargetToShoot(NetworkObject targetObject)
    {
        if (targetObject == null) return;
        this.LookTarget(targetObject.transform);
    }
    public virtual void LookTarget(Transform targetObject)
    {
        this.direction = targetObject.position - model.transform.position;
        this.rotation = Quaternion.LookRotation(direction);
        // Monster monster = targetObject.GetComponent<Monster>();
        // monster.SetTargetMonster(true);
        this.model.transform.rotation = Quaternion.Lerp(this.model.transform.rotation, this.rotation, Time.deltaTime * 1f);
    }
    public void IsDeath()
    {
        this.isDeath = true;
        this.animator.SetTrigger("isDeath");
    }
}
