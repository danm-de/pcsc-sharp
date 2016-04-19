// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake

// Properties
let buildDir = "./.build/"
let binaryOutDir = ""
let testDir = "./.build/"
let packagingDir = buildDir + "/package"
let buildConfig = environVarOrDefault "build_configuration" "Release"

// Targets
Target "Clean" (fun _ ->
    CleanDir buildDir
    !! "*.sln"
      |> MSBuild binaryOutDir "Clean"  ["Configuration", buildConfig; "Platform", "Any CPU"] 
      |> Log "MSBuild-Clean: "
)

Target "Build" (fun _ ->
    !! "*.sln"
      |> MSBuild binaryOutDir "Build" ["Configuration", buildConfig; "Platform", "Any CPU"]
      |> Log "MSBuild: "
)

Target "Test" (fun _ ->
    !! (sprintf "Tests/**/bin/%s/*.Test.dll" buildConfig)
      |> NUnit (fun p ->
          {p with
             DisableShadowCopy = true;
             OutputFile = testDir + "TestResults.xml" })
)

Target "NuGet" (fun _ ->
    Paket.Pack(fun p ->
        {p with
           Symbols = true;
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
  ==> "Build"
  ==> "NuGet"
  ==> "Test"
  ==> "Default"

"NuGet"
  ==> "NuGetPush"

// start build
RunTargetOrDefault "Default"
