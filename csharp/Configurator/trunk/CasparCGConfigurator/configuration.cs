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
        public configuration()
        {
        }

        private Paths path = new Paths();
        public Paths Paths
        {
            get { return this.path; }
            set { this.path = value; NotifyChanged("paths"); }
        }

        private string logLevel = "trace"; // [trace|debug|info|warning|error]</log-level>
        [XmlElement(ElementName = "log-level")]
        public string LogLevel
        {
            get { return this.logLevel; }
            set { this.logLevel = value; NotifyChanged("loglevel"); }
        }

        private Boolean channelGrid = false;
        [XmlElement(ElementName = "channel-grid")]
        public Boolean ChannelGrid
        {
            get { return this.channelGrid; }
            set { this.channelGrid = value; NotifyChanged("channelgrid"); }
        }

        private Boolean blendModes = true;
        [XmlElement(ElementName = "blend-modes")]
        public Boolean BlendModes
        {
            get { return this.blendModes; }
            set { this.blendModes = value; NotifyChanged("blendmodes"); }
        }

        private Boolean autoDeinterlace = false;
        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean AutoDeinterlace
        {
            get { return this.autoDeinterlace; }
            set { this.autoDeinterlace = value; NotifyChanged("autodeinterlace"); }
        }

        private Boolean autoTranscode = true;
        [XmlElement(ElementName = "auto-transcode")]
        public Boolean AutoTranscode
        {
            get { return this.autoTranscode; }
            set { this.autoTranscode = value; NotifyChanged("autotranscode"); }
        }

        private int pipelineTokens = 2;
        [XmlElement(ElementName = "pipeline-tokens")]
        public int PipelineTokens
        {
            get { return this.pipelineTokens; }
            set { this.pipelineTokens = value; NotifyChanged("pipelinetokens"); }
        }

        private BindingList<Channel> channels = new BindingList<Channel>();
        public BindingList<Channel> Channels
        {
            get { return this.channels; }
            set { this.channels = value; NotifyChanged("channels"); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyChanged(String info)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
