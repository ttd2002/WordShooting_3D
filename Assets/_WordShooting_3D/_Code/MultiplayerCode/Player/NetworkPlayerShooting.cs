using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;

public class NetworkPlayerShooting : WNetworkBehaviour
{
    public bool isAutoTyping = false;
    private int currentCharIndex = 0;
    private string currentTarget = "";
    private Coroutine autoTypingCoroutine;
    private TextMeshProUGUI targetTextComponent;
    [SerializeField] private NetworkPlayer networkPlayer;
    [SerializeField] private Animator animator;
    [SerializeField] protected Transform firePoint;
    private void OnEnable()
    {
        ShootingManager.OnResetAllTargets += ResetTarget;
    }

    private void OnDisable()
    {
        ShootingManager.OnResetAllTargets -= ResetTarget;
    }
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
                            this.SpawnBulletObject(true);
                        }
                        else
                        {
                            this.SpawnBulletObject(false);
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

        if (networkPlayer.currentTarget != null)
        {
            var textComponent = networkPlayer.currentTarget.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                this.HighlightTypedText(textComponent);
            }
            else
            {
                Debug.LogWarning("TextMeshProUGUI component not found in current target.");
            }
        }
        else
        {
            Debug.LogWarning("Current target is null in networkGun.");
        }
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

    private void SpawnMuzzle()
    {
        NetworkMuzzleSpawner.Instance.Spawn(firePoint.position, firePoint.rotation);
    }
    private void SpawnBulletObject(bool isFinishBullet)
    {
        if (isFinishBullet)
        {
            NetworkObject bulletObject = NetworkBulletFinishSpawner.Instance.Spawn(this.firePoint.position, Quaternion.identity, (runner, obj) =>
                {
                    var impact = obj.transform.GetComponentInChildren<NetworkBulletImpact>();
                    string playerName = Object.transform.GetComponent<PlayerStats>().playerName.ToString();
                    impact.Initialize(Object.InputAuthority.AsIndex, playerName);
                });
            bulletObject.GetComponent<NetworkBulletFly>().SetTarget(networkPlayer.currentTarget.transform);
            animator.SetTrigger("shoot");
            this.SpawnMuzzle();
        }
        else
        {
            NetworkObject bulletObject = NetworkBulletSpawner.Instance.Spawn(this.firePoint.position, Quaternion.identity, (runner, obj) =>
                {
                    var impact = obj.transform.GetComponentInChildren<NetworkBulletImpact>();
                    string playerName = Object.transform.GetComponent<PlayerStats>().playerName.ToString();
                    impact.Initialize(Object.InputAuthority.AsIndex, playerName);
                });
            bulletObject.GetComponent<NetworkBulletFly>().SetTarget(networkPlayer.currentTarget.transform);
            animator.SetTrigger("shoot");
            this.SpawnMuzzle();
        }
    }

    //Auto typing
    public void StartAutoTyping()
    {
        if (autoTypingCoroutine == null)
        {
            autoTypingCoroutine = StartCoroutine(AutoTypingCoroutine());
        }
    }

    public void StopAutoTyping()
    {
        if (autoTypingCoroutine != null)
        {
            StopCoroutine(autoTypingCoroutine);
            autoTypingCoroutine = null;
        }
    }

    private IEnumerator AutoTypingCoroutine()
    {
        while (isAutoTyping)
        {
            if (this.currentCharIndex < this.currentTarget.Length)
            {
                char nextChar = this.currentTarget[this.currentCharIndex];
                SimulateKeyPress(nextChar);
            }
            float randomDelay = Random.Range(0.1f, 0.5f);
            yield return new WaitForSeconds(randomDelay);
        }
        yield return new WaitForSeconds(0.5f);
    }

    private void SimulateKeyPress(char typedChar)
    {
        NetworkInputData data = new NetworkInputData
        {
            typedChar = typedChar
        };

        data.SetKeyTyped(true);
        if (HasStateAuthority)
        {
            if (this.currentCharIndex < this.currentTarget.Length && typedChar == this.currentTarget[this.currentCharIndex])
            {
                this.currentCharIndex++;
                RPC_UIHighlightTypedText(currentCharIndex, currentTarget);

                if (this.currentCharIndex == this.currentTarget.Length)
                {
                    this.SpawnBulletObject(true);
                    StartCoroutine(WaitAfterFinalCharacter());
                }
                else
                {
                    this.SpawnBulletObject(false);

                }
            }
        }
    }
    private IEnumerator WaitAfterFinalCharacter()
    {
        yield return new WaitForSeconds(2f);
    }


    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadNetworkGun();
    }
    protected virtual void LoadNetworkGun()
    {
        if (this.networkPlayer != null) return;
        this.networkPlayer = transform.parent.GetComponent<NetworkPlayer>();
        Debug.Log(transform.name + ": LoadNetworkGun", gameObject);
    }
}
