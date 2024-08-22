using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LookAtTarget : GunAbstract
{

    private Vector3 direction;
    private Quaternion rotation;
    [SerializeField] protected GameObject model;
    [SerializeField] protected Transform mainCamera;
    [SerializeField] protected List<Transform> textObjects;


    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadModel();
        this.LoadMainCamera();
    }
    public virtual void LookTarget(Transform targetObject)
    {
        this.direction = targetObject.position - model.transform.position;
        this.rotation = Quaternion.LookRotation(direction);

        // this.mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.rotation, this.rotation, 1);
        this.model.transform.localRotation = Quaternion.Lerp(model.transform.rotation, this.rotation, 1);
    }
    public virtual void FillTargetObjects()
    {
        foreach (Transform textObject in MeteoriteSpawner.Instance.GetHolder())
        {
            if (textObject.gameObject.activeSelf && !textObjects.Contains(textObject))
            {
                this.textObjects.Add(textObject);
            }
        }
    }

    public virtual void RemoveTargetObject()
    {
        List<Transform> objectsToRemove = new List<Transform>();

        foreach (Transform textObject in textObjects)
        {
            if (!textObject.gameObject.activeSelf)
            {
                objectsToRemove.Add(textObject);
            }
        }

        foreach (Transform textObject in objectsToRemove)
        {
            textObjects.Remove(textObject);
        }
    }

    public virtual Transform GetFirstTargetObject()
    {
        if (textObjects.Count <= 0) return null;
        return textObjects[0];

    }
    public virtual void SwitchToNextTarget()
    {
        if (textObjects.Count <= 1) return;
        Transform firstTarget = textObjects[0];
        Transform reticle = firstTarget.Find("Canvas/Reticle");
        reticle.gameObject.SetActive(false);
        textObjects.RemoveAt(0);
        textObjects.Add(firstTarget);
        Transform newTarget = textObjects[0];
        this.gunCtrl.Shooting.ResetTarget();
        this.gunCtrl.LookAtTarget.LookTarget(newTarget);
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
}
