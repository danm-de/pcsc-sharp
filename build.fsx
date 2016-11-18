// include Fake lib
#r "packages/FAKE/tools/FakeLib.dll"
open Fake
open Fake.Testing.NUnit3

// Properties
let buildDir = "./.build/"
let nugetDir = buildDir @@ "nuget"
let symbolDir = buildDir @@ "symbols"
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

Target "NuGet" (fun _ ->
    Paket.Pack(fun p ->
        {p with
           Symbols = true;
           OutputPath = buildDir })
    
    ensureDirectory nugetDir
    ensureDirectory symbolDir
    !! (buildDir @@ "*.nupkg")
        |> Seq.iter (fun file -> MoveFile nugetDir file)
    
    !! (nugetDir @@ "*.symbols.nupkg")
        |> Seq.iter (fun file -> MoveFile symbolDir file)
)

Target "NuGetPush" (fun _ ->
    Paket.Push(fun p -> 
        {p with
           DegreeOfParallelism = 1
           WorkingDir = nugetDir 
        })

    Paket.Push(fun p -> 
        {p with
           PublishUrl = "https://nuget.smbsrc.net/"
           DegreeOfParallelism = 1
           WorkingDir = symbolDir
        })
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
