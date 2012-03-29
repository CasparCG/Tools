using System.ServiceProcess;
using System.Threading;
using CasparCG.Conformer.Core;

namespace CasparCG.Conformer.Service
{
    public partial class Service : ServiceBase
    {
        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}
