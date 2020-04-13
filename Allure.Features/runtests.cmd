@pushd %~dp0
@set profile=%1
@if "%profile%" == "" set profile=Default
SpecFlowPlusRunner\SpecRun.exe run %profile%.srprofile "/outputFolder:%~dp0/TestResults/" /log:specrun.log %2 %3 %4 %5
@popd