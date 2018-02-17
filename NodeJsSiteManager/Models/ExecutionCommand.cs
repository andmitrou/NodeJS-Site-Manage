using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeJsSiteManager.Models
{
   public class ExecutionCommand
    {
        public string CommandKeyName { get; set; }
        public string[] Parameters { get; set; }
        public int CommandOrder { get; set; }

    }
}
