// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.NUnit3

// Properties
let buildDir = "./.build/"
let nugetDir = buildDir @@ "nuget"
let binaryOutDir = ""
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
    !! (sprintf "Tests/**/bin/%s/*.Tests.dll" buildConfig)
      |> NUnit3 (fun p ->
          {p with
             ShadowCopy = false
          })
)

Target "NuGetPush" (fun _ ->
    Paket.Push(fun p -> 
        {p with
           DegreeOfParallelism = 1
           WorkingDir = nugetDir 
        })
)

Target "Default" (fun _ ->
    trace "PC/SC wrapper classes for .NET"
)

// Dependencies
"Clean"
  ==> "Build"
  ==> "Test"
  ==> "Default"

"Build"
  ==> "NuGetPush"

// start build
RunTargetOrDefault "Default"
