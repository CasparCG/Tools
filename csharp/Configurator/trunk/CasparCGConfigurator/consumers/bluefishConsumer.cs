using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;


namespace CasparCGConfigurator
{
    public class bluefishConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        private int _device;
        private Boolean _embeddedaudio;
        private Boolean _keyonly;

        public bluefishConsumer()
        {
            device = 1;
            embeddedaudio = false;
            keyonly = false;
;
        }

        public int device
        {
            get { return _device; }
            set { _device = value; NotifyChanged("device"); }
        }

        [XmlElement(ElementName = "embedded-audio")]
        public Boolean embeddedaudio
        {
            get { return _embeddedaudio; }
            set { _embeddedaudio = value; NotifyChanged("embeddedaudio"); }
        }

        [XmlElement(ElementName = "key-only")]
        public Boolean keyonly
        {
            get { return _keyonly; }
            set { _keyonly = value; NotifyChanged("keyonly"); }
        }

        public override string ToString()
        {
            return "Bluefish";
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
