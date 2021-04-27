## Pong
A straightforward implementation of the game Pong. Developed in **C#** using *windows.forms*, *drawing*, and *timers* libraries. Timer objects control the ball’s speed and the paddles’ movement. Each frame is drawn one-by-one and displayed at a high rate, mimicking the motion of objects. The starting angle of the ball’s trajectory is intentionally programmed to avoid assigning values near *90* and *270* degrees; avoiding acute angles prevents games wherein the ball bounces in a highly predictable manner.

## How to Play
Windows:
1.	Execute Pong.exe.
2.	In the dialogue box, enter a speed between 1 and 100 (50 is recommended) and click the “Go” button
3.	The left paddle is controlled by the “W” and “S” keys, and the right paddle the “Up” and “Down” arrow keys.
4.	The ball gets progressively faster after every round. The first player to reach a score of 10 wins.

Linux:
1.	Navigate to the folder “Pong” and enter the command
    >./build.sh
2.	Follow steps 2-4 from above.
