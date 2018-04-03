using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.ServiceProcess;
using Microsoft.Win32;

namespace ClientInfoService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            this.AfterInstall += ServiceInstaller_AfterInstall;
            this.BeforeInstall += ServiceInstaller_BeforeInstall;
            InitializeComponent();
        }

        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController(serviceInstaller1.ServiceName))
            {
                sc.Start();
            }
        }

        void ServiceInstaller_BeforeInstall(object sender, InstallEventArgs e)
        {
            if (IsExistedService(serviceInstaller1.ServiceName))
            {
                using (ServiceController sc = new ServiceController(serviceInstaller1.ServiceName))
                {
                    if (sc.Status == ServiceControllerStatus.Running)
                    {
                        sc.Stop();
                    }
                    UnInstallService(sc.ServiceName);
                }
            }
        }

        /// <summary>
        /// 判断服务是否存在
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private bool IsExistedService(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController s in services)
            {
                if (s.ServiceName == serviceName)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 卸载服务
        /// </summary>
        /// <param name="filepath"></param>
        private void UnInstallService(string serviceName)
        {
            try
            {
                if (IsExistedService(serviceName))
                {
                    //UnInstall Service
                    AssemblyInstaller myAssemblyInstaller = new AssemblyInstaller
                    {
                        UseNewContext = true,
                        Path = GetWindowsServiceInstallPath(serviceName)
                    };
                    myAssemblyInstaller.Uninstall(null);
                    myAssemblyInstaller.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unInstallServiceError/n" + ex.Message);
            }
        }

        /// <summary>
        /// 获取服务安装路径
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static string GetWindowsServiceInstallPath(string serviceName)
        {
            string key = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            string path = Registry.LocalMachine.OpenSubKey(key)?.GetValue("ImagePath").ToString();
            //替换掉双引号   
            path = path.Replace("\"", string.Empty);
            FileInfo fi = new FileInfo(path);
            return fi.Directory.ToString();
        }

    }
}
