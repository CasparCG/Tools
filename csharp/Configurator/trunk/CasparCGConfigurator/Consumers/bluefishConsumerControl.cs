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
    public partial class BluefishConsumerControl : ConsumerControlBase
    {
        public BluefishConsumerControl(BluefishConsumer consumer)
        {
            InitializeComponent();
            bluefishConsumerBindingSource.DataSource = consumer;
        }

        ~BluefishConsumerControl()
        {
            bluefishConsumerBindingSource.Dispose();
        }
    }
}
