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
    public partial class screenConsumerControl : ConsumerControlBase
    {
        public screenConsumerControl(screenConsumer consumer)
        {
            InitializeComponent();
            screenConsumerBindingSource.DataSource = consumer;
        }

        ~screenConsumerControl()
        {
            screenConsumerBindingSource.Dispose();
        }
    }
}
