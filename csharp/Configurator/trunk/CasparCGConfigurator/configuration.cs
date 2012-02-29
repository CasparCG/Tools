using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TKLib;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class configuration : INotifyPropertyChanged
    {
        private PropertyChangeManager<configuration> propertyChanges;

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
            propertyChanges = new PropertyChangeManager<configuration>(this);
            
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
            set { _paths = value; this.propertyChanges.NotifyChanged(x => x.paths); }
        }

        [XmlElement(ElementName = "log-level")]
        public string loglevel
        {
            get { return _loglevel; }
            set { _loglevel = value; this.propertyChanges.NotifyChanged(x => x.loglevel); }
        }

        [XmlElement(ElementName = "channel-grid")]
        public Boolean channelgrid
        {
            get { return _channelgrid; }
            set { _channelgrid = value; this.propertyChanges.NotifyChanged(x => x.channelgrid); }
        }

        [XmlElement(ElementName = "blend-modes")]
        public Boolean blendmodes
        {
            get { return _blendmodes; }
            set { _blendmodes = value; this.propertyChanges.NotifyChanged(x => x.blendmodes); }
        }

        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean autodeinterlace
        {
            get { return _autodeinterlace; }
            set { _autodeinterlace = value; this.propertyChanges.NotifyChanged(x => x.autodeinterlace); }
        }

        [XmlElement(ElementName = "auto-transcode")]
        public Boolean autotranscode
        {
            get { return _autotranscode; }
            set { _autotranscode = value; this.propertyChanges.NotifyChanged(x => x.autotranscode); }
        }

        [XmlElement(ElementName = "pipeline-tokens")]
        public int pipelinetokens
        {
            get { return _pipelinetokens; }
            set { _pipelinetokens = value; this.propertyChanges.NotifyChanged(x => x.pipelinetokens); }
        }

        public BindingList<channel> channels
        {
            get { return _channels; }
            set { _channels = value; this.propertyChanges.NotifyChanged(x => x.channels); }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add { this.propertyChanges.AddHandler(value); }
            remove { this.propertyChanges.RemoveHandler(value); }
        }
    }
}
