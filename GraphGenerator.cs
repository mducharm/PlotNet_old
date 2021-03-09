
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

        public static Project ParseCsproj(string path)
        {
            var projectName = Path.GetFileNameWithoutExtension(path);

            XDocument projectDefinition = XDocument.Load(path);

            var references = projectDefinition
            .Descendants("ProjectReference")
            .Select(p => p.Attribute("Include").Value);

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

        private static object CreateNode(string name) =>
            new
            {
                data = new
                {
                    id = name
                }
            };

        private static object CreateEdge(string source, string target) =>
            new
            {
                data = new
                {
                    id = $"{source}{target}",
                    source,
                    target
                }
            };

        public static IEnumerable<object> GenerateNodesAndEdges(this IEnumerable<Project> projects)
        {
            var nodesAndEdges = new List<object>();

            var nodes = projects
                .SelectMany(p => p.References.Append(p.Name))
                .Select(CreateNode);

            var edges = projects
                .SelectMany(p => p.References.Select(r => CreateEdge(p.Name, r)))
                .ToList();

            nodesAndEdges.AddRange(nodes);
            nodesAndEdges.AddRange(edges);
            return nodesAndEdges;
        }

        public static IEnumerable<object> GenerateGraphData(string path) =>
            GetCsprojPaths(path)
                .Select(ParseCsproj)
                .GenerateNodesAndEdges();

    }
}