namespace BrainpackService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BrainpackProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BrainpackServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BrainpackProcessInstaller
            // 
            this.BrainpackProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BrainpackProcessInstaller.Password = null;
            this.BrainpackProcessInstaller.Username = null;
         //   this.BrainpackProcessInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.BrainpackProcessInstaller_AfterInstall);
            // 
            // BrainpackServiceInstaller
            // 
            this.BrainpackServiceInstaller.Description = "Allows the Heddoko app to communicate with the Heddoko brainpack";
            this.BrainpackServiceInstaller.ServiceName = "BrainpackService";
            this.BrainpackServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.BrainpackServiceInstaller.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.BrainpackServiceInstaller_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BrainpackProcessInstaller,
            this.BrainpackServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BrainpackProcessInstaller;
        private System.ServiceProcess.ServiceInstaller BrainpackServiceInstaller;
    }
}