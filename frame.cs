//comment
//change resolution

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Diagnostics;
using ButtonTypes;

public class Frame : Form
{
	// frame dimensions
	private const int frameheight = 576;
	private const int framewidth = 1024;
	private const int uiheight = frameheight/4;
	private const int totalheight = frameheight + uiheight;	
	private const int buttonwidth = 80;
	private const int buttonheight = 30;
	//button declarations
	private NoFocusButton newGameButton = new NoFocusButton();
	private NoFocusButton goButton = new NoFocusButton();
	private NoFocusButton exitButton = new NoFocusButton();
	private Label playerLeftLabel = new Label();
	private Label playerRightLabel = new Label();
	private Label speedLabel = new Label();
	private TextBox speedTextBox = new TextBox();
	private Label go = new Label();
	private Point golocation = new Point((int)framewidth/2-80,(int)frameheight/2-30);	
	//button locations
	private Point locationOfNewGameButton = new Point((int)((framewidth*1/6)-(buttonwidth/2)),
					  	        (int)(totalheight - ((uiheight/2))));
	private Point locationOfGoButton = new Point((int)((framewidth/2)-(buttonwidth/2)),
					  	        (int)(totalheight - ((uiheight/2))));
	private Point locationOfExitButton = new Point((int)((framewidth*5/6)-(buttonwidth/2)),
					  	        (int)(totalheight - ((uiheight/2))));
	private Point locationOfPlayerLeftLabel = new Point((int)((framewidth*1/6)-(buttonwidth/2)),
					  	        (int)(totalheight - ((uiheight/2)+45)));
	private Point locationOfPlayerRightLabel = new Point((int)((framewidth*5/6)-(buttonwidth/2)),
					  	        (int)(totalheight - ((uiheight/2)+45)));
	private Point locationOfSpeedLabel = new Point((int)((framewidth*1/2)-(buttonwidth/2)),
					  	        (int)(totalheight - ((uiheight/2)+65)));
	private Point locationOfSpeedTextBox = new Point((int)((framewidth*1/2)-(25)),
					  	        (int)(totalheight - ((uiheight/2)+45)));
	
	//paddle variables
	private const int paddleWidth = 20;
	private const int paddleHeight = frameheight/5;
	private int leftPaddleX;
	private int leftPaddleY;
	private int rightPaddleX;
	private int rightPaddleY;
	private int rightVerticalDelta = 0;
	private int leftVerticalDelta = 0;
	enum keyPosition {up,down};
	private keyPosition upKey = keyPosition.up;
	private keyPosition downKey = keyPosition.up;
	private keyPosition wKey = keyPosition.up;
	private keyPosition sKey = keyPosition.up;
	//ball variables
	private const int ballRadius = (int)(.02*frameheight);	//12
	private double ballRealX = (double)(framewidth/2 - ballRadius);
	private double ballRealY = (double)(frameheight/2 - ballRadius);
	private int ballIntX;
	private int ballIntY;
	private double ballVerticalDelta;
	private double ballHorizontalDelta;
	private double angleRadians;	
	private double ballDistanceRefresh = 10.9;	//const? 10.9
	private double ballUpdateRate = 35.5;		//const? 35.5
	//adjustments
	private const int horizontalAdjustment = 8;
	private const int verticalAdjustment = 2*ballRadius;
	//clocks
	private const double graphicRefreshRate = 60.0;
	private static System.Timers.Timer graphicRefreshClock = new System.Timers.Timer();
	private static System.Timers.Timer ballControlClock = new System.Timers.Timer();
	private bool ballClockActive = false;
	private Stopwatch stopwatch;
	//other 
	private Logic algorithms;
	private int leftScore = 0;
	private int rightScore = 0;
	private int totalScore = 0;

