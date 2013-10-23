properties {
    $base_dir = Resolve-Path .
    $sln_file = "$base_dir\pcsc-sharp.sln"
}

Task default -depends Compile

Task Clean {
    msbuild "/property:Configuration=Release" "/t:Clean" "$sln_file"
}

Task Compile -depends Clean {
    msbuild "/property:Configuration=Release" "$sln_file"
}

Task Package -depends Compile {
    nuget pack $base_dir\pcsc-sharp\pcsc-sharp.csproj -Symbols -Prop Configuration=Release
}

Task PushPackage -depends Package {
    nuget push $base_dir\PCSC.?.?.?.?.nupkg
    nuget push $base_dir\PCSC.?.?.?.?.symbols.nupkg
}

