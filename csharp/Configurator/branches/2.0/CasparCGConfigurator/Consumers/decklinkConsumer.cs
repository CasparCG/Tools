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

        public DecklinkConsumer(List<string> IDs)
        {
            this.device = IDs.First();
        }

        private String device ="1";
        [XmlElement(ElementName = "device")]
        public String Device
        {
            get { return this.device; }
            set { this.device = value; NotifyChanged("Device"); }
        }

        private Boolean embeddedaudio = false;
        [XmlElement(ElementName = "embedded-audio")]
        public Boolean EmbeddedAudio
        {
            get { return this.embeddedaudio; }
            set { this.embeddedaudio = value; NotifyChanged("EmbeddedAudio"); }
        }

        private string latency = "normal";
        [XmlElement(ElementName = "latency")]
        public string Latency
        {
            get { return this.latency; }
            set { this.latency = value; NotifyChanged("Latency"); }
        }

        private string keyer = "external";
        [XmlElement(ElementName = "keyer")]
        public string Keyer
        {
            get { return this.keyer; }
            set { this.keyer = value; NotifyChanged("Keyer"); }
        }

        private Boolean keyonly = false;
        [XmlElement(ElementName = "key-only")]
        public Boolean KeyOnly
        {
            get { return this.keyonly; }
            set { this.keyonly = value; NotifyChanged("KeyOnly"); }
        }

        private String bufferdepth = "3";
        [XmlElement(ElementName = "buffer-depth")]
        public String BufferDepth
        {
            get { return this.bufferdepth; }
            set { this.bufferdepth = value; NotifyChanged("BufferDepth"); }
        }

        public override String ToString()
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
