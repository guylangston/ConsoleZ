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
    
    dotnet build -c Release --no-incremental "-p:PackageVersion=$ver"
    dotnet pack -c Release "-p:PackageVersion=$ver"
    
    
    copy ".\ConsoleZ\bin\Release\$name.$ver.nupkg"  "C:\Projects\LocalNuGet\$name.$ver.nupkg"
    echo " ==> C:\Projects\LocalNuGet\$name.$ver.nupkg"
    

    copy ".\ConsoleZ.AspNetCore\bin\Release\$name.AspNetCore.$ver.nupkg"  "C:\Projects\LocalNuGet\$name.AspNetCore.$ver.nupkg"
    echo " ==> C:\Projects\LocalNuGet\$name.AspNetCore.$ver.nupkg"
    
    
    popd
}

