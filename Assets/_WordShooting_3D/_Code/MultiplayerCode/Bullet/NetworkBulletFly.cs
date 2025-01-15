using Fusion;
using UnityEngine;

public class NetworkBulletFly : WNetworkBehaviour
{
    [SerializeField] private float speed = 100f;

    private Transform target;
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }
    public override void FixedUpdateNetwork()
    {
        if (target != null)
        {
            Vector3 direction = (target.GetComponent<NetworkMonster>().GetHitPos() - transform.position).normalized;
            transform.position += direction * speed * Runner.DeltaTime;
        }
        else
        {
            Runner.Despawn(transform.GetComponent<NetworkObject>());
            return;
        }
    }
}
