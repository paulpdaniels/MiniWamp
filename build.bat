@echo Off
set config=%1
if "%config%" == "" (
   set config=Release
)

set version=
if not "%PackageVersion%" == "" (
   set version=-Version %PackageVersion%
)

REM Package restore
call %NuGet% restore MiniWamp\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %NuGet% restore MiniWampTests.Net45\packages.config -OutputDirectory %cd%\packages -NonInteractive
call %NuGet% restore MiniWampTests\packages.config -OutputDirectory %cd%\packages -NonInteractive

REM Build
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild YourSolution.sln /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false

REM Package
mkdir Build
call %nuget% pack "MiniWamp.nuspec" -IncludeReferencedProjects -o Build -p Configuration=%config% %version%