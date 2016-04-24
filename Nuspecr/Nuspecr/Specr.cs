namespace Nuspecr
{
    using Mono.Cecil;
    using NuGet;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;

    public class Specr
    {
        private const string manifestExtension = ".nuspec";

        public void CreateNuspec(string path, PackageVersion packageVersion)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!Enum.IsDefined(typeof(PackageVersion), packageVersion))
            {
                throw new InvalidEnumArgumentException(nameof(packageVersion), (int)packageVersion, typeof(PackageVersion));
            }

            var sourcePath = Path.GetFullPath(path);
            var assembly = AssemblyDefinition.ReadAssembly(sourcePath);
            var manifest = new Manifest
            {
                Metadata =
                {
                    Id = assembly.Name.Name,
                    Version = $"{assembly.GetFullVersion(packageVersion)}+{DateTime.UtcNow:yyyyMMddHHmmss}",
                    Copyright = assembly.GetCopyright(),
                    Authors = assembly.GetCompany(),
                    Owners = Environment.UserName,
                    Description = assembly.GetDescription(),
                    Title = assembly.GetTitle(),
                    DependencySets = assembly.GetPotentialNuGetDependencySets()
                },
                Files = new List<ManifestFile>
                {
                    new ManifestFile
                    {
                        Source = sourcePath,
                        Target = assembly.GetTargetPath(sourcePath)
                    }
                }
            };

            this.AppendResourceFiles(sourcePath, manifest.Files);

            var nuspecPath = Path.Combine(Path.GetDirectoryName(sourcePath), string.Concat(manifest.Metadata.Id, '.', manifest.Metadata.Version, Constants.ManifestExtension));
            manifest.Save(nuspecPath);
        }

        private void AppendResourceFiles(string sourcePath, IList<ManifestFile> manifestFiles)
        {
            var targetFilePath = manifestFiles[0].Target;
            var targetFileDirectoryName = Path.GetDirectoryName(targetFilePath);

            var sourceDirectoryName = Path.GetDirectoryName(sourcePath);
            var resourceFileName = Path.ChangeExtension(Path.GetFileName(sourcePath), ".resources.dll");
            foreach (var resourceFilePath in Directory.EnumerateFiles(sourceDirectoryName, resourceFileName, SearchOption.AllDirectories))
            {
                var resourceFileDirectoryName = Path.GetDirectoryName(resourceFilePath);
                if (resourceFileDirectoryName.Equals(sourceDirectoryName, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var resourceFileDirectory = Path.GetFileName(resourceFileDirectoryName);
                manifestFiles.Add(new ManifestFile
                {
                    Source = resourceFilePath,
                    Target = Path.Combine(targetFileDirectoryName, resourceFileDirectory, Path.GetFileName(resourceFilePath))
                });
            }
        }
    }
}