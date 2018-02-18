using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using NodeJsSiteManager.Models;
using System.Windows;

namespace NodeJsSiteManager.Modules
{
    public class SiteManager
    {
        public List<Site> SiteCollection;

        private string sitesFilePath;
        public SiteManager()
        {
            sitesFilePath = Path.Combine(Environment.CurrentDirectory, "sites.json");

            if (!File.Exists(sitesFilePath)) File.Create(sitesFilePath).Dispose();

            var contents = File.ReadAllText(sitesFilePath);

            if (!String.IsNullOrEmpty(contents))
            {
                this.SiteCollection = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Site>>(contents);
            }
        }

        private bool SiteExists(Site site)
        {
            if (this.SiteCollection == null) return false;

            var count = this.SiteCollection.Count(x => x.SiteId == site.SiteId);
            return count > 0;
        }

        public void CreateSite(Site site)
        {
            try
            {
                if (SiteExists(site)) throw new Exception("Site Already Exists");

                if (SiteCollection == null) this.SiteCollection = new List<Site>();

                this.SiteCollection.Add(site);
            }
            catch
            {
                throw;
            }
        }
        public void RenameSiteDirectory(string sourceDir, string newName)
        {
            var newDirectoryPath = Path.Combine(sourceDir.Substring(0, sourceDir.LastIndexOf("\\")), newName);
            Directory.Move(sourceDir, newDirectoryPath);

        }

        public void UpdateSite(Site site)
        {
            try
            {
                for (var i = 0; i < this.SiteCollection.Count; i++)
                {
                    if (this.SiteCollection[i].SiteId == site.SiteId)
                    {
                        this.SiteCollection[i] = site;
                        return;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public void RemoveSite(Site site, bool deletePhysicalFiles = false)
        {
            try
            {
                var list = this.SiteCollection.ToList();
                var itemToRemove = list.FirstOrDefault(x => x.SiteId == site.SiteId);
                if (itemToRemove != null)
                {
                    list.Remove(itemToRemove);
                    this.SiteCollection = list;
                }

                if (deletePhysicalFiles)
                {
                    var directory = Path.Combine(site.SiteLocation, site.SiteName);

                    if (Directory.Exists(directory))
                        Directory.Delete(directory, true);
                }
            }
            catch
            {
                throw;
            }

        }

        public void Save()
        {
            var serialisedSites = Newtonsoft.Json.JsonConvert.SerializeObject(this.SiteCollection);
            File.WriteAllText(this.sitesFilePath, serialisedSites);
        }

        internal void UpdateSitePortConfigFile(Site site, int newPort)
        {
            var moduleDir = System.IO.Path.Combine(site.SiteLocation,
                                                     site.SiteName,
                                                    "nsm-server-config");

            System.IO.File.WriteAllText(System.IO.Path.Combine(moduleDir, "index.js"),
                                           String.Format("module.exports.portNumber = {0}", newPort.ToString()));
        }
    }
}
