using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeJsSiteManager.Models;
using System.IO;
namespace NodeJsSiteManager.Modules
{
    public class BackUpManager
    {
        public void BackUpSite(Site site, string targetDirectory)
        {
            try
            {
                if (String.IsNullOrEmpty(targetDirectory))
                    throw new Exception("Please Specify Ouput Directory");

                Helpers.CopyDirectory(Path.Combine(site.SiteLocation, site.SiteName), targetDirectory);
            }

           catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }

         
        }


    }
}
