using NodeJsSiteManager.Networking;
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
    /// Interaction logic for SitesRunningStatusPage.xaml
    /// </summary>
    public partial class SitesRunningStatusPage : Page
    {
        public SitesRunningStatusPage()
        {
            InitializeComponent();
        }

        private void LoadSitesStatus()
        {

            var siteInfoList = new List<dynamic>();
            var sites = App.siteManager.SiteCollection;
            foreach (var site in sites)
            {
                var siteIsRunning = Utils.ServerIsListening("localhost", site.SitePort);
                var siteInfo = new
                {
                    Id = site.SiteId,
                    SiteName = site.SiteName,
                    SitePort = site.SitePort,
                    IsRunning = siteIsRunning,
                    Action = siteIsRunning ? "Stop" : "Start"
                };

                siteInfoList.Add(siteInfo);

            }
            SitesStatusListBox.ItemsSource = siteInfoList;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSitesStatus();
        }

        private void hyperlinkClose_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            ((MainWindow)System.Windows.Application.Current.MainWindow).NavigationFrame.Navigate(new Home());
            e.Handled = true;
        }

        private void btnStartStop_Click(object sender, RoutedEventArgs e)
        {
            var btn = (Button)sender;
            var siteInfo = (dynamic)btn.DataContext;
            var site = App.siteManager.SiteCollection.SingleOrDefault(x => x.SiteId == siteInfo.Id);

            if (site == null) throw new NullReferenceException("Site not found");

            try
            {
                if (siteInfo.IsRunning)
                    App.siteManager.StopWebSite(site);
                else
                    App.siteManager.StartWebSite(site);

                System.Threading.Thread.Sleep(2100);

                LoadSitesStatus();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
    }
}
