using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    [XmlInclude(typeof(decklinkConsumer))]
    [XmlInclude(typeof(screenConsumer))]
    [XmlInclude(typeof(systemaudioConsumer))]
    [XmlInclude(typeof(bluefishConsumer))]
    public abstract class AbstractConsumer : INotifyPropertyChanged
    {

        public virtual event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
