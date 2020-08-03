using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.instances
{
    public class MainBoardData
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Size { get; set; }

        public MainBoardData(string name, string type, string size)
        {
            Name = name;
            Type = type;
            Size = size;
        }
    }
}
