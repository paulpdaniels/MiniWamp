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
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild MiniWamp\MiniWamp.Net45.csproj /p:OutputPath=".\bin\%config%\Net45" /p:Platform="Any CPU" /p:StartupObject="" /t:rebuild /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild MiniWamp\MiniWamp.csproj /p:OutputPath=".\bin\%config%\PCL" /p:Platform="AnyCPU" /p:StartupObject="" /t:rebuild /p:Configuration="%config%" /m /v:M /fl /flp:LogFile=msbuild.log;Verbosity=Normal /nr:false



REM Package
call %NuGet% pack "MiniWamp\MiniWamp.nuspec" -o Build -Properties Configuration=%config% %version%