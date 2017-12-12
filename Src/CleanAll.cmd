pushd .

call C:\DevStudio.2015\vc\vcvarsall.bat
call "D:\Program Files (x86)\Microsoft Visual Studio 14.0\vc\vcvarsall.bat"
call "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\vc\vcvarsall.bat"
call "%ProgramFiles(x86)%\Microsoft Visual Studio\2017\Enterprise\VC\Auxiliary\Build\vcvars32.bat"

popd

set BuildTarget=Debug

devenv CNCLib\CNCLib.sln /Clean %BuildTarget%
devenv Plotter\Plotter.sln /Clean %BuildTarget%
devenv SpeedChart\SpeedChart.sln /Clean  %BuildTarget%

set BuildTarget=Release

devenv CNCLib\CNCLib.sln /Clean %BuildTarget%
devenv Plotter\Plotter.sln /Clean %BuildTarget%
devenv SpeedChart\SpeedChart.sln /Clean  %BuildTarget%


echo on

for /D /R  %%d in (TestResults) do if exist %%d rmdir /Q /S %%d
for /D /R  %%d in (packages) do if exist %%d rmdir /Q /S %%d
for /D /R  %%d in (Debug) do if exist %%d rmdir /Q /S %%d
for /D /R  %%d in (Release) do if exist %%d rmdir /Q /S %%d
for /D /R  %%d in (Bin) do if exist %%d rmdir /Q /S %%d
for /D /R  %%d in (Obj) do if exist %%d rmdir /Q /S %%d

pause