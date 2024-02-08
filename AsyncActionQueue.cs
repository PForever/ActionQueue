using System.Collections.Concurrent;

public class AsyncActionQueue : IDisposable
{
    private readonly ConcurrentQueue<Func<CancellationToken, Task>> actionQueue = new();
    private readonly SemaphoreSlim locker = new(1);

    public async Task PushAndDoAllAsync(Func<CancellationToken, Task> action, CancellationToken cancellationToken = default){
        actionQueue.Enqueue(action);
        while (!actionQueue.IsEmpty)
        {
            try
            {
                
                if (!await locker.WaitAsync(0)) return;
                while (actionQueue.TryDequeue(out var executableAction))
                {
                    await executableAction(cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                }
                locker.Release();
            }
            catch
            {
                locker.Release();
                throw;
            }
        }
    }

    public void Dispose()
    {
        locker.Dispose();
    }
}