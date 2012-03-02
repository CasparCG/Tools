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

        private Paths paths = new Paths();
        [XmlElement(ElementName = "paths")]
        public Paths Paths
        {
            get { return this.paths; }
            set { this.paths = value; NotifyChanged("Paths"); }
        }

        private string logLevel = "trace";
        [XmlElement(ElementName = "log-level")]
        public string LogLevel
        {
            get { return this.logLevel; }
            set { this.logLevel = value; NotifyChanged("LogLevel"); }
        }

        private Boolean channelGrid = false;
        [XmlElement(ElementName = "channel-grid")]
        public Boolean ChannelGrid
        {
            get { return this.channelGrid; }
            set { this.channelGrid = value; NotifyChanged("ChannelGrid"); }
        }

        private Boolean blendModes = true;
        [XmlElement(ElementName = "blend-modes")]
        public Boolean BlendModes
        {
            get { return this.blendModes; }
            set { this.blendModes = value; NotifyChanged("BlendModes"); }
        }

        private Boolean autoDeinterlace = true;
        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean AutoDeinterlace
        {
            get { return this.autoDeinterlace; }
            set { this.autoDeinterlace = value; NotifyChanged("AutoDeinterlace"); }
        }

        private Boolean autoTranscode = true;
        [XmlElement(ElementName = "auto-transcode")]
        public Boolean AutoTranscode
        {
            get { return this.autoTranscode; }
            set { this.autoTranscode = value; NotifyChanged("AutoTranscode"); }
        }

        private int pipelineTokens = 2;
        [XmlElement(ElementName = "pipeline-tokens")]
        public int PipelineTokens
        {
            get { return this.pipelineTokens; }
            set { this.pipelineTokens = value; NotifyChanged("PipelineTokens"); }
        }

        private BindingList<Channel> channels = new BindingList<Channel>();
        [XmlArray("channels")]
        [XmlArrayItem("channel", Type = typeof(Channel))]
        public BindingList<Channel> Channels
        {
            get { return this.channels; }
            set { this.channels = value; NotifyChanged("Channels"); }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyChanged(String info)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
