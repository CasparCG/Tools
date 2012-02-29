using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TKLib;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class decklinkConsumer : AConsumer, INotifyPropertyChanged
    {
        private PropertyChangeManager<decklinkConsumer> propertyChanges;

        private int _device;
        private Boolean _embeddedaudio;
        private string _latency;
        private string _keyer;
        private Boolean _keyonly;
        private int _bufferdepth;

        public decklinkConsumer()
        {
            propertyChanges = new PropertyChangeManager<decklinkConsumer>(this);
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
            set { _device = value; this.propertyChanges.NotifyChanged(x => x.device); }
        }

        [XmlElement(ElementName = "embedded-audio")]
        public Boolean embeddedaudio
        {
            get { return _embeddedaudio; }
            set { _embeddedaudio = value; this.propertyChanges.NotifyChanged(x => x.embeddedaudio); }
        }

        public string latency
        {
            get { return _latency; }
            set { _latency = value; this.propertyChanges.NotifyChanged(x => x.latency); }
        }

        public string keyer
        {
            get { return _keyer; }
            set { _keyer = value; this.propertyChanges.NotifyChanged(x => x.keyer); }
        }

        [XmlElement(ElementName = "key-only")]
        public Boolean keyonly
        {
            get { return _keyonly; }
            set { _keyonly = value; this.propertyChanges.NotifyChanged(x => x.keyonly); }
        }

        [XmlElement(ElementName = "buffer-depth")]
        public int bufferdepth
        {
            get { return _bufferdepth; }
            set { _bufferdepth = value; this.propertyChanges.NotifyChanged(x => x.bufferdepth); }
        }

        public override string ToString()
        {
            return "Decklink";
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { this.propertyChanges.AddHandler(value); }
            remove { this.propertyChanges.RemoveHandler(value); }
        }
    }

    public class screenConsumer : AConsumer, INotifyPropertyChanged
    {
        private PropertyChangeManager<screenConsumer> propertyChanges;

        private int _device;

        public screenConsumer()
        {
            propertyChanges = new PropertyChangeManager<screenConsumer>(this);
        }

        public int device
        {
            get { return _device; }
            set { _device = value; this.propertyChanges.NotifyChanged(x => x.device); }
        }

        public override string ToString()
        {
            return "Screen";
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { this.propertyChanges.AddHandler(value); }
            remove { this.propertyChanges.RemoveHandler(value); }
        }
    }

    [XmlInclude(typeof(decklinkConsumer))]
    [XmlInclude(typeof(screenConsumer))]
    public abstract class AConsumer : INotifyPropertyChanged
    {

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }
    }
}
