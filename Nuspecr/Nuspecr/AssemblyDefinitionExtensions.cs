namespace Nuspecr
{
    using Mono.Cecil;
    using NuGet;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;

    public static class AssemblyDefinitionExtensions
    {
        public static List<ManifestDependencySet> GetPotentialNuGetDependencySets(this AssemblyDefinition assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var mainModule = assembly.MainModule;
            if (!mainModule.HasAssemblyReferences)
            {
                return new List<ManifestDependencySet>(0);
            }

            return new List<ManifestDependencySet>(1)
            {
                new ManifestDependencySet
                {
                    TargetFramework = assembly.GetFramework(),
                    Dependencies = assembly.GetPotentialNuGetDependencies()
                }
            };
        }

        public static List<ManifestDependency> GetPotentialNuGetDependencies(this AssemblyDefinition assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            var mainModule = assembly.MainModule;
            if (!mainModule.HasAssemblyReferences)
            {
                return new List<ManifestDependency>(0);
            }

            var result = new List<ManifestDependency>(mainModule.AssemblyReferences.Count);
            foreach (var reference in mainModule.AssemblyReferences)
            {
                if (GacUtility.Contains(reference))
                {
                    continue;
                }

                result.Add(new ManifestDependency
                {
                    Id = reference.Name,
                    Version = $"[{reference.Version.ToString()}]"
                });
            }

            return result;
        }

        public static string GetTargetPath(this AssemblyDefinition assembly, string sourcePath)
        {
            if (sourcePath == null)
            {
                throw new ArgumentNullException(nameof(sourcePath));
            }

            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException($"{nameof(sourcePath)} is null or empty/whitespace", nameof(sourcePath));
            }

            return Path.Combine("lib", assembly.GetFramework(), Path.GetFileName(sourcePath));
        }

        public static string GetFramework(this AssemblyDefinition assembly)
        {
            return $"net{1 + (int)assembly.MainModule.Runtime}0";
        }

        public static string GetFullVersion(this AssemblyDefinition assembly, PackageVersion packageVersion)
        {
            if (!Enum.IsDefined(typeof(PackageVersion), packageVersion))
            {
                throw new InvalidEnumArgumentException(nameof(packageVersion), (int)packageVersion, typeof(PackageVersion));
            }

            if (packageVersion == PackageVersion.AssemblyVersionAttribute)
            {
                return assembly.Name.Version.ToString();
            }

            return assembly.CustomAttributes.GetFirstAttributeValueOrDefault(packageVersion.ToString(), assembly.Name.Version.ToString);
        }

        public static string GetCopyright(this AssemblyDefinition assembly)
        {
            return assembly.GetAssemblyAttribute<AssemblyCopyrightAttribute>();
        }

        public static string GetCompany(this AssemblyDefinition assembly)
        {
            return assembly.GetAssemblyAttribute<AssemblyCompanyAttribute>();
        }

        public static string GetDescription(this AssemblyDefinition assembly)
        {
            return assembly.GetAssemblyAttribute<AssemblyDescriptionAttribute>();
        }

        public static string GetTitle(this AssemblyDefinition assembly)
        {
            return assembly.GetAssemblyAttribute<AssemblyTitleAttribute>();
        }

        private static string GetAssemblyAttribute<TAssemblyAttribute>(this AssemblyDefinition assembly) where TAssemblyAttribute : Attribute
        {
            return assembly.CustomAttributes.GetFirstAttributeValueOrDefault<string>(typeof(TAssemblyAttribute).Name, null);
        }
    }
}
