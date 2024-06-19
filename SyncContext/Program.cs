using System.Threading;

class Program
{
	static async Task Main(string[] args)
	{
		Console.WriteLine($"Main thread ID: {Thread.CurrentThread.ManagedThreadId}");

		// Создаем и устанавливаем кастомный SynchronizationContext
		var customContext = new CustomSynchronizationContext();
		SynchronizationContext.SetSynchronizationContext(customContext);

		// Запускаем асинхронную операцию
		await FetchDataAsync();

		// Ожидаем завершения всех задач в контексте
		customContext.Complete();

		Console.WriteLine("Main thread exiting.");
	}

	static async Task FetchDataAsync()
	{
		Console.WriteLine($"Fetching data started on thread ID: {Thread.CurrentThread.ManagedThreadId}");

		// Имитация длительной операции
		await Task.Delay(2000);

		Console.WriteLine($"Fetching data completed on thread ID: {Thread.CurrentThread.ManagedThreadId}");

		// Используем SynchronizationContext для выполнения кода в нужном потоке
		SynchronizationContext.Current?.Post(state =>
		{
			Console.WriteLine($"Continuation running on thread ID: {Thread.CurrentThread.ManagedThreadId}");
			Console.WriteLine("Data fetched: Sample Data");
		}, null);
	}
}

// Кастомный SynchronizationContext
class CustomSynchronizationContext : SynchronizationContext
{
	private readonly Queue<Tuple<SendOrPostCallback, object>> _items = new Queue<Tuple<SendOrPostCallback, object>>();
	private readonly AutoResetEvent _workItemsWaiting = new AutoResetEvent(false);
	private bool _done;

	public CustomSynchronizationContext()
	{
		new Thread(RunOnCurrentThread) { IsBackground = true }.Start();
	}

	public override void Post(SendOrPostCallback d, object state)
	{
		if (d == null) throw new ArgumentNullException(nameof(d));

		lock (_items)
		{
			_items.Enqueue(Tuple.Create(d, state));
		}
		_workItemsWaiting.Set();
	}

	private void RunOnCurrentThread()
	{
		while (!_done)
		{
			Tuple<SendOrPostCallback, object> task = null;
			lock (_items)
			{
				if (_items.Count > 0)
				{
					task = _items.Dequeue();
				}
			}
			if (task != null)
			{
				task.Item1(task.Item2);
			}
			else
			{
				_workItemsWaiting.WaitOne();
			}
		}
	}

	public void Complete()
	{
		Post(_ => _done = true, null);
		_workItemsWaiting.Set();
	}
}
