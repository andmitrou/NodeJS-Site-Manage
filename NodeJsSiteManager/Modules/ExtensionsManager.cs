using NodeJsSiteManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NodeJsSiteManager.CommandLine;

namespace NodeJsSiteManager.Modules
{
    public class ExtensionsManager
    {
        private Site _site;
        public ExtensionsManager(Site site)
        {
            _site = site;
        }

        public List<NodeExtension> GetAvailableExtensions()
        {
            var extensionsFile = Path.Combine(Environment.CurrentDirectory, "NodeExtensions.json");
            var extensions = Helpers.ReadJsonFile<NodeExtension>(extensionsFile);
            return extensions;
        }

        private string ExecuteExtensionCommand(string commandName, string[] commandParameters, bool exitImmediately = false)
        {
            var npmExecutor = new NSMCommandExecutor(System.IO.Path.Combine(this._site.SiteLocation, _site.SiteName));
            var rst = npmExecutor.ExecuteCommand(commandName, commandParameters, exitImmediately);
            return rst;
        }

        public void InstallExtension(string extensionKey)
        {
            string res = ExecuteExtensionCommand("NPMInstallPackage", new string[] { extensionKey, "--save" });

            if (!String.IsNullOrEmpty(res)) this._site.Extensions.Add(extensionKey);

        }

        public void UnistallExtension(string extensionKey)
        {
            string res = ExecuteExtensionCommand("NPMUnInstallPackage", new string[] { extensionKey, "--save" });

            if (!String.IsNullOrEmpty(res)) this._site.Extensions.Remove(extensionKey);
        }
    }
}
