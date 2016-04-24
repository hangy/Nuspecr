namespace Nuspecr
{
    using CommandLine;
    using System.Collections.Generic;

    [Verb("spec", HelpText = "Creates (a) nuspec file(s) for one or more given assembly/assemblies.")]

    public class SpecOptions
    {
        [Option('i', "input", Required = true, HelpText = "Assemblies to create (separate) Nuspec files for.")]
        public IEnumerable<string> InputFiles { get; set; }

        [Option('v', "version", Required = false, HelpText = "Assembly attribute to harvest packag version from.", Default = PackageVersion.AssemblyVersionAttribute)]
        public PackageVersion VersionSource { get; set; }
    }
}
