using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QTest.instances
{
    public class BaseData : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Image { get; set; }
        string _value;

        public event PropertyChangedEventHandler PropertyChanged;

        public BaseData(string name)
        {
            Name = name;
        }

        public BaseData(string name, string image, int i)
        {
            Name = name;
            Image = image;
        }

        public BaseData(string name, string value)
        {
            this.Name = name;
            this._value = value;
        }

        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                //动态更新数据
                OnPropertyChanged("Value");
            }
        }

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
