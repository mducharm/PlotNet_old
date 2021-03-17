
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;

namespace PlotNet
{
    public static class GraphGenerator
    {
        public static IEnumerable<string> GetCsprojPaths(string path) =>
            Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);

        /// <summary>
        /// Parses out the dependencies of a csproj file, removing paths as
        /// well as .csproj to filter out duplicates
        /// </summary>
        /// <param name="path">The path of the csproj file</param>
        /// <returns>
        /// A list of dependencies
        /// </returns>
        public static Project ParseCsproj(string path)
        {
            string projectName = Path.GetFileNameWithoutExtension(path);

            IEnumerable<string> references = XDocument.Load(path)
                .Descendants("ProjectReference")
                .Select(p => p.Attribute("Include").Value)
                // Drop everything before the last \
                .Select(p =>
                {
                    string[] split = p.Split('\\');
                    return split[^1];
                })
                // Drop .csproj if it has it
                .Select(p => p.Replace(".csproj", ""))
                .Distinct();

            return new Project(projectName, references);
        }

        public static Func<Project, Project> FilterReferences(string referencesToKeep)
        {
            return (Project project) =>
                project with
                {
                    References = project.References.Where(r => r.Contains(referencesToKeep))
                };
        }

        public static IEnumerable<Project> GenerateGraphData(string path) =>
            GetCsprojPaths(path)
                .Select(ParseCsproj);

    }
}