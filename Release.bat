@echo OFF
set nugetversion=1.0.1
set packageid=global-funcs
set projectid=DotNetGlobalFuncs.csproj
set packagepath=./artifacts/%packageid%.%nugetversion%.nupkg
set src=.\src\%projectid%

dotnet build %src% -c Release
dotnet pack %src% -c Release -o ./artifacts /p:Version=%nugetversion%

rem test nuget
rem nuget push %packagepath% -Source "C:\Box\NuGet"

rem WVN Nuget
rem nuget delete %packageid% %nugetversion% -Source https://api.nuget.org/v3/index.json -ApiKey %WVN_NUGET_API_KEY% -NonInteractive
nuget push %packagepath% %WVN_NUGET_API_KEY% -Source 'nuget.org'

rem WVN github
rem dotnet nuget push %packagepath%  --source "github"