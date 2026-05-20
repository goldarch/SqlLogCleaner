using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlLogCleaner
{
    public class ToolTemplate
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public override string ToString() { return Name; }
    }
}
