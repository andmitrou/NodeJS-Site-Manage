using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeJsSiteManager.CommandLine
{
   public class NSMCommandExecutor
    {
        private string _workingDir;

        public NSMCommandExecutor(string workingDirectory)
        {
            this._workingDir = workingDirectory;
        }

        public string ExecuteCommand(string commandName, string[] commandParameters, bool exitImmediately = false)
        {
          
            var cmdManager = new CommandLine.CommandLineManager(this._workingDir);
            var parser = new CommandLine.CommandParser(Environment.CurrentDirectory +
                                                 "\\commands.json");
            var command = parser.GetCommand(commandName).Generate(commandParameters);
            cmdManager.Commands = new List<string> { command };
            cmdManager.Run(exitImmediately);
            return cmdManager.Output;
        }
    }
}
