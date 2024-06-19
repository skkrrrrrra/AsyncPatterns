class Program
{
	static async Task Main(string[] args)
	{
		var task = FetchDataAsync();
		var awaiter = task.GetAwaiter();

		// Проверяем, завершена ли асинхронная операция
		if (awaiter.IsCompleted)
		{
			// Если операция уже завершена, получаем результат синхронно
			string result = awaiter.GetResult();
			Console.WriteLine($"Data fetched: {result}");
		}
		else
		{
			// Если операция не завершена, регистрируем продолжение
			awaiter.OnCompleted(() =>
			{
				try
				{
					string result = awaiter.GetResult();
					Console.WriteLine($"Data fetched: {result}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Exception: {ex.Message}");
				}
			});

			Console.WriteLine("Fetching data asynchronously...");
		}

		await Task.Delay(3000); // Задержка для завершения всех асинхронных операций
	}

	static async Task<string> FetchDataAsync()
	{
		await Task.Delay(2000); // Имитация длительной операции
		return "Sample Data";
	}
}
