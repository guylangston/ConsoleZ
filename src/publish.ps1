param(
     [string]$ver
    )
$name = "ConsoleZ"

if ([string]::IsNullOrEmpty($ver))
{
    $ver = Get-Content ./package-version.txt
}

echo "Package: '$name'"
echo "Package-Version: '$ver'"

$confirmation = Read-Host "Does the version look correct? y/n"
if ($confirmation -eq 'y') {
    pushd
    
    cd ConsoleZ
    dotnet build -c Release --no-incremental "-p:PackageVersion=$ver"
    dotnet pack -c Release "-p:PackageVersion=$ver"

    copy ".\bin\Release\$name.$ver.nupkg"  "C:\Projects\LocalNuGet\$name.$ver.nupkg"
    echo " ==> C:\Projects\LocalNuGet\$name.$ver.nupkg"
    cd ..
    
    
    cd ConsoleZ.AspNetCore
    dotnet build -c Release --no-incremental "-p:PackageVersion=$ver"
    dotnet pack -c Release "-p:PackageVersion=$ver"

    copy ".\bin\Release\$name.AspNetCore.$ver.nupkg"  "C:\Projects\LocalNuGet\$name.AspNetCore.$ver.nupkg"
    echo " ==> C:\Projects\LocalNuGet\$name.AspNetCore.$ver.nupkg"
    cd ..
    
    

 
    
    
    popd
}