	public Frame() 
	{
		//frame
		Text = "Pong by Matthew Low";
		Console.WriteLine("formwidth = {0}, formheight = {1}", framewidth, frameheight);
		Size = new Size(framewidth,totalheight);
		BackColor = Color.Black;
		Console.WriteLine("Enter a speed (1-100) and press \"Go\". 50 is recommended.");
		Console.WriteLine("Left player uses W and S keys.");
		Console.WriteLine("Right player uses up and down keys.");

		//ball
		// initial coordinates
		algorithms = new Logic();
		ballIntX = (int)(ballRealX);
		ballIntY = (int)(ballRealY);
		angleRadians = algorithms.getRandomAngle();

		//paddles
		// right
		rightPaddleX = framewidth - 40;
		rightPaddleY = frameheight/2 - paddleHeight/2;
		KeyPreview = true;
		KeyUp += new KeyEventHandler(OnKeyUp);
		// left
		leftPaddleX = 20;
		leftPaddleY = frameheight/2 - paddleHeight/2;
		KeyPreview = true;
		KeyUp += new KeyEventHandler(OnKeyUp);

		//clocks
		graphicRefreshClock.Enabled = false;
		graphicRefreshClock.Elapsed += new ElapsedEventHandler (UpdateDisplay);
		ballControlClock.Enabled = false;
		ballControlClock.Elapsed += new ElapsedEventHandler (UpdateBall);		

		//buttons
		newGameButton.Text = "New Game";
		newGameButton.Size = new Size (buttonwidth,buttonheight);
		newGameButton.Location = locationOfNewGameButton;
		newGameButton.BackColor = Color.LightGray;
		newGameButton.Font = new Font("Arial",12,FontStyle.Regular);
		newGameButton.Click += new EventHandler (ResetGame);
		Controls.Add(newGameButton);
		goButton.Text = "Go";
		goButton.Size = new Size (buttonwidth,buttonheight);
		goButton.Font = new Font("Arial",12,FontStyle.Regular);
		goButton.Location = locationOfGoButton;
		goButton.BackColor = Color.LightGray;
		Controls.Add(goButton);
		goButton.Click += new EventHandler (StartOrStop);
		exitButton.Text = "Exit";
		exitButton.Size = new Size (buttonwidth,buttonheight);
		exitButton.Font = new Font("Arial",12,FontStyle.Regular);
		exitButton.Location = locationOfExitButton;
		exitButton.BackColor = Color.LightGray;
		Controls.Add(exitButton);
		exitButton.Click += new EventHandler (QuitProgram);
		//labels
		playerLeftLabel.Text = "Left Player\n        0";
		playerLeftLabel.Location = locationOfPlayerLeftLabel;
		playerLeftLabel.BackColor = Color.LightGray;
		playerLeftLabel.Font = new Font("Arial",12,FontStyle.Regular);
		playerLeftLabel.AutoSize = true;
		Controls.Add(playerLeftLabel);
		playerRightLabel.Text = "Right Player\n         0";
		playerRightLabel.Location = locationOfPlayerRightLabel;
		playerRightLabel.Font = new Font("Arial",12,FontStyle.Regular);
		playerRightLabel.BackColor = Color.LightGray;
		playerRightLabel.AutoSize = true;
		Controls.Add(playerRightLabel);
		speedLabel.Text = "     Speed";
		speedLabel.Location = locationOfSpeedLabel;
		speedLabel.Font = new Font("Arial",12,FontStyle.Regular);
		speedLabel.BackColor = Color.LightGray;
		speedLabel.AutoSize = true;
		Controls.Add(speedLabel);
		speedTextBox.Text = "50";
		speedTextBox.Width = 50;
		speedTextBox.Font = new Font("Arial",12,FontStyle.Regular);
		speedTextBox.Location = locationOfSpeedTextBox;
		speedTextBox.AutoSize = true;
		Controls.Add(speedTextBox);
		go.ForeColor = Color.White;
		go.Font = new Font("Arial", 24,FontStyle.Bold);
		go.Location = golocation;
		go.BackColor = Color.Black;
		go.Text = "";
		go.AutoSize = true;		
		Controls.Add(go);

		//smooth animation
		DoubleBuffered = true;
	}

	protected override void OnPaint(PaintEventArgs ee)
	{
		Graphics graph = ee.Graphics;
		//draw ball
		graph.FillEllipse(Brushes.White, ballIntX, ballIntY, 2*ballRadius, 2*ballRadius);
		//draw right paddle
		graph.FillRectangle(Brushes.White,rightPaddleX,rightPaddleY,paddleWidth,paddleHeight);
		//draw left paddle
		graph.FillRectangle(Brushes.White,leftPaddleX,leftPaddleY,paddleWidth,paddleHeight);
		//draw UI
		graph.FillRectangle(Brushes.White,0,totalheight-uiheight,framewidth,uiheight);				

		base.OnPaint(ee);
	}

//------------------------------------------------------------------------------

