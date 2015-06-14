"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" NetInjector.sln

cd Nuget
mkdir build
cd build
mkdir lib
mkdir lib\net45

copy ..\..\NetInjector\bin\Debug\NetInjector.* lib\net45

copy ..\NetInjector.nuspec .
..\nuget pack NetInjector.nuspec
copy *.nupkg ..

cd..
rmdir build /S /Q
cd ..