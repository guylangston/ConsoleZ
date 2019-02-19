pushd
$ver = "0.1.4"

cd ConsoleZ
dotnet pack -c Release "-p:PackageVersion=$ver"
#ls .\bin\Release
copy ".\bin\Release\ConsoleZ.$ver.nupkg"  "C:\Projects\ConsoleZ\dist\ConsoleZ.$ver.nupkg"

cd ..
cd ConsoleZ.AspNetCore
dotnet pack -c Release "-p:PackageVersion=$ver"
#ls .\bin\Release
copy ".\bin\Release\ConsoleZ.AspNetCore.$ver.nupkg"  "C:\Projects\ConsoleZ\dist\ConsoleZ.AspNetCore.$ver.nupkg"

ls C:\Projects\ConsoleZ\dist\

popd