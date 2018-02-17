using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeJsSiteManager.Models
{
    public class Command
    {
        public string CmdName { get; set; }
        public string[] CmdDefaultParameters { get; set; }
        public string CmdText { get; set; }

        public string Generate(params string[] args)
        {
            string result = null;

            if (args == null || args.Length == 0)
            {
                if (this.CmdDefaultParameters == null || CmdDefaultParameters.Length == 0)
                    throw new Exception("You must set Default parameters or Override!");

                result = String.Format("{0}", this.CmdText.Replace("{parameters}", String.Join(" ", this.CmdDefaultParameters)));
            }
            else
            {
                result = String.Format("{0}", this.CmdText.Replace("{parameters}", String.Join(" ", args)));
            }

            return result;
        }
    }
}
