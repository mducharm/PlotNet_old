using System;
using System.IO;
using System.Text.Json;
using static PlotNet.GraphGenerator;


var path = args[0] ?? Directory.GetCurrentDirectory();

var repoName = new DirectoryInfo(path).Name; 

var data = GenerateGraphData(path);

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};

var serializedData = JsonSerializer.Serialize(data, options);

var generatedGraph = File.ReadAllText("template.html")
    .Replace("<!-- data -->", $"{serializedData}");

File.WriteAllText($"{repoName}.Graph.html", generatedGraph);