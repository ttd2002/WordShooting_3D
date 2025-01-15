using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrakonitRHandImpact : MonoBehaviour
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
                playerController.TakeDamage(1);
            }
            else
            {
                // Debug.Log("PlayerController not found!");
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
