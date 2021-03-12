using System.Text;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Saves an .html file as a constant.
/// 
///  e.g.
/// <ItemGroup>
///     <AdditionalFiles Include="index.html" />
/// </ItemGroup
/// </summary>
[Generator]
public class FileConstantGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context) {}

    public void Execute(GeneratorExecutionContext context)
    {

        // var (_, htmlText) = GetHtml(context);

        var fileData = GetFiles(context);
        // var source = "";

        var source = fileData.Select(file => file.ToString())
                                .Aggregate("", (a, c) => $"{a}{c}\n");

        // htmlText = htmlText.Replace("\"", "\"\"");

        context.AddSource("Template.cs", SourceText.From(@"
using System;
namespace AdditionalFiles
{
    public class AsConstants
    {
        " 
    + source + 
    @"}
}", Encoding.UTF8));
    }

    
    static IEnumerable<FileMetadata> GetFiles(GeneratorExecutionContext context)
    {
        return context.AdditionalFiles.Select((AdditionalText f) => {
            return new FileMetadata
            {
                Name = Path.GetFileNameWithoutExtension(f.Path),
                Extension = Path.GetExtension(f.Path),
                Contents = f.GetText().ToString()
            };
        });

    }

}