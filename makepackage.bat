"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe" NetInjector.sln

cd Nuget
mkdir lib
mkdir lib\net45

copy ..\NetInjector\bin\Debug\NetInjector.* lib\net45

nuget pack NetInjector.nuspec

rmdir lib /S /Q

cd ..