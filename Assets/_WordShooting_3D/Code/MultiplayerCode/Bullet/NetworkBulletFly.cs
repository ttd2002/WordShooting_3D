using Fusion;
using UnityEngine;

public class NetworkBulletFly : WNetworkBehaviour
{

    private Transform target;
    [SerializeField] private float speed = 100f;
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }
    public override void FixedUpdateNetwork()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Runner.DeltaTime;
        }
        else
        {
            Runner.Despawn(transform.GetComponent<NetworkObject>());
            return;
        }
    }
}
