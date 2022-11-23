set proj_web=%1
call set proj_client=%%proj_web:.Web=.Client%%
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\wwwroot" "%proj_web%\wwwroot\"
xcopy /Y /S /D "..\..\TangoCS\Tango.Client.Js\src" "%proj_client%\src\"
