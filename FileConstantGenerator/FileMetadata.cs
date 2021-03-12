using System;

public class FileMetadata
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string Contents { get; set; }

    public override string ToString(){
        Func<string, string> wrapInQuotes = s => $"\"{s}\"";

        var ext = wrapInQuotes(Extension);
        var contents = wrapInQuotes(Contents.Replace("\"", "\"\""));

        return $@" 
        public const string {Name}Extension = {ext};
        public const string {Name}Contents = {contents};
        ";
    }  
}
