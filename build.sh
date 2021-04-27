#!/bin/bash

echo "The script has started"

#remove residual files
rm *.dll
rm *.exe

#compile logic
mcs -target:library logic.cs -r:System.Windows.Forms.dll -out:logic.dll

#compile frame
mcs -target:library frame.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:logic.dll -out:frame.dll 

#link dll files and create exe
mcs -target:exe main.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll -r:frame.dll -out:Pong.exe 

#execute program
./Pong.exe

echo "The script has terminated"
