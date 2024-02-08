using System.Collections.Concurrent;

public class ActionQueue
{
    private readonly ConcurrentQueue<Action> actionQueue = new();
    private readonly object _lock = new();

    public void PushAndDoAll(Action action){
        actionQueue.Enqueue(action);
        while (!actionQueue.IsEmpty)
        {
            try
            {
                if (!Monitor.TryEnter(_lock)) return;
                while (actionQueue.TryDequeue(out var executableAction))
                    executableAction();
                Monitor.Exit(_lock);
            }
            catch
            {
                Monitor.Exit(_lock);
                throw;
            }
        }
    }
}