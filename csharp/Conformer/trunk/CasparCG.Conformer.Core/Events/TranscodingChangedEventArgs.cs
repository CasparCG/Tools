using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CasparCG.Conformer.Core.Events
{
    public class TranscodingChangedEventArgs : EventArgs
    {
        public Dictionary<string, string> Items { get; set; }
    }
}
