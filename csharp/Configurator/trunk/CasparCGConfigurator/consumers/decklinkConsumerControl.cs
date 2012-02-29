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
    public partial class decklinkConsumerControl : ConsumerControlBase
    {
        public decklinkConsumerControl(decklinkConsumer consumer)
        {
            InitializeComponent();
            decklinkConsumerBindingSource.DataSource = consumer;
        }

        ~decklinkConsumerControl()
        {
            decklinkConsumerBindingSource.Dispose();
        }
    }
}
