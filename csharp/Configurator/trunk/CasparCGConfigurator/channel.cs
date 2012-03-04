using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class Channel : INotifyPropertyChanged
    {
        public Channel()
        {            
            Consumers.ListChanged += new ListChangedEventHandler(Consumers_ListChanged);
        }

        private void Consumers_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyChanged("Consumers");
        }

        private string videoMode = "PAL";
        [XmlElement(ElementName = "video-mode")]
        public string VideoMode
        {
            get { return this.videoMode; }
            set { this.videoMode = value; NotifyChanged("VideoMode"); }
        }

        private BindingList<AbstractConsumer> consumers = new BindingList<AbstractConsumer>();
        [XmlArray("consumers")]
        [XmlArrayItem("decklink", Type = typeof(DecklinkConsumer))]
        [XmlArrayItem("screen", Type = typeof(ScreenConsumer))]
        [XmlArrayItem("system-audio", Type = typeof(SystemAudioConsumer))]
        [XmlArrayItem("bluefish", Type = typeof(BluefishConsumer))]
        public BindingList<AbstractConsumer> Consumers
        {
            get { return this.consumers; }
            set { this.consumers = value; NotifyChanged("Consumers");}
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate {};

        private void NotifyChanged(String info)
        {        
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
