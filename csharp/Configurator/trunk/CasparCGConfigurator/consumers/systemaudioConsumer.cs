using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class systemaudioConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        [XmlElement(ElementName = "system-audio")]
        public systemaudioConsumer()
        {

        }

        public override string ToString()
        {
            return "System Audio";
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
