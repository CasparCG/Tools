using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;


namespace CasparCGConfigurator
{
    public class BluefishConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        public BluefishConsumer()
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

        private Boolean keyonly = false;
        [XmlElement(ElementName = "key-only")]
        public Boolean KeyOnly
        {
            get { return this.keyonly; }
            set { this.keyonly = value; NotifyChanged("keyonly"); }
        }

        public override string ToString()
        {
            return "Bluefish";
        }

        public override event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyChanged(String info)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
