using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : WMonoBehaviour
{
    private Vector3 direction;
    private Quaternion rotation;
    private int playerHealth = 20;
    private int playerMaxHealth = 20;
    [SerializeField] private UIBarScript HPBarScript;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject targetObject;
    [SerializeField] private PlayerShooting playerShooting;
    [SerializeField] private Animator animator;
    [SerializeField] public bool isDeath;
    public Animator PlayerAnimator => animator;


    protected override void Start()
    {
        this.animator.SetFloat("speed", 1.5f);
        this.HPBarScript.UpdateValue(playerHealth, playerMaxHealth);

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) TimeController.Instance.PauseGame();
        if (Input.GetKeyDown(KeyCode.Tab)) this.SwitchToNextTarget();
        if (isDeath) return;
        if (transform.position.z >= 1f)
        {
            this.animator.SetFloat("speed", 0);
            this.animator.SetBool("isAiming", true);
        }
        if (MonsterSpawner.Instance.PeekFirstMonster() == null) return;
        targetObject = MonsterSpawner.Instance.PeekFirstMonster();
        LookTarget(targetObject.transform);
        playerShooting.CheckKeyInput(targetObject.transform);
    }
    public void TakeDamage(int HP)
    {
        this.playerHealth -= HP;
        this.HPBarScript.UpdateValue(this.playerHealth, this.playerMaxHealth);
        if (playerHealth <= 0)
        {
            this.IsDeath();
            StartCoroutine(EndGameAfterDelay(5f));
        }
    }
    private IEnumerator EndGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TimeController.Instance.EndGame();
    }
    private void SwitchToNextTarget()
    {
        targetObject.GetComponent<Monster>().SetTargetMonster(false);
        MonsterSpawner.Instance.DequeueNextMonster();
        MonsterSpawner.Instance.EnqueueMonster(targetObject);
        targetObject = MonsterSpawner.Instance.PeekFirstMonster();
        LookTarget(targetObject.transform);
        playerShooting.ResetTarget();
        playerShooting.CheckKeyInput(targetObject.transform);
    }
    public virtual void LookTarget(Transform targetObject)
    {
        this.direction = targetObject.position - model.transform.position;
        this.rotation = Quaternion.LookRotation(direction);
        Monster monster = targetObject.GetComponent<Monster>();
        monster.SetTargetMonster(true);
        this.model.transform.rotation = Quaternion.Lerp(this.model.transform.rotation, this.rotation, Time.deltaTime * 1f);
    }
    public void IsDeath()
    {
        this.isDeath = true;
        this.animator.SetTrigger("isDeath");
    }
    public void OK()
    {
        Debug.Log("OK");
    }
}
