using System.Collections.Generic;

namespace PlotNet
{
    public record Project (
        string Name,
        IEnumerable<string> References
    );
}