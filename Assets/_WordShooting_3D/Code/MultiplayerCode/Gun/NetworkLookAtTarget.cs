using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkLookAtTarget : WNetworkBehaviour
{
    private Vector3 direction;
    private Quaternion rotation;
    [SerializeField] protected GameObject model;
    [SerializeField] protected Transform mainCamera;
    [SerializeField] protected List<Transform> textObjects;
    [SerializeField] private NetworkGun networkGun;

    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadModel();
        this.LoadMainCamera();
        this.LoadNetworkGun();

    }
    public virtual void LookTarget(Transform targetObject)
    {
        this.direction = targetObject.position - model.transform.position;
        this.rotation = Quaternion.LookRotation(direction);

        this.model.transform.localRotation = Quaternion.Lerp(model.transform.rotation, this.rotation, 1);
    }
    protected virtual void LoadModel()
    {
        if (this.model != null) return;
        this.model = transform.parent.Find("Model").Find("TopSide").gameObject;
        Debug.Log(transform.name + ": LoadModel", gameObject);
    }
    protected virtual void LoadMainCamera()
    {
        if (this.mainCamera != null) return;
        this.mainCamera = Transform.FindObjectOfType<Camera>().transform;
        Debug.Log(transform.name + ": LoadMainCamera", gameObject);
    }
    protected virtual void LoadNetworkGun()
    {
        if (this.networkGun != null) return;
        this.networkGun = transform.parent.GetComponent<NetworkGun>();
        Debug.Log(transform.name + ": LoadNetworkGun", gameObject);
    }
}
