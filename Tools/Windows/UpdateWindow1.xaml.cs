using System;
using System.ComponentModel;
using System.Deployment.Application;
using System.Net;
using System.Windows;
using Telerik.Windows.Controls;
using Tools.CustomException;
using Tools.Outils;

namespace Tools.Windows
{
    /// <summary>
    /// Logique d'interaction pour ChangeLogWindow.xaml
    /// </summary>
    public partial class UpdateWindow1 : RadWindow
    {
        public UpdateWindow1()
        {
            InitializeComponent();
            OutilsTools.ShowInTaskbar(this);
            this.DialogResult = false;

            this.Header = "Mise à jour de l\'application    ";
            LabelMessage.Text = "Vérification de la connection.";
            ButOk.Visibility = Visibility.Collapsed;
            ButAnnuler.Visibility = Visibility.Collapsed;
            ButDownOui.Visibility = Visibility.Collapsed;
            ButDownMan.Visibility = Visibility.Collapsed;
            ButDownNon.Visibility = Visibility.Collapsed;
            ButDownOK.Visibility = Visibility.Collapsed;
        }


        private void ButOk_Click(object sender, RoutedEventArgs e)
        {
            OK();
        }

        private void OK()
        {
            this.DialogResult = true;
            this.Close();
        }

        private void ButAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void RadWindow_Closed(object sender, WindowClosedEventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
                ad.CheckForUpdateAsyncCancel();
                ad.UpdateAsyncCancel();
            }
        }



        private bool CheckConnection(String URL)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
                request.Timeout = 5000;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                if (response.StatusCode == HttpStatusCode.OK) return true;
                else return false;
            }
            catch
            {
                return false;
            }
        }


        public void UpdateApplication()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                if (!CheckConnection("http://www.ffjudo.com/"))
                {
                    //LabelMessage.Text = "Connection internet innexistante ou trop faible débit.\nImpossible de vérifier la disponibilité d\'une MAJ.";
                    //ButOk.Visibility = Visibility.Visible;
                    // LogTools.Trace(new AppUpdateException("Connection internet inexistante ou trop faible débit."), LogTools.Level.INFO);
                    LogTools.Info(new AppUpdateException("Connection internet inexistante ou trop faible débit."));
                    OK();
                    return;
                }

                ApplicationDeployment app = ApplicationDeployment.CurrentDeployment;
                app.CheckForUpdateCompleted += new CheckForUpdateCompletedEventHandler(ad_CheckForUpdateCompleted);
                app.CheckForUpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_CheckForUpdateProgressChanged);

                app.CheckForUpdateAsync();
            }
            else
            {
                this.Close();
            }

        }

        void ad_CheckForUpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            LabelMessage.Text = String.Format("Téléchargement: {0}. {1:D}Ko/{2:D}Ko.", GetProgressString(e.State), e.BytesCompleted / 1024, e.BytesTotal / 1024);
        }

        string GetProgressString(DeploymentProgressState state)
        {
            if (state == DeploymentProgressState.DownloadingApplicationFiles)
            {
                return "Fichiers application";
            }
            else if (state == DeploymentProgressState.DownloadingApplicationInformation)
            {
                return "Manifeste de l\'application";
            }
            else
            {
                return "Manifeste de deployment";
            }
        }

        void ad_CheckForUpdateCompleted(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                // LogTools.Trace(new AppUpdateException("Problème MAJ.", e.Error), LogTools.Level.INFO);
                LogTools.Info(new AppUpdateException("Problème MAJ.", e.Error));

                //LabelMessage.Text = "Une erreur est survenue: \n" + e.Error.Message;                
                //ButOk.Visibility = Visibility.Visible;
                //ButDownMan.Visibility = Visibility.Visible;

                OK();
                return;
            }
            else if (e.Cancelled == true)
            {
                LabelMessage.Text = "La mise à jour a été annulée.";
                ButOk.Visibility = Visibility.Visible;
                return;
            }

            if (e.UpdateAvailable)
            {
                if (!e.IsUpdateRequired)
                {
                    LabelMessage.Text = "Une mise à jour est diponnible, voulez-vous la télécharger ?";
                    ButDownOui.Visibility = Visibility.Visible;
                    ButDownNon.Visibility = Visibility.Visible;
                    ButDownMan.Visibility = Visibility.Visible;
                }
                else
                {
                    LabelMessage.Text = "Une mise à jour obligatoire est diponnible. Cette mise à jour va être installé.\nVeuillez mettre vos ordinateurs CS à jour.";
                    ButDownOK.Visibility = Visibility.Visible;
                }
            }
            else
            {
                this.Close();
            }
        }

        private void BeginUpdate()
        {
            ApplicationDeployment ad = ApplicationDeployment.CurrentDeployment;
            ad.UpdateCompleted += new AsyncCompletedEventHandler(ad_UpdateCompleted);

            ad.UpdateProgressChanged += new DeploymentProgressChangedEventHandler(ad_UpdateProgressChanged);
            ad.UpdateAsync();
        }

        void ad_UpdateProgressChanged(object sender, DeploymentProgressChangedEventArgs e)
        {
            String progressText = String.Format("{0:D}Ko/{1:D}Ko - {2:D}% complete", e.BytesCompleted / 1024, e.BytesTotal / 1024, e.ProgressPercentage);
            LabelMessage.Text = progressText;
        }

        void ad_UpdateCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                LabelMessage.Text = "La mise à jour de la dernière version de l\'application a été annulée.";
                ButOk.Visibility = Visibility.Visible;
                return;
            }
            else if (e.Error != null)
            {
                LabelMessage.Text = "La mise à jour n\'a pas pu s\'effectuer. Une erreur est survenue: \n" + e.Error.Message;
                LogTools.Info(new AppUpdateException("Problème MAJ.", e.Error));
                ButOk.Visibility = Visibility.Visible;
                ButDownMan.Visibility = Visibility.Visible;
                return;
            }

            LabelMessage.Text = "L\'application a été mise à jour. Veuillez relancer l\'application.";

            ButOk.Visibility = Visibility.Visible;

            //DialogResult dr = MessageBox.Show("The application has been updated. Restart? (If you do not restart now, the new version will not take effect until after you quit and launch the application again.)", "Restart Application", MessageBoxButtons.OKCancel);
            //if (DialogResult.OK == dr)
            //{
            //    Application.Restart();
            //}
        }

        private void ButDownOui_Click(object sender, RoutedEventArgs e)
        {
            BeginUpdate();
            ButOk.Visibility = Visibility.Collapsed;
            ButAnnuler.Visibility = Visibility.Collapsed;
            ButDownOui.Visibility = Visibility.Collapsed;
            ButDownMan.Visibility = Visibility.Collapsed;
            ButDownNon.Visibility = Visibility.Collapsed;
            ButDownOK.Visibility = Visibility.Collapsed;
        }

        private void ButDownNon_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void ButDownOK_Click(object sender, RoutedEventArgs e)
        {
            BeginUpdate();
            ButOk.Visibility = Visibility.Collapsed;
            ButAnnuler.Visibility = Visibility.Collapsed;
            ButDownOui.Visibility = Visibility.Collapsed;
            ButDownMan.Visibility = Visibility.Collapsed;
            ButDownNon.Visibility = Visibility.Collapsed;
            ButDownOK.Visibility = Visibility.Collapsed;
        }

        private void ButDownMan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = "https://dev.licences-ffjudo.com/tas/publish.htm";
                System.Diagnostics.Process.Start(url);

                this.DialogResult = false;
                this.Close();
            }
            catch { }
        }
    }
}
