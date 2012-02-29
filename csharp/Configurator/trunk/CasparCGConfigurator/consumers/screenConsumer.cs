using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CasparCGConfigurator
{
    public class screenConsumer : AbstractConsumer, INotifyPropertyChanged
    {
        private int _device;
        private string _name;
        private string _aspectratio;
        private string _stretch;
        private Boolean _windowed;
        private Boolean _keyonly;
        private Boolean _autodeinterlace;
        private Boolean _vsync;

        public screenConsumer()
        {
            device = 0;
            aspectratio = "default";
            stretch = "fill";
            windowed = true;
            keyonly = false;
            autodeinterlace = true;
            vsync = false;
        }

        public int device
        {
            get { return _device; }
            set { _device = value; NotifyChanged("device"); }
        }

        public string name
        {
            get { return _name; }
            set { _name = value; NotifyChanged("name"); }
        }

        [XmlElement(ElementName = "aspect-ratio")]
        public string aspectratio
        {
            get { return _aspectratio; }
            set { _aspectratio = value; NotifyChanged("aspectratio"); }
        }

        public string stretch
        {
            get { return _stretch; }
            set { _stretch = value; NotifyChanged("stretch"); }
        }

        public Boolean windowed
        {
            get { return _windowed; }
            set { _windowed = value; NotifyChanged("windowed"); }
        }

        [XmlElement(ElementName = "key-only")]
        public Boolean keyonly
        {
            get { return _keyonly; }
            set { _keyonly = value; NotifyChanged("keyonly"); }
        }

        [XmlElement(ElementName = "auto-deinterlace")]
        public Boolean autodeinterlace
        {
            get { return _autodeinterlace; }
            set { _autodeinterlace = value; NotifyChanged("autodeinterlace"); }
        }

        public Boolean vsync
        {
            get { return _vsync; }
            set { _vsync = value; NotifyChanged("vsync"); }
        }

        public override string ToString()
        {
            return "Screen";
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
