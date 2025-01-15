using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingManagment : MonoBehaviour
{
    public static event Action OnResetAllTargets;

    public static void ResetAllTargets()
    {
        if (OnResetAllTargets != null)
        {
            OnResetAllTargets.Invoke();
        }
    }
}
