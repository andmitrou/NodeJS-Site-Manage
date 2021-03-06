﻿using NodeJsSiteManager.Modules;
using NodeJsSiteManager.ViewModels;
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

namespace NodeJsSiteManager.Views
{
    /// <summary>
    /// Interaction logic for BackUpPage.xaml
    /// </summary>
    public partial class BackUpPage : Page
    {
        public BackUpPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSites();
        }

        private void BackUpBtn_Click(object sender, RoutedEventArgs e)
        {
            ResultMessage.Content = "";

            var backUpManager = new BackUpManager();

            if (String.IsNullOrEmpty(this.txtSelectLoc.Text))
            {
                MessageBox.Show("Please specify Locatiion");
                return;
            }

            foreach (var siteItem in this.SitesListBox.Items)
            {
                var item = (BackUpSiteItemTemplate)siteItem;
                if (item.isChecked)
                {
                    ResultMessage.Content += "Performing BackUp for site" + item.SiteName + Environment.NewLine;
                    try
                    {
                        backUpManager.BackUpSite(item, this.txtSelectLoc.Text);
                        ResultMessage.Content += String.Format("Site {0} successfully backed Up",
                                                                item.SiteName) + Environment.NewLine;
                    }
                    catch (Exception ex)
                    {
                        ResultMessage.Content += String.Format("Site {0} back up falied. Error:{1}",
                                                                item.SiteName, ex.Message) + Environment.NewLine;
                    }
                }
            }
        }

        private void selectLocBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.txtSelectLoc.Text = dlg.SelectedPath;
            }
        }

        private void LoadSites()
        {
            List<dynamic> siteInfoList = new List<dynamic>();

            foreach (var site in App.siteManager.SiteCollection)
            {
                var siteInfo = new BackUpSiteItemTemplate
                {
                    SiteId = site.SiteId,
                    SiteName = site.SiteName,
                    SiteLocation = site.SiteLocation,
                    SitePort = site.SitePort,
                    isChecked = false,
                    Extensions = site.Extensions
                };
                siteInfoList.Add(siteInfo);
            }

            this.SitesListBox.ItemsSource = siteInfoList;
        }

        private void hyperlinkClose_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).NavigationFrame.Navigate(new Home());
            e.Handled = true;
        }
    }

}
