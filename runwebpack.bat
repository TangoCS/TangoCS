echo off
set sln=%1
call set proj_client=%%sln:.sln=.Client%%
cd %proj_client%
webpack