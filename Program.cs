using System;
using System.IO;
using System.Text.Json;
using static PlotNet.GraphGenerator;


var path = args[0] ?? Directory.GetCurrentDirectory();

var repoName = new DirectoryInfo(path).Name; 

var data = GenerateGraphData(path);
var serializedData = JsonSerializer.Serialize(data);

var generatedGraph = File.ReadAllText("template.html")
    .Replace("var data = [];", $"var data = {serializedData};");

File.WriteAllText($"{repoName}.Graph.html", generatedGraph);