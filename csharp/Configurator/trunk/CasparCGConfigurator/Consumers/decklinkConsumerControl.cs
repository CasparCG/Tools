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
    public partial class DecklinkConsumerControl : ConsumerControlBase
    {
        public DecklinkConsumerControl(DecklinkConsumer consumer)
        {
            InitializeComponent();
            decklinkConsumerBindingSource.DataSource = consumer;
        }

        ~DecklinkConsumerControl()
        {
            decklinkConsumerBindingSource.Dispose();
        }
    }
}
