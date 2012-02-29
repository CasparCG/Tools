using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class configuration : INotifyPropertyChanged
    {
        private paths _paths;
        private BindingList<channel> _channels;
        private string _loglevel; // [trace|debug|info|warning|error]</log-level>
        private Boolean _channelgrid;
        private Boolean _blendmodes;
        private Boolean _autodeinterlace;
        private Boolean _autotranscode;
        private int _pipelinetokens;

        public configuration()
        {

            paths = new paths();
            loglevel = "trace";
            channelgrid = false;
            blendmodes = false;
            autodeinterlace = true;
            autotranscode = true;
            pipelinetokens = 2;
            channels = new BindingList<channel>();
        }

        public paths paths
        {
            get { return _paths; }
            set { _paths = value; NotifyChanged("paths"); }
        }

        [XmlElement(ElementName = "log-level")]
        public string loglevel
        {
            get { return _loglevel; }
            set { _loglevel = value; NotifyChanged("loglevel"); }
        }

        [XmlElement(ElementName = "channel-grid")]
        public Boolean channelgrid
        {
            get { return _channelgrid; }
            set { _channelgrid = value; NotifyChanged("channelgrid"); }
        }

        [XmlElement(ElementName = "blend-modes")]
        public Boolean blendmodes
        {
            get { return _blendmodes; }
            set { _blendmodes = value; NotifyChanged("blendmodes"); }
        }

        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean autodeinterlace
        {
            get { return _autodeinterlace; }
            set { _autodeinterlace = value; NotifyChanged("autodeinterlace"); }
        }

        [XmlElement(ElementName = "auto-transcode")]
        public Boolean autotranscode
        {
            get { return _autotranscode; }
            set { _autotranscode = value; NotifyChanged("autotranscode"); }
        }

        [XmlElement(ElementName = "pipeline-tokens")]
        public int pipelinetokens
        {
            get { return _pipelinetokens; }
            set { _pipelinetokens = value; NotifyChanged("pipelinetokens"); }
        }

        public BindingList<channel> channels
        {
            get { return _channels; }
            set { _channels = value; NotifyChanged("channels"); }
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
