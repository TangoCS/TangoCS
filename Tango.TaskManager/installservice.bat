sc create  "Tango.TaskManager" binpath= "%~dp0Tango.TaskManager.exe --windows-service" start= delayed-auto
sc start Tango.TaskManager
pause
