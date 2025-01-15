using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public abstract class Monster : WMonoBehaviour
{
    [SerializeField] protected Animator monsterAnim;
    [SerializeField] protected NavMeshAgent agent;
    [SerializeField] protected Transform player;
    [SerializeField] protected GameObject reticle;
    [SerializeField] protected CapsuleCollider monsterCollider;
    [SerializeField] protected TextMeshProUGUI textTarget;
    [SerializeField] protected float distance;
    [SerializeField] private bool hasReachedDestination = false;
    public Animator MonsterAnim => monsterAnim;
    public NavMeshAgent Agent => agent;
    public GameObject Reticle => reticle;

    protected override void Start()
    {
        base.Start();
        this.monsterAnim.SetFloat("speed", this.agent.speed);
    }
    void Update()
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


    }
    public void MonsterDead()
    {
        this.StopAgent();
        this.monsterAnim.SetTrigger("isDeath");
        this.monsterCollider.enabled = false;
        this.textTarget.enabled = false;
        ShootingManagment.ResetAllTargets();
        MonsterSpawner.Instance.DequeueNextMonster();
        this.reticle.SetActive(false);

        StartCoroutine(DespawnAfterDelay(2.5f));
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        MonsterSpawner.Instance.Despawn(transform);
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

    public void SetRespawnMonster(string randomWord)
    {
        this.hasReachedDestination = false;
        this.monsterCollider.enabled = false;
        this.textTarget.enabled = true;
        this.textTarget.text = randomWord;
        this.Agent.speed = 1f;
        this.monsterAnim.SetFloat("speed", 1f);
        this.ResumeAgent();
        this.SetDestination();
    }
    public void SetTargetMonster(bool isActive)
    {
        this.monsterCollider.enabled = isActive;
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
        this.LoadPlayer();
        this.LoadReticle();
        this.LoadMonsterCollider();
        this.LoadTextTarget();
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
        this.player = Transform.FindAnyObjectByType<PlayerController>().transform;
        Debug.Log(transform.name + ": LoadPlayer", gameObject);
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
