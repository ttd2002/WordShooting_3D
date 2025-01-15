using UnityEngine;

public class BillboardText : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;


    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }
}
