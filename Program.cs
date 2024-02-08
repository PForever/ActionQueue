// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

await ActionQueueTest();
// await AsyncActionQueueTest();

static async Task ActionQueueTest()
{
    var action = new ActionQueue();
    int count = 0;
    var rnd = new Random();
    var tasks = new List<Task>(100);
    foreach (var i in Enumerable.Range(0, 100))
    {
        tasks.Add(OnExit());
    }

    await Task.WhenAll(tasks);

    async Task OnExit()
    {
        await Task.Delay(rnd.Next(100, 300));
        action.PushAndDoAll(() => Console.WriteLine($"{count++} -- {Environment.CurrentManagedThreadId}"));
    }
}
static async Task AsyncActionQueueTest()
{
    var action = new AsyncActionQueue();
    int count = 0;
    var rnd = new Random();
    var tasks = new List<Task>(100);
    foreach (var i in Enumerable.Range(0, 100))
    {
        tasks.Add(OnExit());
    }

    await Task.WhenAll(tasks);

    async Task OnExit()
    {
        await Task.Delay(rnd.Next(100, 300));
        await action.PushAndDoAllAsync(DoAsync);
         async Task DoAsync(CancellationToken cancellationToken) {
            await Task.Delay(0, cancellationToken);
            Console.WriteLine($"{count++} -- {Environment.CurrentManagedThreadId}");
        }
    }
}
