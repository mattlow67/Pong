using System;
using System.Windows.Forms;

namespace ButtonTypes
{
   public class NoFocusButton : Button
   {   
	public NoFocusButton ()
        {SetStyle(ControlStyles.Selectable,false);}
   }
}

public class Logic
{
	private System.Random randomGenerator = new System.Random();
	
	public double getRandomAngle()
	{
		//double randomNum = randomGenerator.NextDouble ();
		//randomNum = randomNum - 0.5;
		//randomGenerator.NextDouble() * (maximum - minimum) + minimum;
		//generates randomNum between -.45 and .45
		// there is a bug when the angle equals or is close to either 
		// 90 or -90 degrees. The ball continues travelling forever
		// in the direction.
		int randomType = randomGenerator.Next(1,5); 
		double randomNum = 0.0;

		switch (randomType) {
			case 1: randomNum = randomGenerator.NextDouble() * (.35 - .05) + .05;
			break;
			case 2: randomNum = randomGenerator.NextDouble() * (.85 - .55) + .55;
			break;
			case 3: randomNum = randomGenerator.NextDouble() * (-.55 - (-.85)) + (-.85);
			break;
			case 4: randomNum = randomGenerator.NextDouble() * (-.05 - (-.35)) + (-.35);
			break;

		}
		double angleRadians = System.Math.PI * randomNum;

		return angleRadians;
	}

}
