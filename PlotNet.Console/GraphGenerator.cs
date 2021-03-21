
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
        public static IEnumerable<string> GetCsprojPaths(string path)
        {
            Log($"Checking {path}...");
            var files = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
            Log($"{files.Length} projects found.");
            return files;
        } 

        /// <summary>
        /// Parses out the dependencies of a csproj file, removing paths as
        /// well as .csproj to filter out duplicates
        /// </summary>
        /// <param name="path">The path of the csproj file</param>
        /// <param name="verbose">Logs out the references if supplied</param>
        /// <returns>
        /// A list of dependencies
        /// </returns>
        public static Project ParseCsproj(string path, bool verbose)
        {
            string projectName = Path.GetFileNameWithoutExtension(path);

            IEnumerable<string> references = XDocument.Load(path)
                .Descendants("ProjectReference")
                .Select(p => p.Attribute("Include").Value)
                // Drop everything before the last \
                .Select(p =>
                {
                    string[] split = p.Replace("/", "\\").Split('\\');
                    return split[^1];
                })
                // Drop .csproj if it has it
                .Select(p => p.Replace(".csproj", ""))
                .Distinct();

            Log($"{references.Count()} references found for {projectName}.");

            if (verbose)
            {
                foreach(string reference in references)
                {
                    Log($"          {reference}");
                }
            }

            return new Project(projectName, references);
        }

        /// <summary>
        /// Generates the projects from the path given
        /// </summary>
        /// <param name="path">The path of the solution</param>
        /// <param name="verbose">Determines log level</param>
        /// <returns>
        /// A list of projects
        /// </returns>
        public static IEnumerable<Project> GenerateProjects(string path, bool verbose) =>
            GetCsprojPaths(path)
                .Select(p => ParseCsproj(p, verbose));

        public static void Log(string msg) => 
            Console.WriteLine(msg);

    }
}