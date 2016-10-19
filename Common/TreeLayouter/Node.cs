using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.TreeLayouter
{
    public interface Node
    {
        IEnumerable<Node> Children { get; }
    }
}
