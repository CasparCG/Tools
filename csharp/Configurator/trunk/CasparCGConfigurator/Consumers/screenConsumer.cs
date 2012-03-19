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

        private String device = "1";
        [XmlElement(ElementName = "device")]
        public String Device
        {
            get { return this.device; }
            set { this.device = value; NotifyChanged("Device"); }
        }

        private string name;
        [XmlElement(ElementName = "name")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; NotifyChanged("Name"); }
        }

        private string aspectratio = "default";
        [XmlElement(ElementName = "aspect-ratio")]
        public string AspectRatio
        {
            get { return this.aspectratio; }
            set { this.aspectratio = value; NotifyChanged("AspectRatio"); }
        }

        private string stretch = "fill";
        [XmlElement(ElementName = "stretch")]
        public string Stretch
        {
            get { return this.stretch; }
            set { this.stretch = value; NotifyChanged("Stretch"); }
        }

        private Boolean windowed = true;
        [XmlElement(ElementName = "windowed")]
        public Boolean Windowed
        {
            get { return this.windowed; }
            set { this.windowed = value; NotifyChanged("Windowed"); }
        }

        private Boolean keyonly = false;
        [XmlElement(ElementName = "key-only")]
        public Boolean KeyOnly
        {
            get { return this.keyonly; }
            set { this.keyonly = value; NotifyChanged("KeyOnly"); }
        }

        private Boolean autodeinterlace = true;
        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean AutoDeinterlace
        {
            get { return this.autodeinterlace; }
            set { this.autodeinterlace = value; NotifyChanged("AutoDeinterlace"); }
        }

        private Boolean vsync = true;
        [XmlElement(ElementName = "vsync")]
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
