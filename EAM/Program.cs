using System.Text;
using System.ComponentModel; // Для использования AsyncOperation

class Program
{
	private readonly static string _filePath = "C:\\Users\\Ilya\\Desktop\\progging\\csharp\\Async\\Async\\testfile.txt";

	static void Main(string[] args)
	{
		AsyncFileReader reader = new AsyncFileReader();
		reader.ReadCompleted += (sender, e) =>
		{
			if (e.Error != null)
			{
				Console.WriteLine("Ошибка при чтении файла: " + e.Error.Message);
			}
			else
			{
				Console.WriteLine("Содержимое файла:");
				Console.WriteLine(e.FileContent);
			}
		};

		reader.ReadFile(_filePath); // Укажите путь к файлу
		Console.WriteLine("Чтение файла началось...");
		Console.ReadLine();
	}
}

public class AsyncFileReader
{
	public event EventHandler<ReadCompletedEventArgs> ReadCompleted;

	public void ReadFile(string filePath)
	{
		AsyncOperation asyncOperation = AsyncOperationManager.CreateOperation(null);
		FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
		byte[] buffer = new byte[fs.Length];

		fs.BeginRead(buffer, 0, buffer.Length, iar =>
		{
			int bytesRead = fs.EndRead(iar);
			fs.Close();

			string fileContent = Encoding.UTF8.GetString(buffer, 0, bytesRead);
			ReadCompletedEventArgs args = new ReadCompletedEventArgs(fileContent, null);
			asyncOperation.PostOperationCompleted(e => OnReadCompleted((ReadCompletedEventArgs)e), args);
		}, null);
	}

	protected virtual void OnReadCompleted(ReadCompletedEventArgs e)
	{
		ReadCompleted?.Invoke(this, e);
	}
}

public class ReadCompletedEventArgs : AsyncCompletedEventArgs
{
	public string FileContent { get; }

	public ReadCompletedEventArgs(string fileContent, Exception error) : base(error, false, null)
	{
		FileContent = fileContent;
	}
}
