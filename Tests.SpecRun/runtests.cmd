@pushd %~dp0
@set profile=%1
@if "%profile%" == "" set profile=Default
..\..\..\packages\SpecRun.Runner.1.6.3\tools\SpecRun.exe run %profile%.srprofile "/outputFolder:%~dp0/TestResults/" /log:specrun.log %2 %3 %4 %5
@popd