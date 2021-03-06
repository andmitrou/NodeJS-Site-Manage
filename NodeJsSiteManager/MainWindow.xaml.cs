﻿using NodeJsSiteManager.Models;
using NodeJsSiteManager.Views;
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
using NodeJsSiteManager.Modules;

namespace NodeJsSiteManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void RefreshTreeView()
        {
            var sites = App.siteManager.SiteCollection;
            if (sites != null)
            {
                TreeViewItem sitesNode = (TreeViewItem)this.SitesTree.Items[0];
                sitesNode.ItemsSource = sites;
                sitesNode.Items.Refresh();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.NavigationFrame.Navigate(new Home());
            RefreshTreeView();
        }

        public void SitesUpdated(object sender, EventArgs args)
        {
            RefreshTreeView();           
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var createSitePage = new CreateSitePage();
            createSitePage.SitesUpdated += SitesUpdated;
            this.NavigationFrame.Navigate(createSitePage);
        }

        private void cxtEdit_Click(object sender, RoutedEventArgs e)
        {
            var item = (Site)this.SitesTree.SelectedItem;
            var editPage = new EditSitePage(item);
            editPage.SitesUpdated += SitesUpdated;
            this.NavigationFrame.Navigate(editPage);
        }



        private void ctxBrowse_Click(object sender, RoutedEventArgs e)
        {
            var selectedSite = (Site)this.SitesTree.SelectedItem;
            var siteDirectoryPath = System.IO.Path.Combine(selectedSite.SiteLocation, selectedSite.SiteName);
            System.Diagnostics.Process.Start(siteDirectoryPath);
        }

        private void ctxDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool allowPhysicalDelete = false;

                var selectedSite = (Site)this.SitesTree.SelectedItem;

                SiteManager siteManager = new SiteManager();

                var msgResult = MessageBox.Show("Do you want to delete the files?", "", MessageBoxButton.YesNo);

                if (msgResult.ToString() == "Yes")
                    allowPhysicalDelete = true;

                siteManager.RemoveSite(selectedSite, allowPhysicalDelete);

                siteManager.Save();
                App.siteManager.SiteCollection.Remove(selectedSite);
                RefreshTreeView();
                ((MainWindow)System.Windows.Application.Current.MainWindow).NavigationFrame.Navigate(new Home());
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuItemsSitesStatus_Click(object sender, RoutedEventArgs e)
        {
            var siteStatusesPage = new SitesRunningStatusPage();
            this.NavigationFrame.Navigate(siteStatusesPage);
        }

        private void MenuItemBackUpSite_Click(object sender, RoutedEventArgs e)
        {
            var bcpSite = new BackUpPage();
            this.NavigationFrame.Navigate(bcpSite);
        }
    }
}
