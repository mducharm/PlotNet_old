using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using static PlotNet.GraphGenerator;

var path = args.ElementAtOrDefault(0) ?? Directory.GetCurrentDirectory();

var data = GenerateGraphData(path);

var options = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};

var serializedData = JsonSerializer.Serialize(data, options);

var generatedGraph = AdditionalFiles.AsConstants.templateContents.Replace("<!-- data -->", $"{serializedData}");

var repoName = new DirectoryInfo(path).Name; 

var fileName = $"{repoName}.Graph.html";

File.WriteAllText(fileName, generatedGraph);

Log($"Completed graph creation, file output: {fileName}");