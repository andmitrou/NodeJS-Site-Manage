using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NodeJsSiteManager.Models;

namespace NodeJsSiteManager.CommandLine
{
    public class CommandParser
    {
        private List<Command> commands = null;
        public CommandParser(string fileName)
        {
            commands = Helpers.ReadJsonFile<Command>(fileName);
        }
        public Command GetCommand(string commandName)
        {
            return commands.SingleOrDefault(x=>x.CmdName == commandName);
        } 
    }
}
