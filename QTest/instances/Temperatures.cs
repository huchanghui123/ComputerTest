using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.Tools
{
    class Temperatures
    {
        public string name;
        public float value;
        public float minvalue;
        public float maxvalue;

        public Temperatures(string name, float value, float minvalue, float maxvalue)
        {
            this.name = name;
            this.value = value;
            this.minvalue = minvalue;
            this.maxvalue = maxvalue;
        }
    }
}
