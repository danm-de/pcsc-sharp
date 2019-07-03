#load ".fake/build.fsx/intellisense.fsx"
open Fake.Core
open Fake.DotNet
open Fake.DotNet.NuGet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

Target.create "Clean" (fun _ ->
    !! "src/**/bin"
    ++ "src/**/obj"
    ++ "Tests/**/bin"
    ++ "Tests/**/obj"
    |> Shell.cleanDirs
)

Target.create "Build" (fun _ ->
    !! "src/**/*.*proj"
    ++ "Tests/**/*.*proj"
    |> Seq.iter (DotNet.build id)
)

Target.create "Test" (fun _ ->
    !! "Tests/**/*.*proj"
      |> Seq.iter (DotNet.test id)
)

Target.create "NuGetPush" (fun _ ->
    let setNugetPushParams (defaults:NuGet.NuGetPushParams) =
        { defaults with
            Source = Some "https://api.nuget.org/v3/index.json"
        }

    let setParams (defaults:DotNet.NuGetPushOptions) =
        { defaults with
            PushParams = setNugetPushParams defaults.PushParams
        }

    !! "src/**/bin/Release/*.nupkg"
      |> Seq.iter (fun file -> DotNet.nugetPush setParams file)


)

Target.create "All" ignore

"Clean"
  ==> "Build"
  ==> "Test"
  ==> "All"

"Build"
  ==> "NuGetPush"

Target.runOrDefault "All"
