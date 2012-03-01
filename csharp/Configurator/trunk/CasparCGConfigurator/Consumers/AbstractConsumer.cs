using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    [XmlInclude(typeof(DecklinkConsumer))]
    [XmlInclude(typeof(ScreenConsumer))]
    [XmlInclude(typeof(SystemAudioConsumer))]
    [XmlInclude(typeof(BluefishConsumer))]
    public abstract class AbstractConsumer : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate{};

        private void NotifyChanged(String info)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
