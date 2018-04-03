using System.ServiceProcess;

namespace ClientInfoService
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ClientInfo.Start();
        }

        protected override void OnStop()
        {
        }

    }
}
