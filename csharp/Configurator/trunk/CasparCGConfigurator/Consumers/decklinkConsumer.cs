using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;


namespace CasparCGConfigurator
{
    public class DecklinkConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        public DecklinkConsumer()
        {
        }

        private int device = 1;
        public int Device
        {
            get { return this.device; }
            set { this.device = value; NotifyChanged("device"); }
        }

        private Boolean embeddedaudio = false;
        [XmlElement(ElementName = "embedded-audio")]
        public Boolean EmbeddedAudio
        {
            get { return this.embeddedaudio; }
            set { this.embeddedaudio = value; NotifyChanged("embeddedaudio"); }
        }

        private string latency = "normal";
        public string Latency
        {
            get { return this.latency; }
            set { this.latency = value; NotifyChanged("latency"); }
        }

        private string keyer = "external";
        public string Keyer
        {
            get { return this.keyer; }
            set { this.keyer = value; NotifyChanged("keyer"); }
        }

        private Boolean keyonly = false;
        [XmlElement(ElementName = "key-only")]
        public Boolean KeyOnly
        {
            get { return this.keyonly; }
            set { this.keyonly = value; NotifyChanged("keyonly"); }
        }

        private int bufferdepth = 3;
        [XmlElement(ElementName = "buffer-depth")]
        public int BufferDepth
        {
            get { return this.bufferdepth; }
            set { this.bufferdepth = value; NotifyChanged("bufferdepth"); }
        }

        public override string ToString()
        {
            return "Decklink";
        }

        public override event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyChanged(String info)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
