using System;
using System.IO;
using System.Text;

class Program
{
	private readonly static string _filePath = "C:\\Users\\Ilya\\Desktop\\progging\\csharp\\Async\\Async\\testfile.txt";

	static void Main(string[] args)
	{
		BeginReadFileAsync(_filePath);
		Console.WriteLine("Пожалуйста, ожидайте, идет чтение файла...");
		Console.ReadLine(); // Чтобы окно консоли не закрылось сразу
	}

	static void BeginReadFileAsync(string filePath)
	{
		FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
		byte[] buffer = new byte[fs.Length];
		AsyncCallback callback = new AsyncCallback(EndReadFileAsync);
		fs.BeginRead(buffer, 0, buffer.Length, callback, new State(fs, buffer));
	}

	static void EndReadFileAsync(IAsyncResult ar)
	{
		State state = (State)ar.AsyncState;
		FileStream fs = state.FileStream;
		byte[] buffer = state.Buffer;

		int bytesRead = fs.EndRead(ar);
		if (bytesRead > 0)
		{
			Console.WriteLine("Считано байт: " + bytesRead);
			Console.WriteLine("Содержимое файла:");
			Console.WriteLine(Encoding.UTF8.GetString(buffer, 0, bytesRead));
		}
		fs.Close();
	}

	class State
	{
		public FileStream FileStream { get; private set; }
		public byte[] Buffer { get; private set; }

		public State(FileStream fs, byte[] buffer)
		{
			FileStream = fs;
			Buffer = buffer;
		}
	}
}
