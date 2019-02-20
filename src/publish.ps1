param(
     [string]$ver
    )

ls C:\Projects\LocalNuGet\

if ($ver -eq ""){
    $ver = Read-Host -Prompt 'Version as "0.1.6"'
}

pushd
#$ver = "0.1.6"

cd ConsoleZ
dotnet build -c Release
dotnet pack -c Release "-p:PackageVersion=$ver"
#ls .\bin\Release
copy ".\bin\Release\ConsoleZ.$ver.nupkg"  "C:\Projects\LocalNuGet\ConsoleZ.$ver.nupkg"

cd ..
cd ConsoleZ.AspNetCore
dotnet build -c Release
dotnet pack -c Release "-p:PackageVersion=$ver"
#ls .\bin\Release
copy ".\bin\Release\ConsoleZ.AspNetCore.$ver.nupkg"  "C:\Projects\LocalNuGet\ConsoleZ.AspNetCore.$ver.nupkg"

ls C:\Projects\LocalNuGet\

popd