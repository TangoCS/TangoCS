sc create  "Tango.TaskManager" binpath= "Tango.TaskManager.exe --windows-service" start= delayed-auto
sc start Tango.TaskManager
pause