	protected override bool ProcessCmdKey(ref Message msg, Keys KeyCode)
   	{   
	        if(KeyCode == Keys.Up) {   
			upKey = keyPosition.down;
            	        if(downKey == keyPosition.up)
                            rightVerticalDelta = -20;
           	}
                if(KeyCode == Keys.Down) {  
		        downKey = keyPosition.down;
            	        if(upKey == keyPosition.up)
                            rightVerticalDelta = 20;
           	}
		if(KeyCode == Keys.W) {   
			wKey = keyPosition.down;
            	        if(sKey == keyPosition.up)
                            leftVerticalDelta = -20;
           	}
                if(KeyCode == Keys.S) {  
		        sKey = keyPosition.down;
            	        if(wKey == keyPosition.up)
                            leftVerticalDelta = 20;
           	}
		// prevent paddles from moving when game is paused	
		if (graphicRefreshClock.Enabled == false) {
			rightVerticalDelta = 0;
			leftVerticalDelta = 0;
		}
		rightPaddleY += rightVerticalDelta;
		leftPaddleY += leftVerticalDelta;
      	        Invalidate();
                return base.ProcessCmdKey(ref msg, KeyCode);
        }

	private void OnKeyUp(Object sender, KeyEventArgs e)
   	{   
		if(e.KeyCode == Keys.Up)
           	{   
                        upKey = keyPosition.up;
	            if(downKey == keyPosition.down)
                        rightVerticalDelta = 20;
                    else
                        rightVerticalDelta = 0;
           	}
                if(e.KeyCode == Keys.Down)
           	{   
		    downKey = keyPosition.up;
                    if(upKey == keyPosition.down)
                       rightVerticalDelta = -20;
                    else
                       rightVerticalDelta = 0;
           	}
		if(e.KeyCode == Keys.W)
           	{   
                        wKey = keyPosition.up;
	            if(sKey == keyPosition.down)
                        leftVerticalDelta = 20;
                    else
                        leftVerticalDelta = 0;
           	}
		if(e.KeyCode == Keys.S)
           	{   
                        sKey = keyPosition.up;
	            if(wKey == keyPosition.down)
                        leftVerticalDelta = 20;
                    else
                        leftVerticalDelta = 0;
           	}
		//prevent paddles from moving when game is paused
		if (graphicRefreshClock.Enabled == false) {
			rightVerticalDelta = 0;
			leftVerticalDelta = 0;
		}
           rightPaddleY += rightVerticalDelta; 
	   leftPaddleY += leftVerticalDelta;
           Invalidate();
        }

//------------------------------------------------------------------------------

	protected void StartOrStop(object sender, EventArgs events)
	{
		if (!graphicRefreshClock.Enabled) {
			if (speedTextBox.Text == "") {
				Console.WriteLine("Please enter a valid speed (0-100).\n50 is recommended.");
				return;
			}
			if (Convert.ToDouble(speedTextBox.Text) < 0 || Convert.ToDouble(speedTextBox.Text) > 100) {
				Console.WriteLine("Please enter a valid speed (0-100).\n50 is recommended.");
				return;
			}
			ballDistanceRefresh = Convert.ToDouble(speedTextBox.Text) * .2;
			//ballUpdateRate = Convert.ToDouble(speedTextBox.Text) * 0.71;
			ballHorizontalDelta = ballDistanceRefresh * System.Math.Cos (angleRadians);
			ballVerticalDelta = ballDistanceRefresh * System.Math.Sin (angleRadians);
			//Convert.ToDouble(speedTextBox.Text) > 50? Convert.ToDouble(speedTextBox.Text) * 0.71 : 40.9;
			Console.WriteLine ("You started the program.");
			goButton.Text = "Pause";
			StartGraphicClock (graphicRefreshRate);
			StartBallClock (ballUpdateRate);
			//prevent speed from being edited
			speedTextBox.ReadOnly = true;
			speedTextBox.BackColor = System.Drawing.SystemColors.Window;		
		} 
		else {
			Console.WriteLine ("You paused the program.");
			goButton.Text = "Start";
			graphicRefreshClock.Enabled = false;
			ballControlClock.Enabled = false;
			ballClockActive = false;
		}
	}

	protected void QuitProgram(Object sender, EventArgs events)
	{
		Close ();
	}

	protected void StartGraphicClock(double refreshRate)
	{
		double elapsedTime;
		if (refreshRate < 1.0)
			refreshRate = 1.0;
		elapsedTime = 1000.0 / refreshRate;	//milliseconds
		graphicRefreshClock.Interval = (int)System.Math.Round(elapsedTime);
		graphicRefreshClock.Enabled = true;
	}

	protected void StartBallClock(double updateRate)
	{   
		double elapsedTime;
		if(updateRate < 1.0) 
			updateRate = 1.0;  
		elapsedTime = 1000.0/updateRate;  //1000.0ms = 1second.  Units are milliseconds
		ballControlClock.Interval = (int)System.Math.Round(elapsedTime);
		ballControlClock.Enabled = true;  
		ballClockActive = true;
	}

	protected void UpdateDisplay(System.Object sender, ElapsedEventArgs evt)
	{
		Invalidate ();
		if (!ballClockActive) {
			graphicRefreshClock.Enabled = false;
		}
	}

