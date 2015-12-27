call C:\DevStudio.2015\vc\vcvarsall.bat
call "D:\Program Files (x86)\Microsoft Visual Studio 14.0\vc\vcvarsall.bat"
call "%ProgramFiles(x86)%\Microsoft Visual Studio 14.0\vc\vcvarsall.bat"

set BuildTarget=Debug

devenv CNCLib\CNCLib.sln /Rebuild %BuildTarget%
devenv Plotter\Plotter.sln /Rebuild %BuildTarget%
devenv SpeedChart\SpeedChart.sln /Rebuild  %BuildTarget%

del /S /Q builds

md builds
md builds\CNCLib

copy CNCLib\CNCLib.Wpf.Start\bin\Debug\*.xml builds\CNCLib
copy CNCLib\CNCLib.Wpf.Start\bin\Debug\*.dll builds\CNCLib
copy CNCLib\CNCLib.Wpf.Start\bin\Debug\CNCLib.Wpf.Start.exe builds\CNCLib
copy CNCLib\CNCLib.Wpf.Start\bin\Debug\CNCLib.Wpf.Start.exe.config builds\CNCLib

xcopy /E CNCLib\CNCLib.Wpf.Start\bin\Debug\amd64 builds\CNCLib\amd64\*.*
xcopy /E CNCLib\CNCLib.Wpf.Start\bin\Debug\x86 builds\CNCLib\x86\*.*

md builds\Plotter

copy Plotter\Plotter.GUI\bin\Debug\*.dll builds\Plotter
copy Plotter\Plotter.GUI\bin\Debug\Plotter.GUI.exe builds\Plotter
copy Plotter\Plotter.GUI\bin\Debug\Plotter.GUI.exe.config builds\Plotter

md builds\SpeedChart

copy SpeedChart\SpeedChart\SpeedChart\bin\Debug\*.dll builds\SpeedChart
copy SpeedChart\SpeedChart\bin\Debug\SpeedChart.exe builds\SpeedChart
copy SpeedChart\SpeedChart\bin\Debug\SpeedChart.exe.config builds\SpeedChart

pause