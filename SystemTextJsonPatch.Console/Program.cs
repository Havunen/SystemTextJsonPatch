namespace SystemTextJsonPatch.Console;

public class Program
{

	public static void Main(string[] args)
	{
		for (int i = 0; i < 10000; i++)
		{
			DeserializeTest.Run();
		}
	}
}