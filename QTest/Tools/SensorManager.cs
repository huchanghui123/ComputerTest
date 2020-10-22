using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.Tools
{
    public sealed class SensorManager
    {
        private CpuTemperatureReader cpuCelsius;
        private static readonly SensorManager instance = new SensorManager();
        static SensorManager(){}

        public static SensorManager Instance
        {
            get
            {
                return instance;
            }
        }

        public CpuTemperatureReader CpuCelsius { get => cpuCelsius; set => cpuCelsius = value; }
    }
}
