using System.Collections.Generic;
public static class MainThreadInvoker
{
    private static readonly Queue<System.Action> actions = new Queue<System.Action>();
    public static void Update()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue().Invoke();
        }
    }
    public static void RunOnMainThread(System.Action action)
    {
        lock (actions)
        {
            actions.Enqueue(action);
        }
    }
}
