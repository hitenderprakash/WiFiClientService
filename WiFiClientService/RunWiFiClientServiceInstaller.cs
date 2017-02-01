using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace WiFiClientService
{
    [RunInstaller(true)]
    public partial class RunWiFiClientServiceInstaller : System.Configuration.Install.Installer
    {
        public RunWiFiClientServiceInstaller()
        {
            InitializeComponent();
        }

        private void RunWiFiClientServiceProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }

        private void RunWiFiClientServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

        }
    }
}
