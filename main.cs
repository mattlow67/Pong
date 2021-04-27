using System;
using System.Windows.Forms;

public class MainClass
{
	public static void Main()
	{
		Console.WriteLine("Pong has started.");
		Frame pong = new Frame();
		Application.Run(pong);
		Console.WriteLine("The program has terminated.\nYou may close the window.");
	}
}
