using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public abstract class NetworkMonster : WNetworkBehaviour
{
    [Networked]
    private string textValue { get; set; }
    [SerializeField] protected Animator monsterAnim;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform player;
    [SerializeField] protected GameObject reticle;
    [SerializeField] protected CapsuleCollider monsterCollider;
    [SerializeField] protected TextMeshProUGUI textTarget;
    [SerializeField] protected float distance;
    [SerializeField] private bool hasReachedDestination = false;
    public NavMeshAgent Agent => agent;
    protected override void Start()
    {
        base.Start();
        this.monsterAnim.SetFloat("speed", this.agent.speed);
    }
    public override void FixedUpdateNetwork()
    {
        distance = Vector3.Distance(transform.position, player.position);

        if (distance <= 10f)
        {
            if (!hasReachedDestination)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    hasReachedDestination = true;
                    this.StopAgent();
                    this.monsterAnim.SetFloat("speed", 0f);
                }
                else
                {
                    this.agent.speed = 1.5f;
                    this.monsterAnim.SetFloat("speed", this.agent.speed);
                    agent.SetDestination(player.position);
                }
            }
        }
        if (this.player != null) return;
        this.player = Transform.FindAnyObjectByType<NetworkPlayer>().transform;
        this.hasReachedDestination = false;
        this.agent.speed = 1.5f;
        this.monsterAnim.SetFloat("speed", this.agent.speed);
        agent.SetDestination(player.position);
        this.ResumeAgent();
    }
    public override void Spawned()
    {
        UpdateText();
    }
    public void MonsterDead()
    {
        this.StopAgent();
        this.monsterAnim.SetTrigger("isDeath");
        this.monsterCollider.enabled = false;
        this.textTarget.enabled = false;
        this.reticle.SetActive(false);
        ShootingManager.ResetAllTargets();
        NetworkMonsterSpawner.Instance.OnTextCompleted();

        StartCoroutine(DespawnAfterDelay(2.5f));
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        NetworkMonsterSpawner.Instance.DespawnMonster(transform.GetComponent<NetworkObject>());
    }

    public virtual void GetHit()
    {
    }
    public virtual Vector3 GetHitPos()
    {
        return transform.position;
    }
    public void SetDestination()
    {
        agent.SetDestination(this.player.transform.position);
    }
    
    public void SetRespawnMonster()
    {
        this.hasReachedDestination = false;
        this.monsterCollider.enabled = false;
        this.textTarget.enabled = true;
        this.Agent.speed = 1f;
        this.monsterAnim.SetFloat("speed", 1f);
        this.ResumeAgent();
        this.SetDestination();
    }
    public void Initialize(string text)
    {
        textValue = text;
        UpdateText();
        this.LoadPlayer();
    }

    public void UpdateText()
    {
        if (textTarget != null)
        {
            textTarget.text = textValue;
        }
    }
    public void SetTargetMonster(bool isActive)
    {
        this.monsterCollider.enabled = true;
        this.reticle.SetActive(isActive);
    }

    public void StopAgent()
    {
        agent.isStopped = true;
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    public void ResumeAgent()
    {
        agent.isStopped = false;
        agent.updatePosition = true;
        agent.updateRotation = true;
    }

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadMonsterAnim();
        this.LoadNavMeshAgent();
        this.LoadReticle();
        this.LoadMonsterCollider();
        this.LoadTextTarget();
        this.LoadPlayer();

    }

    protected virtual void LoadMonsterAnim()
    {
        if (this.monsterAnim != null) return;
        this.monsterAnim = transform.GetComponentInChildren<Animator>();
        Debug.Log(transform.name + ": LoadMonsterAnim", gameObject);
    }
    protected virtual void LoadNavMeshAgent()
    {
        if (this.agent != null) return;
        this.agent = transform.GetComponentInChildren<NavMeshAgent>();
        Debug.Log(transform.name + ": LoadNavMeshAgent", gameObject);
    }
    protected virtual void LoadPlayer()
    {
        if (this.player != null) return;
        this.player = Transform.FindAnyObjectByType<NetworkPlayer>().transform;
        // Debug.Log(transform.name + ": LoadPlayer", gameObject);
    }
    protected virtual void LoadReticle()
    {
        if (this.reticle != null) return;
        this.reticle = transform.Find("Canvas/Reticle").gameObject;
        Debug.Log(transform.name + ": LoadReticle", gameObject);
    }
    protected virtual void LoadMonsterCollider()
    {
        if (this.monsterCollider != null) return;
        this.monsterCollider = transform.GetComponent<CapsuleCollider>();
        Debug.Log(transform.name + ": LoadMonsterCollider", gameObject);
    }
    protected virtual void LoadTextTarget()
    {
        if (this.textTarget != null) return;
        this.textTarget = transform.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log(transform.name + ": LoadTextTarget", gameObject);
    }
}
