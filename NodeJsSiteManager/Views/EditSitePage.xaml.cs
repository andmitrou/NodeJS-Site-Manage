﻿using NodeJsSiteManager.CommandLine;
using NodeJsSiteManager.Models;
using NodeJsSiteManager.Modules;
using NodeJsSiteManager.Networking;
using NodeJsSiteManager.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace NodeJsSiteManager.Views
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    /// 

    public partial class EditSitePage : Page
    {
        public delegate void SitesUpdatedEventHandler(object sender, EventArgs e);

        public event SitesUpdatedEventHandler SitesUpdated;

        private bool SiteIsRunning = false;
        private Site _site;
        private ExtensionsManager extensionsManager;
        public EditSitePage()
        {
            InitializeComponent();
        }
        public EditSitePage(Site site) : this()
        {
            this._site = site;
            this.txtSiteName.Text = _site.SiteName;
            this.txtWebLocation.Content = _site.SiteLocation;
            this.txtPort.Text = _site.SitePort.ToString();
            extensionsManager = new ExtensionsManager(_site);
        }

        protected virtual void OnSitesUpdated(EventArgs e)
        {
            if (SitesUpdated != null)
            {
                SitesUpdated(this, e);
            }
        }

        private void LoadExtensions()
        {
            var extensions = extensionsManager.GetAvailableExtensions();

            List<ExtensionItemTemplate> extensionTemplates = new List<ExtensionItemTemplate>();

            foreach (var ext in extensions)
            {
                ExtensionItemTemplate itemTemplate = new ExtensionItemTemplate();
                itemTemplate.ExtensionKey = ext.NPMName;
                if (_site.Extensions != null && _site.Extensions.Find(x => x == ext.NPMName) != null)
                {

                    itemTemplate.Color = Brushes.Gray;
                    itemTemplate.Action = "Uninstall";
                    itemTemplate.Title = ext.Name;
                    itemTemplate.IsActive = true;
                }
                else
                {
                    itemTemplate.Color = Brushes.Red;
                    itemTemplate.Action = "Install";
                    itemTemplate.Title = ext.Name;
                    itemTemplate.IsActive = false;
                }

                extensionTemplates.Add(itemTemplate);
            }

            this.ExtensionsListBox.ItemsSource = extensionTemplates;
        }

        private void btnSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            bool sitePropertiesChanged = false;

            if (!this.SiteIsRunning)
            {
                if (this._site.SiteName != this.txtSiteName.Text)
                {
                    sitePropertiesChanged = true;
                    App.siteManager.RenameSiteDirectory(System.IO.Path.Combine(_site.SiteLocation, _site.SiteName), 
                                                                            this.txtSiteName.Text);
                    _site.SiteName = this.txtSiteName.Text;
                }

                if (this._site.SitePort.ToString() != this.txtPort.Text)
                {
                    sitePropertiesChanged = true;
                    int newPort = Int32.Parse(this.txtPort.Text);
                    App.siteManager.UpdateSitePortConfigFile(_site, newPort);
                    _site.SitePort = newPort;
                }

                if (sitePropertiesChanged)
                {
                    App.siteManager.UpdateSite(_site);
                    App.siteManager.Save();
                    OnSitesUpdated(new EventArgs());
                }
            }
            else
            {
                MessageBox.Show("You need to stop the Site first!");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.SiteIsRunning = Utils.ServerIsListening("localhost", _site.SitePort);

            if (this.SiteIsRunning)
            {
                this.hyperlinkSite.Visibility = Visibility.Visible;
                SetStatusSiteIndicators("start");
            }
            else
            {
                this.hyperlinkSite.Visibility = Visibility.Hidden;
                SetStatusSiteIndicators("stop");
            }

            LoadExtensions();
        }

        private void SetStatusSiteIndicators(string command)
        {
            if (command == "start")
            {
                websiteStatusLbl.Content = "Running";
                websiteStatusLbl.Foreground = Brushes.Green;
                this.btnStartSite.IsEnabled = false;
                this.btnStopSite.IsEnabled = true;
            }
            else if (command == "stop")
            {
                websiteStatusLbl.Content = "Stopped";
                websiteStatusLbl.Foreground = Brushes.Red;
                this.btnStartSite.IsEnabled = true;
                this.btnStopSite.IsEnabled = false;
            }
        }

        public static string LookupProcess(int pid)
        {
            string procName;
            try { procName = Process.GetProcessById(pid).ProcessName; }
            catch (Exception) { procName = "-"; }
            return procName;
        }
        private string ExecuteCommand(string commandName, string[] commandParameters, bool exitImmediately = false)
        {
            var workingDir = System.IO.Path.Combine(this._site.SiteLocation, _site.SiteName);
            var npmExecutor = new NSMCommandExecutor(workingDir);
            var rst = npmExecutor.ExecuteCommand(commandName, commandParameters, exitImmediately);

            return rst;
        }

        private void btnStartWebSite_Click(object sender, RoutedEventArgs e)
        {
            var result = ExecuteCommand("NodeStartWebSite", new string[] { "server.js" }, true);
            SetStatusSiteIndicators("start");
            this.hyperlinkSite.Visibility = Visibility.Visible;
            this.SiteIsRunning = true;
        }

        private void btnStopWebSite_Click(object sender, RoutedEventArgs e)
        {
            string PID = "";

            var result = ExecuteCommand("CmdGetProcess", new string[] { });

            string[] rows = Regex.Split(result, "\r\n");

            foreach (string row in rows)
            {
                string[] tokens = Regex.Split(row, "\\s+");

                if (tokens.Length > 4 && (tokens[1].Equals("UDP") || tokens[1].Equals("TCP")))
                {
                    string localAddress = Regex.Replace(tokens[2], @"\[(.*?)\]", "1.1.1.1");
                    var protocol = localAddress.Contains("1.1.1.1") ? String.Format("{0}v6", tokens[1]) : String.Format("{0}v4", tokens[1]);
                    var port_number = localAddress.Split(':')[1];
                    if (port_number == this._site.SitePort.ToString())
                    {
                        PID = tokens[1] == "UDP" ? tokens[4] : tokens[5];
                        break;
                    }
                }
            }

            var aProcess = System.Diagnostics.Process.GetProcessById(Int32.Parse(PID));
            aProcess.Kill();
            SetStatusSiteIndicators("stop");
            this.hyperlinkSite.Visibility = Visibility.Hidden;
            this.SiteIsRunning = false;

        }


        public void OpenSiteOnBrowser()
        {
            string targetUrl = "http://localhost:" + this._site.SitePort;

            try
            {
                System.Diagnostics.Process.Start(targetUrl);
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }

        private void btnExtensionAction_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var extension = (ExtensionItemTemplate)btn.DataContext;

            if (extension.IsActive)
                this.extensionsManager.UnistallExtension(extension.ExtensionKey);
            else
                this.extensionsManager.InstallExtension(extension.ExtensionKey);

            
            App.siteManager.UpdateSite(_site);
            App.siteManager.Save();

            LoadExtensions();
        }

        private void hyperlinkSite_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            OpenSiteOnBrowser();
            e.Handled = true;
        }

        private void hyperlinkClose_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).NavigationFrame.Navigate(new Home());
            e.Handled = true;
        }
    }
}
