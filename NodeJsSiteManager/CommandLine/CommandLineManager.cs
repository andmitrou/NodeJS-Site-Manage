using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeJsSiteManager.CommandLine
{
    public class CommandLineManager
    {
        Process proc;

        ProcessStartInfo procInfo;

        public string WorkingDirectory
        {
            get { return this.workingDirectory; }
            set { this.workingDirectory = value; }
        }

        private string workingDirectory;

        public string Output;
        public IEnumerable<string> Commands { get; set; }

        public CommandLineManager(string workingDir)
        {
            this.workingDirectory = workingDir;

            this.proc = new System.Diagnostics.Process();

            procInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
        }

        public void Run(bool exitImmediatelyAfterCommnand = false)
        {
            this.Output = null;

            procInfo.WorkingDirectory = this.workingDirectory;

            proc.StartInfo = this.procInfo;

            var stdOutput = new StringBuilder();

            proc.OutputDataReceived += (sender, args) =>
            {
                var str = args.Data;               
                stdOutput.AppendLine(args.Data);
            };

            try
            {
                proc.Start();

                proc.BeginOutputReadLine();

                System.IO.StreamWriter pipeCommandSender = proc.StandardInput;

                if (this.Commands.Count() == 0) return;

                foreach(var commandText in this.Commands)
                {
                    pipeCommandSender.WriteLine(commandText);
                }

                if (!exitImmediatelyAfterCommnand)
                {
                    pipeCommandSender.WriteLine("exit");
                    proc.WaitForExit();
                }               
            }

            catch
            {
                throw;
            }
            if (!exitImmediatelyAfterCommnand)
            {
                if (proc.ExitCode == 0)
                {
                    Output = stdOutput.ToString();
                }
                else
                {
                    var message = new StringBuilder();

                    if (stdOutput.Length != 0)
                    {
                        message.AppendLine("Std output:");
                        message.AppendLine(stdOutput.ToString());
                    }
                }
            }
          
        }
    }
}