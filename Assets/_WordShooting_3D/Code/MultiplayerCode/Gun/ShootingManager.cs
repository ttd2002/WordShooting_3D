using System;

public static class ShootingManager
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
