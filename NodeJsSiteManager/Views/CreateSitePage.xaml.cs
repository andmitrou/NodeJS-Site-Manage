using NodeJsSiteManager.CommandLine;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using NodeJsSiteManager.Models;
using System.Text.RegularExpressions;

namespace NodeJsSiteManager.Views
{
    /// <summary>
    /// Interaction logic for CreateSite.xaml
    /// </summary>
    public partial class CreateSitePage : Page
    {
        public delegate void SitesUpdatedEventHandler(object sender, EventArgs e);

        public event SitesUpdatedEventHandler SitesUpdated;

        public CreateSitePage()
        {
            InitializeComponent();
        }

        private void btnCreateSite_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage.Visibility = Visibility.Hidden;
            try
            {
                if (String.IsNullOrEmpty(this.txtSiteName.Text)) throw new Exception("Site Name must Not be empty");

                if (String.IsNullOrEmpty(this.txtWebLocation.Text)) throw new Exception("Location must Not be empty");

                if (String.IsNullOrEmpty(this.txtPort.Text) ||  
                    Regex.IsMatch(this.txtPort.Text, @"\d") || 
                    ((Int32.Parse(this.txtPort.Text) > 65535)))
                        throw new Exception("Port number is not valid it should be in range 0-65535");

                List<string> commandStringList = new List<string>();

                CommandParser cmdParser = new CommandParser(Environment.CurrentDirectory + "\\commands.json");

                

                Dictionary<string, string[]> CommandsForExecution = new Dictionary<string, string[]>();
                CommandsForExecution.Add("NPMInit", new string[] { });
                CommandsForExecution.Add("NPMInstallPackage", new string[] { "--save express" });
       
                CommandsForExecution.Add("CmdCreateDirectory", new string[] { "public" });
    
                foreach (var pair in CommandsForExecution)
                {
                    var com = cmdParser.GetCommand(pair.Key).Generate(pair.Value);
                    commandStringList.Add(com);
                }

                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(this.txtWebLocation.Text, this.txtSiteName.Text));
                CommandLineManager mgr = new CommandLineManager(System.IO.Path.Combine(this.txtWebLocation.Text, this.txtSiteName.Text));
                mgr.Commands = commandStringList;
                mgr.Run();

                //Create QuickStart Scripts
                var moduleDir = System.IO.Path.Combine(this.txtWebLocation.Text,
                                                       this.txtSiteName.Text,
                                                      "nsm-server-config");
                Directory.CreateDirectory(moduleDir);

                var packConfigContent = @"{
                    'name': 'nsm-server-config',
                    'description':'Server Configuration NSM',
                    'version': '1.0.0',
                    'main': 'index.js'
                 }";

                System.IO.File.WriteAllText(System.IO.Path.Combine(moduleDir, "index.js"), 
                                            String.Format("module.exports.portNumber = {0}",this.txtPort.Text));

                System.IO.File.WriteAllText(System.IO.Path.Combine(moduleDir, "package.json"), packConfigContent);
              

                if (this.cmbWebServer.Text.ToLower() == "express")
                {
                    File.Copy(System.IO.Path.Combine(Environment.CurrentDirectory, "QuickStartScripts\\Express.js"),
                                            System.IO.Path.Combine(this.txtWebLocation.Text,
                                                                   this.txtSiteName.Text ,
                                                                   "server.js"));
                }

                App.siteManager.CreateSite(new Site
                {
                    SiteName = this.txtSiteName.Text,
                    SiteLocation = txtWebLocation.Text,
                    SitePort = Int32.Parse(txtPort.Text)
                });

                App.siteManager.Save();

                OnSitesUpdated(new EventArgs());
            }
            catch (Exception ex)
            {
                ErrorMessage.Visibility = Visibility.Visible;
                ErrorMessage.Content = ex.Message;
            }

        }

        protected virtual void OnSitesUpdated(EventArgs e)
        {
            if (SitesUpdated != null)
            {
                SitesUpdated(this, e);
            }
        }

        private void btnFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
                this.txtWebLocation.Text = dialog.SelectedPath;

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).NavigationFrame.Navigate(new Home());
        }
    }
}
