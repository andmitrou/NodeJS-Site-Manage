using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NodeJsSiteManager.ViewModels
{
   public class ExtensionItemTemplate
    {
        public string Title { get; set; }
        public bool IsActive { get; set; }
        public Brush Color { get; set; }
        public  string Action { get; set; }
        public string ExtensionKey { get; set; }

    }
}
