using OpenHardwareMonitor.Hardware;
using QTest.instances;
using System;
using System.Collections.Generic;

namespace QTest.Tools
{
    public class CpuTemperatureReader : IDisposable
    {
        private Computer _computer;

        public CpuTemperatureReader()
        {
            _computer = new Computer
            {
                MainboardEnabled = true,
                CPUEnabled = true,
                //GPUEnabled = true,
                HDDEnabled = true,
                RAMEnabled = true
                //FanControllerEnabled = true
            };
            _computer.Open();
        }

        public List<SensorData> GetTemperaturesInCelsius()
        {
            List<SensorData> sensorList = new List<SensorData>();
            try
            {
                foreach (var hardware in _computer.Hardware)
                {
                    hardware.Update();

                    //Console.WriteLine("hardware name:{0}, type:{1}", hardware.Name, hardware.HardwareType);
                    //通过SUPERIO获取主板的电压、温度、CPU风扇转速 
                    if (hardware.HardwareType == HardwareType.Mainboard)
                    {
                        foreach (var subhardware in hardware.SubHardware)
                        {
                            subhardware.Update();
                            //Console.WriteLine("subhardware Name:{0}", subhardware.Name);
                            sensorList.Add(new SensorData(subhardware.Name));
                            foreach (var sensor in subhardware.Sensors)
                            {
                                if (sensor.SensorType == SensorType.Voltage && sensor.Name.Equals("VBat", StringComparison.OrdinalIgnoreCase))
                                {
                                    //Console.WriteLine("{0} Voltage:{1} min:{2} max:{3}", sensor.Name, sensor.Value.Value,sensor.Min.Value, sensor.Max.Value);
                                    sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value + " V",
                                        sensor.Min.Value + " V", sensor.Max.Value + " V"));
                                }
                                else if (sensor.SensorType == SensorType.Temperature)
                                {
                                    //Console.WriteLine("{0} Temperature value:{1} min:{2} max:{3}",sensor.Name, sensor.Value.Value, sensor.Min.Value, sensor.Max.Value);
                                    sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value + " °C",
                                        sensor.Min.Value + " °C", sensor.Max.Value + " °C"));
                                }
                                else if (sensor.SensorType == SensorType.Fan)
                                {
                                    //Console.WriteLine("{0} Speed value:{1} min:{2} max:{3}",sensor.Name, Math.Floor(sensor.Value.Value) + " RPM", sensor.Min.Value + "RPM", sensor.Max.Value + " RPM");
                                    sensorList.Add(new SensorData(sensor.Name, Math.Floor(sensor.Value.Value) + " RPM",
                                        Math.Floor(sensor.Min.Value) + " RPM", Math.Floor(sensor.Max.Value) + " RPM"));
                                }
                            }
                        }
                    }
                    //获取CPU核心使用率、温度、功耗
                    else if (hardware.HardwareType == HardwareType.CPU)
                    {
                        sensorList.Add(new SensorData(hardware.Name));
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                            {
                                //Console.WriteLine("Load :{0} {1} {2} {3}", sensor.Name, sensor.Value.Value, sensor.Min.Value, sensor.Max.Value);
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value.ToString("0.0") + " %",
                                    sensor.Min.Value.ToString("0.0") + " %", sensor.Max.Value.ToString("0.0") + " %"));
                            }
                            else if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                            {
                                //Console.WriteLine("{0}, Value={1}, Min Value={2}, Max Value={3}", sensor.Name, sensor.Value.Value, sensor.Min.Value, sensor.Max.Value);
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value + " °C",
                                    sensor.Min.Value + " °C", sensor.Max.Value + " °C"));
                            }
                            else if (sensor.SensorType == SensorType.Power && sensor.Value.HasValue)
                            {
                                //Console.WriteLine("Power :{0} {1} {2} {3}", sensor.Name, sensor.Value.Value, sensor.Min.Value, sensor.Max.Value);
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value.ToString("0.0") + " W",
                                    sensor.Min.Value.ToString("0.0") + " W", sensor.Max.Value.ToString("0.0") + " W"));
                            }
                        }
                    }
                    //获取内存使用率
                    else if (hardware.HardwareType == HardwareType.RAM)
                    {
                        sensorList.Add(new SensorData(hardware.Name));
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                            {
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value.ToString("0.0") + " %",
                                    sensor.Min.Value.ToString("0.0") + " %", sensor.Max.Value.ToString("0.0") + " %"));
                            }
                            else if (sensor.SensorType == SensorType.Data && sensor.Value.HasValue)
                            {
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value.ToString("0.0") + " GB",
                                    sensor.Min.Value.ToString("0.0") + " GB", sensor.Max.Value.ToString("0.0") + " GB"));
                            }
                        }
                    }
                    //获取硬盘温度、使用率
                    else if (hardware.HardwareType == HardwareType.HDD)
                    {
                        sensorList.Add(new SensorData(hardware.Name));
                        foreach (var sensor in hardware.Sensors)
                        {
                            if (sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
                            {
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value + " °C",
                                    sensor.Min.Value + " °C", sensor.Max.Value + " °C"));
                            }
                            else if (sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
                            {
                                sensorList.Add(new SensorData(sensor.Name, sensor.Value.Value.ToString("0.0") + " %",
                                    sensor.Min.Value.ToString("0.0") + " %", sensor.Max.Value.ToString("0.0") + " %"));
                            }
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return sensorList;
        }

        public void Dispose()
        {
            try
            {
                _computer.Close();
            }
            catch (Exception)
            {
                //ignore closing errors
            }
        }
    }
}
