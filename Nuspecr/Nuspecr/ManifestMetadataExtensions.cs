namespace Nuspecr
{
    using NuGet;
    using System;

    public static class ManifestMetadataExtensions
    {
        public static void EnsureTitleIsNotEmpty(this ManifestMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (string.IsNullOrWhiteSpace(metadata.Title))
            {
                metadata.Title = metadata.Id;
            }
        }

        public static void EnsureDescriptionIsNotEmpty(this ManifestMetadata metadata)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (string.IsNullOrWhiteSpace(metadata.Description))
            {
                metadata.Description = metadata.Title;
            }
        }
    }
}
