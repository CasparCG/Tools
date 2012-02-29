using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TKLib;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class paths : INotifyPropertyChanged
    {
        private PropertyChangeManager<paths> propertyChanges;

        private string _mediapath;
        private string _logpath;
        private string _datapath;
        private string _templatepath;

        public paths()
        {
            propertyChanges = new PropertyChangeManager<paths>(this);
            
            mediapath = "media\\";
            logpath = "log\\";
            datapath = "data\\";
            templatepath = "templates\\";
        }

        [XmlElement(ElementName = "media-path")]
        public string mediapath
        {
            get { return _mediapath; }
            set { _mediapath = value; this.propertyChanges.NotifyChanged(x => x.mediapath); }
        }

        [XmlElement(ElementName = "log-path")]
        public string logpath
        {
            get { return _logpath; }
            set { _logpath = value; this.propertyChanges.NotifyChanged(x => x.logpath); }
        }

        [XmlElement(ElementName = "data-path")]
        public string datapath
        {
            get { return _datapath; }
            set { _datapath = value; this.propertyChanges.NotifyChanged(x => x.datapath); }
        }

        [XmlElement(ElementName = "template-path")]
        public string templatepath
        {
            get { return _templatepath; }
            set { _templatepath = value; this.propertyChanges.NotifyChanged(x => x.templatepath); }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { this.propertyChanges.AddHandler(value); }
            remove { this.propertyChanges.RemoveHandler(value); }
        }
    }
}
