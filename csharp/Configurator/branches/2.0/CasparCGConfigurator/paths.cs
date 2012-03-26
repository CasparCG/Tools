using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class Paths : INotifyPropertyChanged
    {
        public Paths()
        {            
        }

        private string mediaPath = "media\\";
        [XmlElement(ElementName = "media-path")]
        public string MediaPath
        {
            get { return this.mediaPath; }
            set { this.mediaPath = value; NotifyChanged("MediaPath"); }
        }

        private string logPath = "log\\";
        [XmlElement(ElementName = "log-path")]
        public string LogPath
        {
            get { return this.logPath; }
            set { this.logPath = value; NotifyChanged("LogPath"); }
        }

        private string dataPath = "data\\";
        [XmlElement(ElementName = "data-path")]
        public string DataPath
        {
            get { return this.dataPath; }
            set { this.dataPath = value; NotifyChanged("datapath"); }
        }

        private string templatePath = "templates\\";
        [XmlElement(ElementName = "template-path")]
        public string TemplatePath
        {
            get { return this.templatePath; }
            set { this.templatePath = value; NotifyChanged("TemplatePath"); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate {};

        private void NotifyChanged(String info)
        {           
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
