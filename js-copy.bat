set proj_web=%1
call set proj_client=%%proj_web:.Web=.Client%%
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\wwwroot" "%proj_web%\wwwroot\"
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\wwwroot\js\tango" "%proj_client%\src\tango\"
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\wwwroot\js\calendar" "%proj_client%\src\calendar\"
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\wwwroot\js\contextmenu" "%proj_client%\src\contextmenu\"
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\wwwroot\js\daterangepicker" "%proj_client%\src\daterangepicker\"
