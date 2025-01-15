using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CyberWeaponImpact : MonoBehaviour
{
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask playerLayer;

    public void DealDamage()
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
