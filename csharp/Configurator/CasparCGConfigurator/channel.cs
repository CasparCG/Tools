using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using TKLib;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class channel : INotifyPropertyChanged
    {
        private PropertyChangeManager<channel> propertyChanges;

        protected string[] videomodes = {"PAL","NTSC","1080i5000","576p2500","720p2500","720p5000","720p5994","720p6000","1080i5000","1080i5994","1080i6000","1080p2500","1080p2997","1080p3000","1080p5000"};

        private string _videomode;
        private BindingList<AConsumer> _consumers;

        public channel()
        {
            propertyChanges = new PropertyChangeManager<channel>(this);
            
            videomode = "PAL";
            consumers = new BindingList<AConsumer>();

        }

        [XmlElement(ElementName = "video-mode")]
        public string videomode
        {
            get { return _videomode; }
            set 
            {
                if (videomodes.Contains(value)){
                    _videomode = value;
                    this.propertyChanges.NotifyChanged(x => x.videomode); 
                }else{
                    System.Windows.Forms.MessageBox.Show("That video format <" + value.ToString() + "> is not supported.");
                }
            }
        }

        [XmlArray("consumers")]
        [XmlArrayItem("decklink", Type = typeof(decklinkConsumer))]
        [XmlArrayItem("screen", Type = typeof(screenConsumer))]
        public BindingList<AConsumer> consumers
        {
            get { return _consumers; }
            set { _consumers = value; this.propertyChanges.NotifyChanged(x => x.consumers);}
        }
        
        public event PropertyChangedEventHandler PropertyChanged
        {
            add { this.propertyChanges.AddHandler(value); }
            remove { this.propertyChanges.RemoveHandler(value); }
        }
    }
}