	protected void UpdateBall(System.Object sender, ElapsedEventArgs evt)
	{	
		//end game if score limit is reached
		if (totalScore >= 10) {
			StopGame();
		}
		//update position of ball
		ballRealX = ballRealX + ballHorizontalDelta;
		ballRealY = ballRealY - ballVerticalDelta;
		ballIntX = (int)System.Math.Round (ballRealX);
		ballIntY = (int)System.Math.Round (ballRealY);
		
		//ball collides when in contact with a wall 
		if(ballIntX <= -40) { 
			rightScore++;
			totalScore++;
			playerRightLabel.Text = "Right Player\n        " + Convert.ToString(rightScore);
			ResetBall();
		}
		if(ballIntY <= 0) { 
			ballVerticalDelta = - ballVerticalDelta;
		}
		if(ballIntX+2*ballRadius >= framewidth+40) { 
			leftScore++;
			totalScore++;
			playerLeftLabel.Text = "Left Player\n        " + Convert.ToString(leftScore);
			ResetBall();
		}
		if(ballIntY+2*ballRadius >= frameheight) {
			ballVerticalDelta = - ballVerticalDelta;
		}
		//ballides collides with a paddle
		// right
		if ( ballRealX+2*ballRadius >= rightPaddleX &&
		     ballRealY+2*ballRadius >= rightPaddleY-ballRadius*2 &&
		     ballRealX+2*ballRadius <= rightPaddleX+paddleWidth &&
		     ballRealY+2*ballRadius <= rightPaddleY+paddleHeight+ballRadius*2 )
		{    
		     ballHorizontalDelta = - ballHorizontalDelta;
		}
		// left
		if ( ballRealX >= leftPaddleX &&
		     ballRealY >= leftPaddleY &&
		     ballRealX <= leftPaddleX+paddleWidth &&
		     ballRealY <= leftPaddleY+paddleHeight )
		{    
		     ballHorizontalDelta = - ballHorizontalDelta;
		}
	}

	protected void ResetBall() {
		//reinitialize coordinates
		algorithms = new Logic();
		ballRealX = (double)(framewidth/2 - ballRadius);
		ballRealY = (double)(frameheight/2 - ballRadius);
		ballIntX = (int)(ballRealX);
		ballIntY = (int)(ballRealY);

		angleRadians = algorithms.getRandomAngle();
		ballDistanceRefresh += 1.0;		
		ballHorizontalDelta = ballDistanceRefresh * System.Math.Cos (angleRadians);
		ballVerticalDelta = ballDistanceRefresh * System.Math.Sin (angleRadians);
		
		//pause ball for three seconds before new round
		stopwatch = new Stopwatch();
		stopwatch.Start();
		while (stopwatch.Elapsed.TotalSeconds < 2) {
			// wait for 2 seconds
			ballRealX = (double)(framewidth/2 - ballRadius);
			ballRealY = (double)(frameheight/2 - ballRadius);
			ballIntX = (int)(ballRealX);
			ballIntY = (int)(ballRealY);			
		}
		stopwatch.Stop();	
	}

	protected void ResetGame(object sender, EventArgs events) {
		Console.WriteLine("A new game was made.");
		Console.WriteLine("Enter a speed (1-100) and press \"Go\". 50 is recommended.");
		//reset ball
		ballRealX = (double)(framewidth/2 - ballRadius);
		ballRealY = (double)(frameheight/2 - ballRadius);
		ballIntX = (int)(ballRealX);
		ballIntY = (int)(ballRealY);
		//reset paddles
		// right
		rightPaddleX = framewidth - 40;
		rightPaddleY = frameheight/2 - paddleHeight/2;
		// left
		leftPaddleX = 20;
		leftPaddleY = frameheight/2 - paddleHeight/2;
		//stop game
		graphicRefreshClock.Enabled = false;
		ballControlClock.Enabled = false;
		ballClockActive = false;
		//reset scores
		leftScore = 0;
		rightScore = 0;
		totalScore = 0;
		playerLeftLabel.Text = "Left Player\n        " + Convert.ToString(leftScore);
		playerRightLabel.Text = "Right Player\n        " + Convert.ToString(rightScore);
		//enter new speed
		speedTextBox.ReadOnly = false;
		goButton.Enabled = true;
		goButton.Text = "Go";

		Invalidate();
	}

	protected void StopGame() {
		goButton.Enabled = false;

		graphicRefreshClock.Enabled = false;
		ballControlClock.Enabled = false;
		ballClockActive = false;

		go.Text = "Game Over";
	}
}
