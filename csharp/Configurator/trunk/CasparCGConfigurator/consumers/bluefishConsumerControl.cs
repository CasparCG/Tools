using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CasparCGConfigurator
{
    public partial class bluefishConsumerControl : ConsumerControlBase
    {
        public bluefishConsumerControl(bluefishConsumer consumer)
        {
            InitializeComponent();
            bluefishConsumerBindingSource.DataSource = consumer;
        }

        ~bluefishConsumerControl()
        {
            bluefishConsumerBindingSource.Dispose();
        }
    }
}
