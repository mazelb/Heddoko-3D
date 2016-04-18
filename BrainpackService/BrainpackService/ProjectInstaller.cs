using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace BrainpackService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
            this.AfterInstall += new InstallEventHandler(ServiceInstaller_AfterInstall);
        }


     /**
     * OnBeforeInstall 
     * @brief OnBeforeInstall called right before the actual installation of the service. Sets parameters for event logs  
     */ 
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            string parameter = "BrainpackSource\" \"BrainpackLog";
            Context.Parameters["assemblypath"] = "\"" + Context.Parameters["assemblypath"] + "\"\"" + parameter + "\"";
            base.OnBeforeInstall(savedState);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);
            //automatically start the service as soon as its installed
            //ServiceController sc = new ServiceController("BrainpackService");
           // sc.Start();
           //todo: the service controller was messing up. come back here to see why
        }

        private void BrainpackServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {

            using (ServiceController sc = new ServiceController(this.BrainpackServiceInstaller.ServiceName))
            {
                sc.Start();
            }
        }

        private void BrainpackProcessInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
        }
        void ServiceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            
        }
    }
}
