namespace Nuspecr
{
    using NuGet;
    using System;
    using System.IO;
    using System.Text.RegularExpressions;

    /// <remarks>
    /// <para>https://github.com/NuGet/NuGet.Client/blob/dev/src/NuGet.Clients/NuGet.CommandLine/Commands/SpecCommand.cs</para>
    /// <para>Copyright (c) .NET Foundation. All rights reserved.</para>
    /// <para>Licensed under the Apache License, Version 2.0 (the "License"); you may not use
    /// these files except in compliance with the License.You may obtain a copy of the
    /// License at http://www.apache.org/licenses/LICENSE-2.0</para>
    /// <para>Unless required by applicable law or agreed to in writing, software distributed
    /// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR
    /// CONDITIONS OF ANY KIND, either express or implied. See the License for the
    /// specific language governing permissions and limitations under the License.</para>
    /// </remarks>
    internal static class StuffBorrowedFromSpecCommand
    {
        public static void Save(this Manifest manifest, string nuspecPath)
        {
            if (string.IsNullOrWhiteSpace(nuspecPath))
            {
                throw new ArgumentNullException(nameof(nuspecPath));
            }

            if (File.Exists(nuspecPath))
            {
                Console.WriteLine($"{nuspecPath} already exists");
            }
            else
            {
                try
                {
                    using (var stream = new MemoryStream())
                    {
                        manifest.Save(stream, validate: false);
                        stream.Seek(0, SeekOrigin.Begin);
                        string content = stream.ReadToEnd();
                        File.WriteAllText(nuspecPath, RemoveSchemaNamespace(content));
                    }

                    Console.WriteLine($"{nuspecPath} created");
                }
                catch
                {
                    // Cleanup the file if it fails to save for some reason
                    File.Delete(nuspecPath);
                    throw;
                }
            }
        }

        private static string RemoveSchemaNamespace(string content)
        {
            // This seems to be the only way to clear out xml namespaces.
            return Regex.Replace(content, @"(xmlns:?[^=]*=[""][^""]*[""])", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        }
    }
}
