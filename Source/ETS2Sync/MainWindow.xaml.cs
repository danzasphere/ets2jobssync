using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Media;
using System.Diagnostics;
using System.Deployment.Application;
using System.Reflection;

namespace ETS2Sync
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public JobSyncer JobsSyncer { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            this.Init();
        }


        public void Init()
        {
            this.labelVersion.Content = "Version: " + this.Version();

            this.JobsSyncer = new JobSyncer();


            this.checkBoxGoingEast.IsChecked = this.JobsSyncer.DlcGoingEast;
            this.checkBoxHighPowerCargoes.IsChecked = this.JobsSyncer.DlcHighPowerCargoes;
            this.checkBoxScandinavia.IsChecked = this.JobsSyncer.DlcScandinavia;

            this.textBoxSyncSaveGamePath.Text = this.JobsSyncer.SaveFolderPath;


            this.checkBoxSound.IsChecked = Rn.Config.General.PlaySound;
            this.checkBoxBackup.IsChecked = Rn.Config.General.BackupSaveGame;

            Update s = new Update();
            s.Start();


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            JobsSyncer.SaveFolderPath = this.textBoxSyncSaveGamePath.Text;
            JobsSyncer.SyncJobs();



        }

        private void textBoxSyncSaveGamePath_TextChanged(object sender, TextChangedEventArgs e)
        {

            this.JobsSyncer.SaveFolderPath = this.textBoxSyncSaveGamePath.Text;
            this.JobsSyncer.StartFileWatcher();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        public String Version()
        {
            string version = null;
            try
            {
                //// get deployment version
                version = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            catch (InvalidDeploymentException)
            {
                //// you cannot read publish version when app isn't installed 
                //// (e.g. during debug)
                version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }

            return version;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // 
            Process.Start(new ProcessStartInfo("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=KS66AEL8LGSCY"));
            e.Handled = true;
        }

        private void checkBoxSound_Click(object sender, RoutedEventArgs e)
        {
            Rn.Config.General.PlaySound = Convert.ToBoolean(this.checkBoxSound.IsChecked);
        }

        private void checkBoxBackup_Click(object sender, RoutedEventArgs e)
        {
            Rn.Config.General.BackupSaveGame = Convert.ToBoolean(this.checkBoxBackup.IsChecked);
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ETS2SyncConfig.SaveConfig(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Rn.Config);
        }

        private void checkBoxScandinavia_Click(object sender, RoutedEventArgs e)
        {
            this.JobsSyncer.DlcScandinavia = Convert.ToBoolean(this.checkBoxScandinavia.IsChecked);
        }

        private void checkBoxGoingEast_Click(object sender, RoutedEventArgs e)
        {
            this.JobsSyncer.DlcGoingEast = Convert.ToBoolean(this.checkBoxGoingEast.IsChecked);
        }

        private void checkBoxHighPowerCargoes_Click(object sender, RoutedEventArgs e)
        {
            this.JobsSyncer.DlcHighPowerCargoes = Convert.ToBoolean(this.checkBoxHighPowerCargoes.IsChecked);
        }
    }
}
