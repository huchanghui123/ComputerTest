using System.ComponentModel;

namespace QTest.instances
{
    public class NetWork : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string NetDevice { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }

        string _value;

        public NetWork(string device, string image)
        {
            NetDevice = device;
            Image = image;
        }

        public NetWork(string device, string image, string value)
        {
            NetDevice = device;
            Image = image;
            _value = value;
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

        //动态更新数据
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}
