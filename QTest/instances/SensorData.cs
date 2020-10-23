using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QTest.instances
{
    public class SensorData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Image { get; set; }
        string _value;
        string _minValue;
        string _maxValue;

        public event PropertyChangedEventHandler PropertyChanged;

        public SensorData(string name)
        {
            Name = name;
        }

        public SensorData(string name, string image)
        {
            Name = name;
            Image = image;
        }

        public SensorData(string value, string minValue, string maxValue)
        {
            _value = value;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public SensorData(string name, string value, string minValue, string maxValue)
        {
            Name = name;
            _value = value;
            _minValue = minValue;
            _maxValue = maxValue;
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                OnPropertyChanged("Value");
            }
        }
        public string MinValue
        {
            get { return _minValue; }
            set
            {
                _minValue = value;
                OnPropertyChanged("MinValue");
            }
        }
        public string MaxValue
        {
            get { return _maxValue; }
            set
            {
                _maxValue = value;
                OnPropertyChanged("MaxValue");
            }
        }

        //动态更新数据
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                //Console.WriteLine("OnPropertyChanged:{0}", propertyName);
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        
    }
    
}
