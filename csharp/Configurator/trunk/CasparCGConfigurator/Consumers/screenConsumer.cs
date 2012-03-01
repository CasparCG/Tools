using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class ScreenConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        public ScreenConsumer()
        {
        }

        private int device = 0;
        public int Device
        {
            get { return this.device; }
            set { this.device = value; NotifyChanged("device"); }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set { this.name = value; NotifyChanged("name"); }
        }

        private string aspectratio = "default";
        [XmlElement(ElementName = "aspect-ratio")]
        public string AspectRatio
        {
            get { return this.aspectratio; }
            set { this.aspectratio = value; NotifyChanged("aspectratio"); }
        }

        private string stretch = "fill";
        public string Stretch
        {
            get { return this.stretch; }
            set { this.stretch = value; NotifyChanged("stretch"); }
        }

        private Boolean windowed = true;
        public Boolean Windowed
        {
            get { return this.windowed; }
            set { this.windowed = value; NotifyChanged("windowed"); }
        }

        private Boolean keyonly = false;
        [XmlElement(ElementName = "key-only")]
        public Boolean KeyOnly
        {
            get { return this.keyonly; }
            set { this.keyonly = value; NotifyChanged("keyonly"); }
        }

        private Boolean autodeinterlace = true;
        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean AutoDeinterlace
        {
            get { return this.autodeinterlace; }
            set { this.autodeinterlace = value; NotifyChanged("autodeinterlace"); }
        }

        private Boolean vsync = false;
        public Boolean VSync
        {
            get { return this.vsync; }
            set { this.vsync = value; NotifyChanged("vsync"); }
        }

        public override string ToString()
        {
            return "Screen";
        }

        public override event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void NotifyChanged(String info)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(info));            
        }
    }
}
