using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CasparCGConfigurator
{
    public class screenConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        private int _device;

        public screenConsumer()
        {
            device = 0;
        }

        public int device
        {
            get { return _device; }
            set { _device = value; NotifyChanged("device"); }
        }

        public override string ToString()
        {
            return "Screen";
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
