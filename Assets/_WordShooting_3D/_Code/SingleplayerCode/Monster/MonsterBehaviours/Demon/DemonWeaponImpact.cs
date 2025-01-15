using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonWeaponImpact : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask playerLayer;

    public void Purch2()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

        foreach (var player in hitPlayers)
        {
            if (player.TryGetComponent(out PlayerController playerController))
            {
                // playerController.IsDeath();
                playerController.TakeDamage(2);
            }
            else
            {
                if (player.TryGetComponent(out NetworkPlayer networkPlayer))
                {
                    networkPlayer.TakeDamage(2);
                }
            }
        }
    }
    public void Purch3()
    {
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);

        foreach (var player in hitPlayers)
        {
            if (player.TryGetComponent(out PlayerController playerController))
            {
                // playerController.IsDeath();
                playerController.TakeDamage(3);
            }
            else
            {
                if (player.TryGetComponent(out NetworkPlayer networkPlayer))
                {
                    networkPlayer.TakeDamage(3);
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
