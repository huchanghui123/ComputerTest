using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QTest.Tools
{
    public sealed class WatchDogManager
    {
        private DispatcherTimer timer;
        private SuperIO watchDog;
        private static readonly WatchDogManager instance = new WatchDogManager();

        static WatchDogManager() { }

        private WatchDogManager() { }

        public static WatchDogManager Instance
        {
            get
            {
                return instance;
            }
        }

        public DispatcherTimer Timer { get => timer; set => timer = value; }
        public SuperIO WatchDog { get => watchDog; set => watchDog = value; }
    }
}
