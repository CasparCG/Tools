using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;


namespace CasparCGConfigurator
{
    public class decklinkConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        private int _device;
        private Boolean _embeddedaudio;
        private string _latency;
        private string _keyer;
        private Boolean _keyonly;
        private int _bufferdepth;

        public decklinkConsumer()
        {
            device = 1;
            embeddedaudio = false;
            latency = "normal";
            keyer = "external";
            keyonly = false;
            bufferdepth = 3;
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

        public string latency
        {
            get { return _latency; }
            set { _latency = value; NotifyChanged("latency"); }
        }

        public string keyer
        {
            get { return _keyer; }
            set { _keyer = value; NotifyChanged("keyer"); }
        }

        [XmlElement(ElementName = "key-only")]
        public Boolean keyonly
        {
            get { return _keyonly; }
            set { _keyonly = value; NotifyChanged("keyonly"); }
        }

        [XmlElement(ElementName = "buffer-depth")]
        public int bufferdepth
        {
            get { return _bufferdepth; }
            set { _bufferdepth = value; NotifyChanged("bufferdepth"); }
        }

        public override string ToString()
        {
            return "Decklink";
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
