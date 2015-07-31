// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = "./.build/"
let testDir = "./.build/"
let packagingDir = buildDir + "/package"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
)

Target "BuildApp" (fun _ ->
    !! "*.sln"
      |> MSBuildRelease buildDir "Build"
      |> Log "AppBuild-Output: "
)

Target "Test" (fun _ ->
    !! (testDir + "/*.Test.dll")
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = testDir + "TestResults.xml" })
)

Target "NuGet" (fun _ ->
    Paket.Pack(fun p ->
        {p with
           OutputPath = buildDir })
)

Target "NuGetPush" (fun _ ->
    Paket.Push(fun p -> 
        {p with
           WorkingDir = buildDir })
)

Target "Default" (fun _ ->
    trace "PC/SC wrapper classes for .NET"
)

// Dependencies
"Clean"
  ==> "BuildApp"
  ==> "Test"
  ==> "Default"

"BuildApp"
  ==> "NuGet"

"NuGet"
  ==> "NuGetPush"

// start build
RunTargetOrDefault "Default"
