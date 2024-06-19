public class Program
{
	private readonly static string _filePath = "C:\\Users\\Ilya\\Desktop\\progging\\csharp\\Async\\Async\\testfile.txt";

	public async static Task Main(string[] args)
	{
		var data = string.Empty;

		//data = ReadFromFile(_filePath);
		data = await ReadFromFileAsync(_filePath);
		//data = await ReadFromFileVoidAsync(_filePath);
	}

	public async static Task<string> ReadFromFileAsync(string filePath)
	{
		using (var reader = new StreamReader(filePath))
		{
			await Task.Delay(3000);
			Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");
			return await reader.ReadToEndAsync();
		}
	}


	public async static void ReadFromFileVoidAsync(string filePath)
	{
		using (var reader = new StreamReader(filePath))
		{
			await Task.Delay(3000);
			Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");
			Console.WriteLine(await reader.ReadToEndAsync());
		}
	}

	public static string ReadFromFile(string filePath)
	{
		using (var reader = new StreamReader(filePath))
		{
			Console.WriteLine($"Thread Id: {Thread.CurrentThread.ManagedThreadId}");
			return reader.ReadToEnd();
		}
	}
}

