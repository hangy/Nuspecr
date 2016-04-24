namespace Nuspecr
{
    using Mono.Cecil;
    using System;
    using System.Reflection;

    internal static class GacUtility
    {
        private static readonly DefaultAssemblyResolver defaultAssemblyResolver = new DefaultAssemblyResolver();

        public static bool Contains(AssemblyNameReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            return GetAssemblyInGac(reference) != null;
        }

        private static AssemblyDefinition GetAssemblyInGac(AssemblyNameReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            // YOLO?! => AssemblyDefinition GetAssemblyInGac (AssemblyNameReference reference, ReaderParameters parameters)
            return ((AssemblyDefinition)typeof(BaseAssemblyResolver).GetMethod(nameof(GetAssemblyInGac), BindingFlags.Instance | BindingFlags.NonPublic).Invoke(defaultAssemblyResolver, new object[] { reference, new ReaderParameters() }));
        }
    }
}
