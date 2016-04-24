Nuspecr for .NET
===

Ever needed a NuGet package for a third-party library that you've licensed but which is not being distributed as NuGet packages? Just use Nuspecr to create some `Nuspec` files that can be used with `NuGet.exe` to create packages.

Difference to `NuGet spec`
---
The main difference to using `NuGet spec` is that the create Nuspec file will only contain one DLL and it's resources, but it will include dependencies to detected referenced assemblies.
This certainly isn't useful for in all cases, and you may have to modify the generated file in order to get the desired final result, but it should work really well for largers suites of 3rd party libraries which contain several shared assemblies.