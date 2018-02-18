using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeJsSiteManager.Models
{
    public class Site
    {
        public string SiteId { get; set; }
        public string SiteName { get; set; }
        public string SiteLocation { get; set; }
        public int SitePort { get; set; }
        public List<string> Extensions { get; set; }
    }
}
