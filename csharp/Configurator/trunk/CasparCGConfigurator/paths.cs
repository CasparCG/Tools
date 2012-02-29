using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class paths : INotifyPropertyChanged
    {
        private string _mediapath;
        private string _logpath;
        private string _datapath;
        private string _templatepath;

        public paths()
        {
            
            mediapath = "media\\";
            logpath = "log\\";
            datapath = "data\\";
            templatepath = "templates\\";
        }

        [XmlElement(ElementName = "media-path")]
        public string mediapath
        {
            get { return _mediapath; }
            set { _mediapath = value; NotifyChanged("mediapath"); }
        }

        [XmlElement(ElementName = "log-path")]
        public string logpath
        {
            get { return _logpath; }
            set { _logpath = value; NotifyChanged("logpath"); }
        }

        [XmlElement(ElementName = "data-path")]
        public string datapath
        {
            get { return _datapath; }
            set { _datapath = value; NotifyChanged("datapath"); }
        }

        [XmlElement(ElementName = "template-path")]
        public string templatepath
        {
            get { return _templatepath; }
            set { _templatepath = value; NotifyChanged("templatepath"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
